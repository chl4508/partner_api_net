using Colorverse.Common.Errors;
using Colorverse.Application.Mediator;
using Colorverse.Database;
using Colorverse.Meta.Documents;
using Colorverse.Common.Exceptions;
using CvFramework.MongoDB.Extensions;
using MongoDB.Bson;
using MongoDB.Driver;
using Colorverse.Meta.DataTypes.Resource;
using Colorverse.Application.Session;
using MediatR;
using Colorverse.ResourceLibrary;
using ResourceLibrary.Nats.V1.DataTypes;
using CvFramework.Common;
using Colorverse.Meta.Apis.DataTypes.Asset;
using Colorverse.Common.Helper;
using Colorverse.Common.DataTypes;

namespace Colorverse.Meta.Mediators;

/// <summary>
/// 애셋 Mediator
/// </summary>
[MediatorHandler]
public class AssetMediatorHandler : MediatorHandlerBase,
    IMediatorHandler<GetAssetRequest, AssetResponse>,
    IMediatorHandler<GetAssetListRequest, CvListResponse<AssetResponse>>,
    IMediatorHandler<CreateAssetRequest, AssetResponse>,
    IMediatorHandler<SaleInspectionAssetRequest, AssetResponse>,
    IMediatorHandler<SaleStartAssetRequest, AssetResponse>,
    IMediatorHandler<SaleStopAssetRequest, AssetResponse>

{
    private readonly IContextUserProfile _profile;
    private readonly IDbContext _dbContext;
    private readonly IMediator _mediator;
    private readonly IResourceLibrary _resourceLibrary;

    /// <summary>
    /// Asset Mediator 설정
    /// </summary>
    /// <param name="profile"></param>
    /// <param name="dbContext"></param>
    /// <param name="mediator"></param>
    /// <param name="resourceLibrary"></param>
    public AssetMediatorHandler(IContextUserProfile profile, IDbContext dbContext, IMediator mediator, IResourceLibrary resourceLibrary)
    {
        _profile = profile;
        _dbContext = dbContext;
        _mediator = mediator;
        _resourceLibrary = resourceLibrary;
    }

    /// <summary>
    /// 애셋 상세정보
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<AssetResponse> Handle(GetAssetRequest request, CancellationToken cancellationToken)
    {
        var assetDoc = await _dbContext.GetDocAsync<AssetDoc>(Uuid.FromBase62(request.Assetid), cancellationToken);
        if (!assetDoc.Exists)
        {
             throw new ErrorNotfoundId(request.Assetid);
        }
        
        if (request.MeCheck && assetDoc.GetUuid("profileid") != _profile.ProfileId)
        {
            throw new ErrorBadRequest("Not matching Profileid " + _profile.ProfileId);
        }
        
        if (!request.MeCheck && assetDoc.GetInt32("option", "sale_status") != 3)
        {
            throw new ErrorNotfoundId(request.Assetid);
        }

        var response = assetDoc.To<AssetResponse>();
        response.Resource ??= new AssetResourceResponse();
        response.Resource.Manifest = PublicUrlHelper.GetAssetManifestUri(response.Id, response.Option.Version);
        response.Resource.Thumbnail = response.Resource.Thumbnail != null || response.Resource.Thumbnail != "" ? PublicUrlHelper.GetResourceImageUri(response.Resource.Thumbnail!) : PublicUrlHelper.GetAssetThumbnailtUri(response.Id, response.Option.Version);
        return response;
    }

    /// <summary>
    /// 애셋 리스트
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<CvListResponse<AssetResponse>> Handle(GetAssetListRequest request, CancellationToken cancellationToken)
    {
        var filter = Builders<AssetResponse>.Filter.Eq("profileid", Uuid.FromBase62(request.Profileid).ToBsonBinaryData());
        if (!request.MeCheck)
        {
            filter &= Builders<AssetResponse>.Filter.Eq("option.sale_status", 3);
        }
        if (request.NoneSale != null)
        {
            filter &= Builders<AssetResponse>.Filter.Eq("option.none_sale", request.NoneSale);
        }
        if (request.Category != null)
        {
            filter &= Builders<AssetResponse>.Filter.In("option.category", new BsonArray() { request.Category });
        }

        var options = new FindOptions<AssetResponse>
        {
            Skip = request.Page - 1,
            Limit = request.Limit
        };

        var assetList = await _dbContext.GetListAsync("asset", filter, options, cancellationToken);
        foreach (var asset in assetList)
        {
            asset.Resource ??= new AssetResourceResponse();
            asset.Resource.Manifest = PublicUrlHelper.GetAssetManifestUri(asset.Id, asset.Option.Version);
            asset.Resource.Thumbnail = asset.Resource.Thumbnail != null || asset.Resource.Thumbnail != "" ? PublicUrlHelper.GetResourceImageUri(asset.Resource.Thumbnail!) : PublicUrlHelper.GetAssetThumbnailtUri(asset.Id, asset.Option.Version);
        }
        var Count = new CvListCountResponse
        {
            Current = request.Page,
            Page = request.Page,
            Limit = request.Limit,
        };
        var response = new CvListResponse<AssetResponse>(Count, assetList);
        return response;
    }

    /// <summary>
    /// 애셋 생성
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<AssetResponse> Handle(CreateAssetRequest request, CancellationToken cancellationToken)
    {
        AssetDoc assetDoc;
        AssetRevisionDoc assetRevisionDoc;
        var assetUuid = Uuid.FromBase62(request.Assetid);
        //애셋 존재하는지 체크
        var findasset = await _dbContext.GetDocAsync<AssetDoc>(assetUuid, cancellationToken);
        if (findasset.Exists)
        {
            throw new ErrorBadRequest("Duplicate assetid " + nameof(request.Assetid));
        }

        assetDoc = AssetDoc.Create(assetUuid);
        _dbContext.Add(assetDoc);

        assetDoc.ValidationCheck(request, _profile.IsAdmin);


        //등록 수수료 부과 ( 성공시 다음 로직 진행)


        assetRevisionDoc = AssetRevisionDoc.Create(assetUuid, request.Option.Version);
        _dbContext.Add(assetRevisionDoc);

        // resource db 에서 version 및 type 정상적인 값인지 한번더 검증필요
        var resourceDto = await _mediator.Send(new GetResourceParam(assetUuid, request.Option.Version), cancellationToken);
        if (resourceDto != null && resourceDto.Manifest != null)
        {
            var version = resourceDto.Version;
            var type = resourceDto.Manifest.GetTreeValue("main", "type").AsString;

            if (request.Option.Version != version)
            {
                throw new ErrorInvalidParam("Not Matching Version =" + request.Option.Version);
            }
            if (type.Equals("Item"))
            {
                throw new ErrorInvalidParam("Not Matching Type id="+request.Assetid);
            }
            // manifest 정보를 itemrevisioncollection에 저장
            var manifest = resourceDto.Manifest;
            assetRevisionDoc.SetAssetRevision(0, manifest);
        }
        else
        {
            assetRevisionDoc.SetAssetRevision(0);
        }

        // 리소스 사용
        var useResouceResult = await _mediator.Send(new UseResourceParam(assetUuid, request.Option.Version), cancellationToken);
        if (useResouceResult == null) throw new ErrorInternal(nameof(useResouceResult));
        if(request.Resource.Thumbnail != null)
        {
            var thumbnailResult = await _resourceLibrary.AddReference(new AddReferenceParam(Uuid.FromBase62(request.Resource.Thumbnail)), cancellationToken);
            if (!thumbnailResult.Succeeded) throw new ErrorInternal(nameof(thumbnailResult));
        }


        assetDoc.SetAsset(request, _profile.ProfileId, _profile.WorldId, _profile.UserId);

        await assetRevisionDoc.SaveAsync(cancellationToken); // assetrevision create
        await assetDoc.SaveAsync(cancellationToken); // asset create

        var response = assetDoc.To<AssetResponse>();
        response.Resource ??= new AssetResourceResponse();
        response.Resource.Manifest = PublicUrlHelper.GetAssetManifestUri(response.Id, response.Option.Version);
        response.Resource.Thumbnail = response.Resource.Thumbnail != null || response.Resource.Thumbnail != "" ? PublicUrlHelper.GetResourceImageUri(response.Resource.Thumbnail!) : PublicUrlHelper.GetAssetThumbnailtUri(response.Id, response.Option.Version);
        return response;

    }

    /// <summary>
    /// 애셋 판매심사 요청
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ErrorNotfoundId"></exception>
    public async Task<AssetResponse> Handle(SaleInspectionAssetRequest request, CancellationToken cancellationToken)
    {
        var assetUuid = Uuid.FromBase62(request.Assetid);
        var assetDoc = await _dbContext.GetDocAsync<AssetDoc>(assetUuid, cancellationToken); // 애셋 판매요청시 판매할 애셋있는지 체크
        if (!assetDoc.Exists)
        {
            throw new ErrorNotfoundId(request.Assetid);
        }

        assetDoc.ValidationCheck(request, assetDoc, _profile.ProfileId, _profile.IsAdmin);

        //판매 수수료 부과 ( 성공시 다음 로직 진행)



        // 현재 판매심사요청하면 자동으로 판매승인을 내게처리 (판매일정이있으면 판매중으로, 일정이없으면 판매허용으로)
        // 나중에 백오피스 완료되면 SaleInspectionAsset 으로 사용
        assetDoc.SaleInspectionAsset2(request);
        await assetDoc.SaveAsync(cancellationToken);
        


        // 마켓 처리는 판매심사요청 자동 판매승인때문에 처리 나중엔 제거
        var marketProductDoc = await _dbContext.GetDocAsync<MarketProductDoc>(assetUuid, cancellationToken); // 마켓에 등록되있는지 체크
        if (marketProductDoc.Exists)
        {
            marketProductDoc.UpdateAssetMarketProduct();
            await marketProductDoc.SaveAsync(cancellationToken); // 기존에 데이터를 판매중으로 변경
        }
        else
        {
            marketProductDoc = MarketProductDoc.Create(assetUuid);
            _dbContext.Add(marketProductDoc);
            marketProductDoc.SetAssetMarketProduct(assetDoc);
            marketProductDoc.SetItems(1, assetUuid, 1); // 타입, 아이디, 수량
            await marketProductDoc.SaveAsync(cancellationToken); // 마켓 등록
        }
        // / 마켓 처리는 판매심사요청 자동 판매승인때문에 처리 나중엔 제거

        var response = assetDoc.To<AssetResponse>();
        response.Resource ??= new AssetResourceResponse();
        response.Resource.Manifest = PublicUrlHelper.GetAssetManifestUri(response.Id, response.Option.Version);
        response.Resource.Thumbnail = response.Resource.Thumbnail != null || response.Resource.Thumbnail != "" ? PublicUrlHelper.GetResourceImageUri(response.Resource.Thumbnail!) : PublicUrlHelper.GetAssetThumbnailtUri(response.Id, response.Option.Version);
        return response;
    }

    /// <summary>
    /// 애셋 판매
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ErrorNotfoundId"></exception>
    public async Task<AssetResponse> Handle(SaleStartAssetRequest request, CancellationToken cancellationToken)
    {
        var assetUuid = Uuid.FromBase62(request.Assetid);
        var assetDoc = await _dbContext.GetDocAsync<AssetDoc>(assetUuid, cancellationToken); // 애셋 판매요청시 판매할 애셋있는지 체크
        if (!assetDoc.Exists)
        {
            throw new ErrorNotfoundId(request.Assetid);
        }
        assetDoc.ValidationCheck(request, assetDoc, _profile.ProfileId);
        assetDoc.SaleStartAsset();
        await assetDoc.SaveAsync(cancellationToken); // 판매중 변경
        

        var marketProductDoc = await _dbContext.GetDocAsync<MarketProductDoc>(assetUuid, cancellationToken); // 마켓에 등록되있는지 체크
        if (marketProductDoc.Exists)
        {
            marketProductDoc.UpdateAssetMarketProduct();
            await marketProductDoc.SaveAsync(cancellationToken); // 기존에 데이터를 판매중으로 변경
        }
        else
        {
            marketProductDoc = MarketProductDoc.Create(assetUuid);
            _dbContext.Add(marketProductDoc);
            marketProductDoc.SetAssetMarketProduct(assetDoc);
            marketProductDoc.SetItems(1, assetUuid, 1); // 타입, 아이디, 수량
            await marketProductDoc.SaveAsync(cancellationToken); // 마켓 등록
        }

        var response = assetDoc.To<AssetResponse>();
        response.Resource ??= new AssetResourceResponse();
        response.Resource.Manifest = PublicUrlHelper.GetAssetManifestUri(response.Id, response.Option.Version);
        response.Resource.Thumbnail = response.Resource.Thumbnail != null || response.Resource.Thumbnail != "" ? PublicUrlHelper.GetResourceImageUri(response.Resource.Thumbnail!) : PublicUrlHelper.GetAssetThumbnailtUri(response.Id, response.Option.Version);
        return response;
    }

    /// <summary>
    /// 애셋 판매 중단
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<AssetResponse> Handle(SaleStopAssetRequest request, CancellationToken cancellationToken)
    {
        var assetUuid = Uuid.FromBase62(request.Assetid);
        var assetDoc = await _dbContext.GetDocAsync<AssetDoc>(assetUuid, cancellationToken); // 애셋있는지 체크
        if (!assetDoc.Exists)
        {
            throw new ErrorNotfoundId(request.Assetid);
        }

        assetDoc.ValidationCheck(request, assetDoc, _profile.ProfileId);
        assetDoc.SaleCancelAsset();
        await assetDoc.SaveAsync(cancellationToken); // 판매중지

        var marketProductDoc = await _dbContext.GetDocAsync<MarketProductDoc>(assetUuid, cancellationToken); // 마켓에 등록되있는지 체크
        if (marketProductDoc.Exists)
        {
            marketProductDoc.SaleStopStatus();
            await marketProductDoc.SaveAsync(cancellationToken); // 판매중지
        }

        var response = assetDoc.To<AssetResponse>();
        response.Resource ??= new AssetResourceResponse();
        response.Resource.Manifest = PublicUrlHelper.GetAssetManifestUri(response.Id, response.Option.Version);
        response.Resource.Thumbnail = response.Resource.Thumbnail != null || response.Resource.Thumbnail != "" ? PublicUrlHelper.GetResourceImageUri(response.Resource.Thumbnail!) : PublicUrlHelper.GetAssetThumbnailtUri(response.Id, response.Option.Version);
        return response;
    }
}
