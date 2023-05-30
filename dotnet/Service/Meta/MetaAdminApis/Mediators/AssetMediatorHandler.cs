using Colorverse.Application.Mediator;
using Colorverse.Common.DataTypes;
using Colorverse.Common.Errors;
using Colorverse.Common.Exceptions;
using Colorverse.Common.Helper;
using Colorverse.Database;
using Colorverse.MetaAdmin.Apis.DataTypes.Asset;
using Colorverse.MetaAdmin.DataTypes.Asset;
using Colorverse.MetaAdmin.DataTypes.Resource;
using Colorverse.MetaAdmin.Documents;
using Colorverse.ResourceLibrary;
using Colorverse.UserLibrary;
using CvFramework.Common;
using CvFramework.MongoDB.Extensions;
using MongoDB.Bson;
using MongoDB.Driver;
using ResourceLibrary.Nats.V1.DataTypes;

namespace Colorverse.MetaAdmin.Mediators;

/// <summary>
/// 
/// </summary>
public class AssetMediatorHandler : MediatorHandlerBase,
    IMediatorHandler<GetAssetRequest, AssetResponse>,
    IMediatorHandler<GetAssetListRequest, CvAdminListResponse<AssetResponse>>,
    IMediatorHandler<UpdateAssetRequest, AssetResponse>,
    IMediatorHandler<SaleApprovalAssetRequest, AssetResponse>,
    IMediatorHandler<SaleRejectionAssetRequest, AssetResponse?>,
    IMediatorHandler<DeleteAssetRequest, AssetResponse>,
    IMediatorHandler<DeleteCancelAssetRequest, AssetResponse>
{
    private readonly ILogger _logger;
    private readonly IDbContext _dbContext;
    private readonly IUserLibrary _userLibrary;
    private readonly IResourceLibrary _resourceLibrary;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="dbContext"></param>
    /// <param name="userLibrary"></param>
    /// <param name="resourceLibrary"></param>
    public AssetMediatorHandler(ILogger<AssetMediatorHandler> logger, IDbContext dbContext, IUserLibrary userLibrary, IResourceLibrary resourceLibrary)
    {
        _logger = logger;
        _dbContext = dbContext;
        _resourceLibrary = resourceLibrary;
        _userLibrary = userLibrary;
    }

    /// <summary>
    /// 애셋 조회
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<AssetResponse> Handle(GetAssetRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("AssetDoc GetDocAsync : {Assetid}", request.Assetid);
        var assetDoc = await _dbContext.GetDocAsync<AssetDoc>(Uuid.FromBase62(request.Assetid), false, cancellationToken);
        if (!assetDoc.Exists)
        {
            throw new ErrorNotfoundId(request.Assetid);
        }
        
        var response = assetDoc.To<AssetResponse>();
        response.Resource ??= new AssetResourceResponse();
        response.Resource.Manifest = PublicUrlHelper.GetAssetManifestUri(response.Id, response.Option.Version);
        response.Resource.Thumbnail = response.Resource.Thumbnail != null || response.Resource.Thumbnail != "" ? PublicUrlHelper.GetResourceImageUri(response.Resource.Thumbnail!) : PublicUrlHelper.GetAssetThumbnailtUri(response.Id, response.Option.Version);
        _logger.LogInformation("AssetResultResource : {result}, {version}", response, response.Option.Version);
        return response;
        
    }

    /// <summary>
    /// 애셋 리스트
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<CvAdminListResponse<AssetResponse>> Handle(GetAssetListRequest request, CancellationToken cancellationToken)
    {
        var filter = Builders<AssetResponse>.Filter.Eq("profileid", Uuid.FromBase62(request.Profileid).ToBsonBinaryData());
        if (request.NoneSale != null)
        {
            _logger.LogInformation("param.NoneSale : {NoneSale}", request.NoneSale);
            filter &= Builders<AssetResponse>.Filter.Eq("option.none_sale", request.NoneSale);
        }
        if (request.Category != null)
        {
            var cateArray = request.Category.Split(",");
            for (int i = 0; i < cateArray.Length; i++)
            {
                string? cate = cateArray[i];
                if (string.IsNullOrEmpty(cate)) throw new ErrorInvalidParam(cate);
            }
            _logger.LogInformation("param.Category : {Category}", new BsonArray() { request.Category });
            filter &= Builders<AssetResponse>.Filter.In("option.category", new BsonArray() { request.Category });
        }
        if (request.RedayStatus != null)
        {
            _logger.LogInformation("param.RedayStatus : {RedayStatus}", request.RedayStatus);
            filter &= Builders<AssetResponse>.Filter.Eq("option.ready_status", request.RedayStatus);
        }
        if (request.SaleStatus != null)
            {_logger.LogInformation("param.sale_status : {sale_status}", request.SaleStatus);
            filter &= Builders<AssetResponse>.Filter.Eq("option.sale_status", request.SaleStatus);
        }
        if (request.SaleReviewStatus != null)
        {
            _logger.LogInformation("param.SaleReviewStatus : {SaleReviewStatus}", request.SaleReviewStatus);
            filter &= Builders<AssetResponse>.Filter.Eq("option.sale_review_status", request.SaleReviewStatus);
        }
        if (request.JudgeStatus != null)
        {
            _logger.LogInformation("param.JudgeStatus : {JudgeStatus}", request.JudgeStatus);
            filter &= Builders<AssetResponse>.Filter.Eq("option.judge_status", request.JudgeStatus);
        }
        if (request.Blind != null)
        {
            _logger.LogInformation("param.Blind : {Blind}", request.Blind);
            filter &= Builders<AssetResponse>.Filter.Eq("option.blind", request.Blind);
        }
        if (request.Delete != null)
        {
            _logger.LogInformation("param.Delete : {Delete}", request.Delete);
            filter &= Builders<AssetResponse>.Filter.Eq("option.delete", request.Delete);
        }

        var options = new FindOptions<AssetResponse>
        {
            Skip = request.Page - 1,
            Limit = request.Limit
        };
        _logger.LogInformation("asset GetListAsync {param.Profileid}", request.Profileid);
        var assetList = await _dbContext.GetListAsync("asset", filter, options, cancellationToken);
        _logger.LogInformation("asset GetListCountAsync {param.Profileid}", request.Profileid);
        var totalCount = await _dbContext.GetListCountAsync("asset", Builders<AssetResponse>.Filter.Eq("profileid", Uuid.FromBase62(request.Profileid).ToBsonBinaryData()), null, cancellationToken);
        foreach (var asset in assetList)
        {
            asset.Resource ??= new AssetResourceResponse();
            asset.Resource.Manifest = PublicUrlHelper.GetAssetManifestUri(asset.Id, asset.Option.Version);
            asset.Resource.Thumbnail = asset.Resource.Thumbnail != null || asset.Resource.Thumbnail != "" ? PublicUrlHelper.GetResourceImageUri(asset.Resource.Thumbnail!) : PublicUrlHelper.GetAssetThumbnailtUri(asset.Id, asset.Option.Version);
        }
        var assetListCount = new CvAdminListCountResponse
        {
            Current = request.Page,
            Page = request.Page,
            Limit = request.Limit,
            Total = (int)totalCount
        };
        var response = new CvAdminListResponse<AssetResponse>(assetListCount, assetList);
        return response;
    }

    /// <summary>
    /// 애셋 수정
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<AssetResponse> Handle(UpdateAssetRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("AssetDoc GetDocAsync {param.Assetid}", request.Assetid);
        var assetDoc = await _dbContext.GetDocAsync<AssetDoc>(Uuid.FromBase62(request.Assetid), false, cancellationToken);
        if (!assetDoc.Exists)
        {
            throw new ErrorNotfoundId(request.Assetid);
        }
        
        BsonDocument? manifest = null;
        if (request.Option != null && assetDoc.GetInt32("option","version") != request.Option.Version) // 버전이 변동되면 revision 수정으로 감지
        {
            _logger.LogInformation("ResourceDoc GetDocAsync {param.Assetid}", request.Assetid);
            var resourceDoc = await _dbContext.GetDocAsync<ResourceDoc>(Uuid.FromBase62(request.Assetid), false, cancellationToken);
            if (resourceDoc.Exists)
            {
                var resourceDto = resourceDoc.To<ResourceDto>();
                if (resourceDto != null && resourceDto.Manifest != null)
                {
                    manifest = resourceDto.Manifest;
                    resourceDoc.UseReference();
                    _logger.LogInformation("ResourceDoc SaveAsync {param.Assetid}", request.Assetid);
                    _ = await resourceDoc.SaveAsync(cancellationToken);
                }
            }

            var assetRevisionDoc = AssetRevisionDoc.Create(Uuid.FromBase62(request.Assetid), request.Option.Version);
            _dbContext.Add(assetRevisionDoc);
            assetRevisionDoc.SetAssetRevision(1, manifest);
            _logger.LogInformation("AssetRevisionDoc SaveAsync {param.Assetid}", request.Assetid);
            await assetRevisionDoc.SaveAsync(cancellationToken);
        }

        assetDoc.UpdateAsset(request, manifest);
        
        // 리소스서버 섬네일 신규값 사용처리, 기존값 사용중지처리
        if (request.Resource != null && request.Resource.Thumbnail != null)
        {
            _logger.LogInformation("_resourceLibrary AddReference {Thumbnail}", request.Resource.Thumbnail);
            if (request.Resource.Thumbnail != "")
            {
                var thumbnailResult = await _resourceLibrary.AddReference(new AddReferenceParam(Uuid.FromBase62(request.Resource.Thumbnail)), cancellationToken);
                if (!thumbnailResult.Succeeded) throw new ErrorInternal(nameof(thumbnailResult));
            }
            if (assetDoc.GetNullableString("resource", "thumbnail") != null && assetDoc.GetNullableString("resource", "thumbnail") != "")
            {
                // 실패했을경우 어떤식으로 처리할지 고민필요 .. logging????
                _logger.LogInformation("_resourceLibrary RemoveReference {Thumbnail}", request.Resource.Thumbnail);
                _ = await _resourceLibrary.RemoveReference(new RemoveReferenceParam() { ResourceId = Uuid.FromBase62(assetDoc.GetString("resource", "thumbnail")) }, cancellationToken);
            }
        }

        _logger.LogInformation("assetDoc SaveAsync {param.Assetid}", request.Assetid);
        await assetDoc.SaveAsync(cancellationToken);

        // 마켓 수정
        _logger.LogInformation("MarketProductDoc GetDocAsync {param.Assetid}", request.Assetid);
        var marketProductDoc = await _dbContext.GetDocAsync<MarketProductDoc>(Uuid.FromBase62(request.Assetid), false, cancellationToken);
        if (marketProductDoc.Exists)
        {
            marketProductDoc.UpdateMarket(request, manifest);
            _logger.LogInformation("MarketProductDoc SaveAsync {param.Assetid}", request.Assetid);
            await marketProductDoc.SaveAsync(cancellationToken);
        }
        var response = assetDoc.To<AssetResponse>();
        response.Resource ??= new AssetResourceResponse();
        response.Resource.Manifest = PublicUrlHelper.GetAssetManifestUri(response.Id, response.Option.Version);
        response.Resource.Thumbnail = response.Resource.Thumbnail != null || response.Resource.Thumbnail != "" ? PublicUrlHelper.GetResourceImageUri(response.Resource.Thumbnail!) : PublicUrlHelper.GetAssetThumbnailtUri(response.Id, response.Option.Version);
        _logger.LogInformation("AssetResultResource : {result}, {version}", response, response.Option.Version);
        return response;
    }

    /// <summary>
    /// 애셋 승인
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<AssetResponse> Handle(SaleApprovalAssetRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("MarketProductDoc GetDocAsync {param.Assetid}", request.Assetid);
        var assetDoc = await _dbContext.GetDocAsync<AssetDoc>(Uuid.FromBase62(request.Assetid), false, cancellationToken);
        if (!assetDoc.Exists)
        {
            throw new ErrorNotfoundId(Uuid.FromBase62(request.Assetid));
        }
        
        _logger.LogInformation("MarketProductDoc GetDocAsync {param.Assetid}", Uuid.FromBase62(request.Assetid));
        var marketProductDoc = await _dbContext.GetDocAsync<MarketProductDoc>(Uuid.FromBase62(request.Assetid), false, cancellationToken); // 마켓에 등록되있는지 체크
        var saleAccept = DateTime.UtcNow;
        if (request.LaterReview) // 사후심사
        {
            //사후심사 승인
            assetDoc.SaleApprovalAsset();

            if (marketProductDoc.Exists)
            {
                //마켓 사후심사 승인
                marketProductDoc.SaleApprovalMarketProduct();
                _logger.LogInformation("MarketProductDoc SaveAsync {param.Assetid}", Uuid.FromBase62(request.Assetid));
                await marketProductDoc.SaveAsync(cancellationToken);
            }

            // 사후심사 승인되면 신고 count 초기화
            var reportResult = await _userLibrary.ReportReset(Uuid.FromBase62(request.Assetid), cancellationToken);
            if (!reportResult.Succeeded)
            {
                throw new ErrorBadRequest("repory reset Fail");
            }

        }
        else
        {
            var saleStatus = 2;
            if (assetDoc.GetNullableBoolean("option","instant_sale") != null && assetDoc.GetNullableBoolean("option", "instant_sale") == true)
            {
                saleStatus = 3;
            }

            // asset_revision 수정 status 1 
            var revisionFilter = Builders<BsonDocument>.Filter.Eq("assetid", assetDoc.GetUuid("_id").ToBsonBinaryData());
            revisionFilter &= Builders<BsonDocument>.Filter.Eq("version", assetDoc.GetInt32("option", "version"));
            _logger.LogInformation("AssetRevisionDoc GetDocAsync {param.Assetid}", Uuid.FromBase62(request.Assetid));
            var findRevisionDoc = await _dbContext.GetDocAsync<AssetRevisionDoc>(revisionFilter, false, cancellationToken);
            findRevisionDoc.SaleApprovalRevision();
            _logger.LogInformation("AssetRevisionDoc SaveAsync {param.Assetid}", Uuid.FromBase62(request.Assetid));
            await findRevisionDoc.SaveAsync(cancellationToken);

            var assetRevision = findRevisionDoc.To<AssetRevisionResult>();
            // 승인 ( revision manifest 넣어주기)
            assetDoc.SaleApprovalAsset(request, saleAccept, saleStatus, assetDoc.GetInt32("option", "version"), assetRevision);

            // market product 등록 즉시판매인지 아닌지 판단해서 판매허용 상태또는 판매중으로 등록
            marketProductDoc = MarketProductDoc.Create(Uuid.FromBase62(request.Assetid));
            _dbContext.Add(marketProductDoc);
            marketProductDoc.SaleApprovalMarketProduct(assetDoc, saleAccept, saleStatus);
            _logger.LogInformation("MarketProductDoc SaveAsync {param.Assetid}", Uuid.FromBase62(request.Assetid));
            await marketProductDoc.SaveAsync(cancellationToken);
        }

        _logger.LogInformation("assetDoc SaveAsync {param.Assetid}", request.Assetid);
        await assetDoc.SaveAsync(cancellationToken);
        
        var response = assetDoc.To<AssetResponse>();
        response.Resource ??= new AssetResourceResponse();
        response.Resource.Manifest = PublicUrlHelper.GetAssetManifestUri(response.Id, response.Option.Version);
        response.Resource.Thumbnail = response.Resource.Thumbnail != null || response.Resource.Thumbnail != "" ? PublicUrlHelper.GetResourceImageUri(response.Resource.Thumbnail!) : PublicUrlHelper.GetAssetThumbnailtUri(response.Id, response.Option.Version);
        _logger.LogInformation("AssetResultResource : {result}, {version}", response, response.Option.Version);
        return response;
    }

    /// <summary>
    /// 애셋 반려
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<AssetResponse?> Handle(SaleRejectionAssetRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("AssetDoc GetDocAsync {param.Assetid}", request.Assetid);
        var assetDoc = await _dbContext.GetDocAsync<AssetDoc>(Uuid.FromBase62(request.Assetid), false, cancellationToken);
        if (!assetDoc.Exists)
        {
            throw new ErrorNotfoundId(request.Assetid);
        }
        
        if (!request.LaterReview) //사후심사가아닐경우 판매수수료 환불
        {
            // 애셋 삭제 collection 에 넣어주기
            var assetDeleteDoc = AssetDeleteDoc.Create(Uuid.FromBase62(request.Assetid));
            _dbContext.Add(assetDeleteDoc);
            assetDeleteDoc.SetDelete(assetDoc);
            _logger.LogInformation("AssetDoc SaveAsync {param.Assetid}", request.Assetid);
            await assetDeleteDoc.SaveAsync(cancellationToken);

            // 애셋 collection에서는 즉시삭제
            _logger.LogInformation("AssetDoc DeleteAsync {param.Assetid}", request.Assetid);
            await assetDoc.DeleteAsync(true, cancellationToken);

            // 판매수수료 환불

            return null;
        }
        else
        {
            // 사후심사반려
            assetDoc.SaleRejectionAsset();
            _logger.LogInformation("AssetDoc SaveAsync {param.Assetid}", request.Assetid);
            await assetDoc.SaveAsync(cancellationToken);
            
            //애셋 삭제요청 collection 추가
            var assetDeleteRequestDoc = AssetDeleteRequestDoc.Create(Uuid.FromBase62(request.Assetid));
            _dbContext.Add(assetDeleteRequestDoc);
            _logger.LogInformation("assetDeleteRequestDoc SaveAsync {param.Assetid}", request.Assetid);
            await assetDeleteRequestDoc.SaveAsync(cancellationToken);

            var response = assetDoc.To<AssetResponse>();
            response.Resource ??= new AssetResourceResponse();
            response.Resource.Manifest = PublicUrlHelper.GetAssetManifestUri(response.Id, response.Option.Version);
            response.Resource.Thumbnail = response.Resource.Thumbnail != null || response.Resource.Thumbnail != "" ? PublicUrlHelper.GetResourceImageUri(response.Resource.Thumbnail!) : PublicUrlHelper.GetAssetThumbnailtUri(response.Id, response.Option.Version);
            _logger.LogInformation("AssetResultResource : {result}, {version}", response, response.Option.Version);
            return response;
        }
    }

    /// <summary>
    /// 애셋 삭제요청
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<AssetResponse> Handle(DeleteAssetRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("AssetDoc GetDocAsync {param.Assetid}", request.Assetid);
        var assetDoc = await _dbContext.GetDocAsync<AssetDoc>(Uuid.FromBase62(request.Assetid), false, cancellationToken);
        if (!assetDoc.Exists)
        {
            throw new ErrorNotfoundId(request.Assetid);
        }

        //애셋 상태값 변경 blind, delete = true
        assetDoc.DeleteAsset();
        _logger.LogInformation("AssetDoc SaveAsync {param.Assetid}", request.Assetid);
        await assetDoc.SaveAsync(cancellationToken);
        


        //마켓 상태값 변경 blind, delete = true
        _logger.LogInformation("MarketProductDoc GetDocAsync {param.Assetid}", request.Assetid);
        var marketProductDoc = await _dbContext.GetDocAsync<MarketProductDoc>(Uuid.FromBase62(request.Assetid), false, cancellationToken); // 마켓에 등록되있는지 체크
        if (marketProductDoc.Exists)
        {
            marketProductDoc.DeleteMarketProduct();
            _logger.LogInformation("MarketProductDoc SaveAsync {param.Assetid}", request.Assetid);
            await marketProductDoc.SaveAsync(cancellationToken);
        }

        //애셋 삭제요청 collection 추가
        var assetDeleteRequestDoc = AssetDeleteRequestDoc.Create(Uuid.FromBase62(request.Assetid));
        _dbContext.Add(assetDeleteRequestDoc);
        _logger.LogInformation("AssetDeleteRequestDoc SaveAsync {param.Assetid}", request.Assetid);
        await assetDeleteRequestDoc.SaveAsync(cancellationToken);

        var response = assetDoc.To<AssetResponse>();
        response.Resource ??= new AssetResourceResponse();
        response.Resource.Manifest = PublicUrlHelper.GetAssetManifestUri(response.Id, response.Option.Version);
        response.Resource.Thumbnail = response.Resource.Thumbnail != null || response.Resource.Thumbnail != "" ? PublicUrlHelper.GetResourceImageUri(response.Resource.Thumbnail!) : PublicUrlHelper.GetAssetThumbnailtUri(response.Id, response.Option.Version);
        _logger.LogInformation("AssetResultResource : {result}, {version}", response, response.Option.Version);
        return response;
    }

    /// <summary>
    /// 애셋 삭제요청 취소
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<AssetResponse> Handle(DeleteCancelAssetRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("AssetDoc GetDocAsync {param.Assetid}", request.Assetid);
        var assetDoc = await _dbContext.GetDocAsync<AssetDoc>(Uuid.FromBase62(request.Assetid), false, cancellationToken);
        if (!assetDoc.Exists)
        {
            throw new ErrorNotfoundId(request.Assetid);
        }

        //애셋 상태값 변경 blind, delete = false
        assetDoc.DeleteCancelAsset();
        _logger.LogInformation("AssetDoc SaveAsync {param.Assetid}", request.Assetid);
        await assetDoc.SaveAsync(cancellationToken);
        

        //마켓 상태값 변경 blind, delete = false
        _logger.LogInformation("MarketProductDoc GetDocAsync {param.Assetid}", request.Assetid);
        var marketProductDoc = await _dbContext.GetDocAsync<MarketProductDoc>(Uuid.FromBase62(request.Assetid), false, cancellationToken); // 마켓에 등록되있는지 체크
        if (marketProductDoc.Exists)
        {
            marketProductDoc.DeleteCancelMarketProduct();
            _logger.LogInformation("MarketProductDoc SaveAsync {param.Assetid}", request.Assetid);
            await marketProductDoc.SaveAsync(cancellationToken);
        }

        //애셋 삭제요청 collection 삭제
        _logger.LogInformation("AssetDeleteRequestDoc GetDocAsync {param.Assetid}", request.Assetid);
        var assetDeleteRequestDoc = await _dbContext.GetDocAsync<AssetDeleteRequestDoc>(Uuid.FromBase62(request.Assetid), false, cancellationToken);
        if (assetDeleteRequestDoc.Exists)
        {
            _logger.LogInformation("assetDeleteRequestDoc DeleteAsync {param.Assetid}", request.Assetid);
            await assetDeleteRequestDoc.DeleteAsync(true, cancellationToken);
        }

        var response = assetDoc.To<AssetResponse>();
        response.Resource ??= new AssetResourceResponse();
        response.Resource.Manifest = PublicUrlHelper.GetAssetManifestUri(response.Id, response.Option.Version);
        response.Resource.Thumbnail = response.Resource.Thumbnail != null || response.Resource.Thumbnail != "" ? PublicUrlHelper.GetResourceImageUri(response.Resource.Thumbnail!) : PublicUrlHelper.GetAssetThumbnailtUri(response.Id, response.Option.Version);
        _logger.LogInformation("AssetResultResource : {result}, {version}", response, response.Option.Version);
        return response;
    }
}
