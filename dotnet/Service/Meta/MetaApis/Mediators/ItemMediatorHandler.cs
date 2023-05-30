using Colorverse.Application.Mediator;
using Colorverse.Application.Session;
using Colorverse.Common.DataTypes;
using Colorverse.Common.Errors;
using Colorverse.Common.Exceptions;
using Colorverse.Common.Helper;
using Colorverse.Database;
using Colorverse.Meta.Apis.DataTypes.Item;
using Colorverse.Meta.DataTypes.Resource;
using Colorverse.Meta.Documents;
using Colorverse.Meta.Domains;
using Colorverse.ResourceLibrary;
using Colorverse.UserLibrary;
using CvFramework.Common;
using CvFramework.MongoDB.Extensions;
using MediatR;
using MongoDB.Bson;
using MongoDB.Driver;
using ResourceLibrary.Nats.V1.DataTypes;

namespace Colorverse.Meta.Mediators;

/// <summary>
/// 아이템 Mediator
/// </summary>
[MediatorHandler]
public class ItemMediatorHandler : MediatorHandlerBase,
    IMediatorHandler<GetItemRequest, ItemResponse>,
    IMediatorHandler<GetItemListRequest, CvListResponse<ItemResponse>>,
    IMediatorHandler<CreateItemRequest, ItemResponse>,
    IMediatorHandler<UpdateItemRequest, ItemResponse>,
    IMediatorHandler<SaleInspectionItemRequest, ItemResponse>,
    IMediatorHandler<SaleStartItemRequest, ItemResponse>,
    IMediatorHandler<SaleStopItemRequest, ItemResponse>,
    IMediatorHandler<CreateItemTemplateRequest, ItemTemplateResponse>,
    IMediatorHandler<GetItemTemplateListRequest, CvListResponse<ItemTemplateResponse>>,
    IMediatorHandler<DeleteItemTemplateRequest, string>
{
    private readonly IDbContext _dbContext;
    private readonly IContextUserProfile _profile;
    private readonly IMediator _mediator;
    private readonly IUserLibrary _userLibrary;
    private readonly IResourceLibrary _resourceLibrary;

    /// <summary>
    /// 아이템 Mediator 설정
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="profile"></param>
    /// <param name="mediator"></param>
    /// <param name="userLibrary"></param>
    /// <param name="resourceLibrary"></param>
    public ItemMediatorHandler(IDbContext dbContext, IContextUserProfile profile, IMediator mediator, IUserLibrary userLibrary, IResourceLibrary resourceLibrary)
    {
        _dbContext = dbContext;
        _profile = profile;
        _mediator = mediator;
        _userLibrary = userLibrary;
        _resourceLibrary = resourceLibrary;
    }

    /// <summary>
    /// 아이템 조회
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ErrorNotfoundId"></exception>
    public async Task<ItemResponse> Handle(GetItemRequest request, CancellationToken cancellationToken)
    {
        var itemDoc = await _dbContext.GetDocAsync<ItemDoc>(Uuid.FromBase62(request.Itemid), cancellationToken);
        if (!itemDoc.Exists)
        {
            throw new ErrorNotfoundId(request.Itemid);
        }
        
        if(request.MeCheck && itemDoc.GetUuid("profileid") != _profile.ProfileId)
        {
            throw new ErrorBadRequest("Not matching Profileid " + _profile.ProfileId);
        }
        if (!request.MeCheck && itemDoc.GetInt32("option","sale_status") != 3)
        {
            throw new ErrorNotfoundId(request.Itemid);
        }

        var response = itemDoc.To<ItemResponse>();
        response.Resource ??= new ItemResourceResponse();
        response.Resource.Manifest = PublicUrlHelper.GetItemManifestUri(response.Id, response.Option.Version);
        response.Resource.Preview = response.Resource.Preview != null ? PublicUrlHelper.GetResourceImageUri(response.Resource.Preview) : null;
        response.Resource.Thumbnail = response.Resource.Thumbnail != null || response.Resource.Thumbnail != "" ? PublicUrlHelper.GetResourceImageUri(response.Resource.Thumbnail!) : PublicUrlHelper.GetItemThumbnailtUri(response.Id, response.Option.Version);
        return response;
    }

    /// <summary>
    /// 아이템 목록조회
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ErrorInvalidParam"></exception>
    public async Task<CvListResponse<ItemResponse>> Handle(GetItemListRequest request, CancellationToken cancellationToken)
    {
        var filter = Builders<ItemResponse>.Filter.Eq("profileid", Uuid.FromBase62(request.Profileid).ToBsonBinaryData());
        if (!request.MeCheck)
        {
            filter &= Builders<ItemResponse>.Filter.Eq("option.sale_status", 3);
        }
        if (request.NoneSale != null)
        {
            filter &= Builders<ItemResponse>.Filter.Eq("option.none_sale" , request.NoneSale);
        }
        if (request.Category != null)
        {
            filter &= Builders<ItemResponse>.Filter.In("option.category", new BsonArray() { request.Category });
        }

        var options = new FindOptions<ItemResponse>
        {
            Skip = request.Page - 1,
            Limit = request.Limit
        };

        var itemList = await _dbContext.GetListAsync("item", filter, options, cancellationToken);
        foreach (var item in itemList)
        {
            item.Resource ??= new ItemResourceResponse();
            item.Resource.Manifest = PublicUrlHelper.GetItemManifestUri(item.Id, item.Option.Version);
            item.Resource.Preview = item.Resource.Preview != null ? PublicUrlHelper.GetResourceImageUri(item.Resource.Preview) : null;
            item.Resource.Thumbnail = item.Resource.Thumbnail != null || item.Resource.Thumbnail != "" ? PublicUrlHelper.GetResourceImageUri(item.Resource.Thumbnail!) : PublicUrlHelper.GetItemThumbnailtUri(item.Id, item.Option.Version);
        }
        var Count = new CvListCountResponse
        {
            Current = request.Page,
            Page = request.Page,
            Limit = request.Limit,
        };
        var response = new CvListResponse<ItemResponse>(Count, itemList);
        return response;
    }

    /// <summary>
    /// 아이템 생성
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<ItemResponse> Handle(CreateItemRequest request, CancellationToken cancellationToken)
    {
        ItemDoc itemDoc;
        ItemRevisionDoc itemRevisionDoc;

        //아이템이 존재하는지 체크
        var itemUuid = Uuid.FromBase62(request.Itemid);
        var finditemDoc = await _dbContext.GetDocAsync<ItemDoc>(itemUuid, cancellationToken);
        if (finditemDoc.Exists)
        {
            throw new ErrorBadRequest("Duplicate itemid " + nameof(request.Itemid));
        }

        itemDoc = ItemDoc.Create(itemUuid);
        _dbContext.Add(itemDoc);

        itemDoc.ValidationCheck(request, _profile.IsAdmin);

        itemRevisionDoc = ItemRevisionDoc.Create(itemUuid, request.Option.Version);
        _dbContext.Add(itemRevisionDoc);
        
        // resource db 에서 version 및 type 정상적인 값인지 한번더 검증필요
        var resourceDto = await _mediator.Send(new GetResourceParam(itemUuid, request.Option.Version), cancellationToken);
        if (resourceDto != null && resourceDto.Manifest != null)
        {
            var version = resourceDto.Version;
            var type = resourceDto.Manifest.GetTreeValue("main", "type").AsString;

            if (request.Option.Version != version)
            {
                throw new ErrorInvalidParam("Not Matching Version " + nameof(request.Option.Version));
            }
            if (!type.Equals("Item"))
            {
                throw new ErrorInvalidParam("Not Matching Type " + nameof(request.Itemid));
            }
            // manifest 정보를 itemrevisioncollection에 저장
            var manifest = resourceDto.Manifest;
            itemRevisionDoc.SetItemRevision(0, manifest);
        }
        else
        {
            itemRevisionDoc.SetItemRevision(0);
        }

        // 리소스서버 섬네일, 프리뷰 신규값 사용처리
        if (request.Resource.Thumbnail != null)
        {
            var thumbnailResult = await _resourceLibrary.AddReference(new AddReferenceParam(Uuid.FromBase62(request.Resource.Thumbnail)), cancellationToken);
            if(!thumbnailResult.Succeeded) throw new ErrorInternal(nameof(thumbnailResult));
        }
        if(request.Resource.Preview != null)
        {
            var previewResult = await _resourceLibrary.AddReference(new AddReferenceParam(Uuid.FromBase62(request.Resource.Preview)), cancellationToken);
            if (!previewResult.Succeeded) throw new ErrorInternal(nameof(previewResult));
        }

        // 리소스 사용 ( 추후 리소스서버로 이동시 library로 변경)
        var useResouceResult = await _mediator.Send(new UseResourceParam(itemUuid, request.Option.Version), cancellationToken);
        if (useResouceResult == null) throw new ErrorInternal(nameof(useResouceResult));

        itemDoc.SetTemplateItem(request, _profile.IsAdmin);
        itemDoc.SetItem(request, _profile.ProfileId, _profile.WorldId, _profile.UserId);

        await itemRevisionDoc.SaveAsync(cancellationToken); // itemrevision create
        await itemDoc.SaveAsync(cancellationToken); // item create

        var response = itemDoc.To<ItemResponse>();
        response.Resource ??= new ItemResourceResponse();
        response.Resource.Manifest = PublicUrlHelper.GetItemManifestUri(response.Id, response.Option.Version);
        response.Resource.Preview = response.Resource.Preview != null ? PublicUrlHelper.GetResourceImageUri(response.Resource.Preview) : null;
        response.Resource.Thumbnail = response.Resource.Thumbnail != null || response.Resource.Thumbnail != "" ? PublicUrlHelper.GetResourceImageUri(response.Resource.Thumbnail!) : PublicUrlHelper.GetItemThumbnailtUri(response.Id, response.Option.Version);
        return response;
    }

    /// <summary>
    /// 아이템 수정
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<ItemResponse> Handle(UpdateItemRequest request, CancellationToken cancellationToken)
    {
        ItemDoc itemDoc;
        ItemRevisionDoc itemRevisionDoc;
        var itemUuid = Uuid.FromBase62(request.Itemid);
        // 아이템이 존재하는지 체크
        itemDoc = await _dbContext.GetDocAsync<ItemDoc>(itemUuid, cancellationToken);
        if (!itemDoc.Exists)
        {
            throw new ErrorNotfoundId(request.Itemid);
        }
        

        itemDoc.ValidationCheck(request, itemDoc, _profile.ProfileId, _profile.IsAdmin); // version validation

        
        if (itemDoc.GetNullableBsonDocument("option") != null && request.Option != null && request.Option.Version > itemDoc.GetInt32("option", "version")) // 요청버전이 기존버전보다 클경우 resource 수정으로 판단
        {
            itemRevisionDoc = ItemRevisionDoc.Create(itemUuid, request.Option!.Version);
            _dbContext.Add(itemRevisionDoc);

            // resource db 에서 version 및 type 정상적인 값인지 한번더 검증필요
            var resourceDto = await _mediator.Send(new GetResourceParam(itemUuid, request.Option.Version), cancellationToken);
            if (resourceDto != null && resourceDto.Manifest != null)
            {
                var version = resourceDto.Version;
                var type = resourceDto.Manifest.GetTreeValue("main", "type").AsString;

                if (request.Option.Version != version)
                {
                    throw new ErrorInvalidParam("Not Matching Version " + nameof(request.Option.Version));
                }
                if (!type.Equals("Item"))
                {
                    throw new ErrorInvalidParam("Not Matching Type " + nameof(request.Itemid));
                }

                // manifest 정보를 itemrevisioncollection에 저장
                var manifest = resourceDto.Manifest;
                itemRevisionDoc.SetItemRevision(0, manifest);
            }
            else
            {
                itemRevisionDoc.SetItemRevision(0);
            }

            // 리소스 사용 ( 추후 리소스서버로 이동시 library로 변경)
            var useResouceResult = await _mediator.Send(new UseResourceParam(itemUuid, request.Option.Version), cancellationToken);
            if (useResouceResult == null) throw new ErrorInternal(nameof(useResouceResult));

            await itemRevisionDoc.SaveAsync(cancellationToken); // itemrevision create

        }

        //가격수정이 일어날시 차감이벤트 진행
        if(request.Option != null && request.Option.Price != null)
        {
            var amount = request.Option.Price.Amount;
            
            if (itemDoc.GetNullableBsonDocument("option") != null && itemDoc.GetNullableBsonDocument("option", "price") != null && amount > itemDoc.GetInt32("option", "price", "amount"))
            {
                //var payAmount = amount - finditem.Option.Price.Amount; //수수료 금액
            }

        }

        itemDoc.UpdateItem(request);

        // 리소스서버 섬네일 신규값 사용처리, 기존값 사용중지처리
        if (request.Resource != null)
        {
            if (request.Resource.Thumbnail != null)
            {
                if (request.Resource.Thumbnail != "")
                {
                    var thumbnailResult = await _resourceLibrary.AddReference(new AddReferenceParam(Uuid.FromBase62(request.Resource.Thumbnail)), cancellationToken);
                    if (!thumbnailResult.Succeeded) throw new ErrorInternal(nameof(thumbnailResult));
                }
                if (itemDoc.GetNullableString("resource","thumbnail") != null && itemDoc.GetNullableString("resource", "thumbnail") != "")
                {
                    // 실패했을경우 어떤식으로 처리할지 고민필요 .. logging????
                    _ = await _resourceLibrary.RemoveReference(new RemoveReferenceParam() { ResourceId = Uuid.FromBase62(itemDoc.GetString("resource","thumbnail")) }, cancellationToken);
                }
            }
            if (request.Resource.Preview != null)
            {
                var previewResult = await _resourceLibrary.AddReference(new AddReferenceParam(Uuid.FromBase62(request.Resource.Preview)), cancellationToken);
                if (!previewResult.Succeeded) throw new ErrorInternal(nameof(previewResult));
                if (itemDoc.GetNullableString("resource","preview") != null)
                {
                    // 실패했을경우 어떤식으로 처리할지 고민필요 .. logging????
                    _ = await _resourceLibrary.RemoveReference(new RemoveReferenceParam() { ResourceId = Uuid.FromBase62(itemDoc.GetString("resource","preview")) }, cancellationToken);
                }
            }
        }

        await itemDoc.SaveAsync(cancellationToken);

        if (itemDoc.GetNullableBsonDocument("option") != null && itemDoc.GetNullableBoolean("option","template") != null && itemDoc.GetNullableBoolean("option", "template") == true && _profile.IsAdmin) // template 을 건드릴수있는 어드민일경우
        {
            ItemTemplateDoc templateDoc = await _dbContext.GetDocAsync<ItemTemplateDoc>(request.Itemid, cancellationToken); // item template 존재할시 데이터 수정
            if (templateDoc.Exists)
            {
                templateDoc.UpdateTemplate(request);
                await templateDoc.SaveAsync(cancellationToken);
            }

        }

        // 마켓 수정
        var marketProductDoc = await _dbContext.GetDocAsync<MarketProductDoc>(itemUuid, cancellationToken);
        if (marketProductDoc.Exists)
        {
            // 판매중일시 부가설명, 해시태그 , 가격 수정이가능하다. (쿨타임)
            marketProductDoc.UpdatePriceMarket(request);
            await marketProductDoc.SaveAsync(cancellationToken);
        }
        var response = itemDoc.To<ItemResponse>();
        response.Resource ??= new ItemResourceResponse();
        response.Resource.Manifest = PublicUrlHelper.GetItemManifestUri(response.Id, response.Option.Version);
        response.Resource.Preview = response.Resource.Preview != null ? PublicUrlHelper.GetResourceImageUri(response.Resource.Preview) : null;
        response.Resource.Thumbnail = response.Resource.Thumbnail != null || response.Resource.Thumbnail != "" ? PublicUrlHelper.GetResourceImageUri(response.Resource.Thumbnail!) : PublicUrlHelper.GetItemThumbnailtUri(response.Id, response.Option.Version);
        return response;
    }

    /// <summary>
    /// 아이템 판매심사 요청
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<ItemResponse> Handle(SaleInspectionItemRequest request, CancellationToken cancellationToken)
    {
        var itemUuid = Uuid.FromBase62(request.Itemid);
        var itemDoc = await _dbContext.GetDocAsync<ItemDoc>(itemUuid, cancellationToken); // 아이템 판매요청시 판매할 아이템있는지 체크
        if (!itemDoc.Exists)
        {
            throw new ErrorNotfoundId(request.Itemid);
        }
        
        itemDoc.ValidationCheck(request, itemDoc, _profile.ProfileId, _profile.IsAdmin);

        var totalPrice = new Dictionary<int, int>
        {
            { request.Option.Price!.Type, request.Option.Price!.Amount } //요청한 아이템 타입 및 가격
        };

        var revisionFilter = Builders<BsonDocument>.Filter.Eq("itemid", itemDoc.GetUuid("_id").ToBsonBinaryData);
        revisionFilter &= Builders<BsonDocument>.Filter.Eq("version", itemDoc.GetInt32("option", "version"));
        var findRevisionDoc = await _dbContext.GetDocAsync<ItemRevisionDoc>(revisionFilter, cancellationToken);
        if(!findRevisionDoc.Exists) throw new ErrorBadRequest("Not Found Item Revision "+ request.Itemid);
        var findRevision = findRevisionDoc.To<ItemRevision>();
        if (findRevision.Manifest != null && findRevision.Manifest.TryGetValue("subItemIds", out _)) //subitemids 가 존재하지 않을수도 있음
        {
            // subitemids 가격
            var subitemids = findRevision.Manifest.GetValue("subItemIds").AsBsonArray.Select(p => p.AsString).ToArray();
            foreach (var subitemid in subitemids)
            {
                var assetDoc = await _dbContext.GetDocAsync<AssetDoc>(Uuid.FromBase62(subitemid), cancellationToken);
                if (!assetDoc.Exists) throw new ErrorBadRequest("Not Found SubItemid " + subitemid);
                var subAmount = totalPrice[assetDoc.GetInt32("option", "price", "type")];
                subAmount += assetDoc.GetInt32("option", "price", "amount");
                totalPrice.Add(assetDoc.GetInt32("option", "price", "type"), subAmount);
            }

        }
        // 판매수수료 = 아이템의 최종가
        request.Option.Price.Amount = totalPrice[request.Option.Price.Type];


        // 판매 수수료 부과 ( 성공시 다음 로직 진행)  = 요청한아이템가격(공임비) + subitemids 가격 totalPrice(type, amount)  ( 나중엔 카테고리별 퍼센테이지 )



        // 현재 판매심사요청하면 자동으로 판매승인을 내게처리 (판매일정이있으면 판매중으로, 일정이없으면 판매허용으로)
        // 나중에 백오피스 완료되면 SaleInspectionItem 으로 사용
        itemDoc.SaleInspectionItem2(request);
        await itemDoc.SaveAsync(cancellationToken);
        
        // /현재 판매심사요청하면 자동으로 판매승인을 내게처리 (판매일정이있으면 판매중으로, 일정이없으면 판매허용으로)


        // 유저 아이템 생성 ( 판매심사 반려시 유저아이템 삭제)
        var userItemDoc = UserItemDoc.Create(itemUuid, _profile.ProfileId);
        _dbContext.Add(userItemDoc);
        userItemDoc.SetUserItem(_profile.UserId, _profile.WorldId, itemDoc.GetBsonArray("option", "category")); // 추후 _profile에 townid 생성시 추가
        userItemDoc.SetQuantity(1);
        await userItemDoc.SaveAsync(cancellationToken); // create

        // 유저아이템 카운트 동기화
        _ = _userLibrary.UpdateItemCount(_profile.ProfileId, 1, cancellationToken);


        // 마켓 처리는 판매심사요청 자동 판매승인때문에 처리 나중엔 제거
        var marketProductDoc = await _dbContext.GetDocAsync<MarketProductDoc>(itemUuid, cancellationToken); // 마켓에 등록되있는지 체크
        if (marketProductDoc.Exists)
        {
            marketProductDoc.UpdateItemMarketProduct();
            await marketProductDoc.SaveAsync(cancellationToken); // 기존에 데이터를 판매중으로 변경
        }
        else
        {
            marketProductDoc = MarketProductDoc.Create(itemUuid);
            _dbContext.Add(marketProductDoc);
            marketProductDoc.SetItemMarketProduct(itemDoc);
            marketProductDoc.SetItems(1, itemUuid, 1); // 타입, 아이디, 수량
            await marketProductDoc.SaveAsync(cancellationToken); // 마켓 등록
        }
        // / 마켓 처리는 판매심사요청 자동 판매승인때문에 처리 나중엔 제거


        var response = itemDoc.To<ItemResponse>();
        response.Resource ??= new ItemResourceResponse();
        response.Resource.Manifest = PublicUrlHelper.GetItemManifestUri(response.Id, response.Option.Version);
        response.Resource.Preview = response.Resource.Preview != null ? PublicUrlHelper.GetResourceImageUri(response.Resource.Preview) : null;
        response.Resource.Thumbnail = response.Resource.Thumbnail != null || response.Resource.Thumbnail != "" ? PublicUrlHelper.GetResourceImageUri(response.Resource.Thumbnail!) : PublicUrlHelper.GetItemThumbnailtUri(response.Id, response.Option.Version);
        return response;
    }

    /// <summary>
    /// 아이템 판매
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ErrorNotfoundId"></exception>
    public async Task<ItemResponse> Handle(SaleStartItemRequest request, CancellationToken cancellationToken)
    {
        var itemUuid = Uuid.FromBase62(request.Itemid);
        var itemDoc = await _dbContext.GetDocAsync<ItemDoc>(itemUuid, cancellationToken); // 아이템 판매요청시 판매할 아이템있는지 체크
        if (!itemDoc.Exists)
        {
            throw new ErrorNotfoundId(request.Itemid);
        }
        itemDoc.ValidationCheck(request, itemDoc, _profile.ProfileId);
        itemDoc.SaleStartItem();
        await itemDoc.SaveAsync(cancellationToken); // 판매중 변경
        
        var marketProductDoc = await _dbContext.GetDocAsync<MarketProductDoc>(itemUuid, cancellationToken); // 마켓에 등록되있는지 체크
        if (marketProductDoc.Exists)
        {
            marketProductDoc.UpdateItemMarketProduct();
            await marketProductDoc.SaveAsync(cancellationToken); // 기존에 데이터를 판매중으로 변경
        }
        else
        {
            marketProductDoc = MarketProductDoc.Create(itemUuid);
            _dbContext.Add(marketProductDoc);
            marketProductDoc.SetItemMarketProduct(itemDoc);
            marketProductDoc.SetItems(1, itemUuid, 1); // 타입, 아이디, 수량
            await marketProductDoc.SaveAsync(cancellationToken); // 마켓 등록
        }

        var response = itemDoc.To<ItemResponse>();
        response.Resource ??= new ItemResourceResponse();
        response.Resource.Manifest = PublicUrlHelper.GetItemManifestUri(response.Id, response.Option.Version);
        response.Resource.Preview = response.Resource.Preview != null ? PublicUrlHelper.GetResourceImageUri(response.Resource.Preview) : null;
        response.Resource.Thumbnail = response.Resource.Thumbnail != null || response.Resource.Thumbnail != "" ? PublicUrlHelper.GetResourceImageUri(response.Resource.Thumbnail!) : PublicUrlHelper.GetItemThumbnailtUri(response.Id, response.Option.Version);
        return response;
    }

    /// <summary>
    /// 아이템 판매중단
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<ItemResponse> Handle(SaleStopItemRequest request, CancellationToken cancellationToken)
    {
        var itemUuid = Uuid.FromBase62(request.Itemid);
        var itemDoc = await _dbContext.GetDocAsync<ItemDoc>(itemUuid, cancellationToken); // 아이템있는지 체크
        if (!itemDoc.Exists)
        {
            throw new ErrorNotfoundId(request.Itemid);
        }
        
        itemDoc.ValidationCheck(request, itemDoc, _profile.ProfileId);
        itemDoc.SaleCancelItem();
        await itemDoc.SaveAsync(cancellationToken); // 판매중지
        
        var marketProductDoc = await _dbContext.GetDocAsync<MarketProductDoc>(itemUuid, cancellationToken); // 마켓에 등록되있는지 체크
        if (marketProductDoc.Exists)
        {
            marketProductDoc.SaleStopStatus();
            await marketProductDoc.SaveAsync(cancellationToken); // 판매중지
        }

        var response = itemDoc.To<ItemResponse>();
        response.Resource ??= new ItemResourceResponse();
        response.Resource.Manifest = PublicUrlHelper.GetItemManifestUri(response.Id, response.Option.Version);
        response.Resource.Preview = response.Resource.Preview != null ? PublicUrlHelper.GetResourceImageUri(response.Resource.Preview) : null;
        response.Resource.Thumbnail = response.Resource.Thumbnail != null || response.Resource.Thumbnail != "" ? PublicUrlHelper.GetResourceImageUri(response.Resource.Thumbnail!) : PublicUrlHelper.GetItemThumbnailtUri(response.Id, response.Option.Version);
        return response;
    }

    /// <summary>
    /// 아이템 템플릿 생성
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ErrorNotfoundId"></exception>
    /// <exception cref="ErrorBadRequest"></exception>
    public async Task<ItemTemplateResponse> Handle(CreateItemTemplateRequest request, CancellationToken cancellationToken)
    {
        var itemUuid = Uuid.FromBase62(request.Itemid);
        var itemDoc = await _dbContext.GetDocAsync<ItemDoc>(itemUuid, cancellationToken); // 아이템있는지 체크
        if (!itemDoc.Exists)
        {
            throw new ErrorNotfoundId(request.Itemid);
        }
        itemDoc.ValidationCheck(request, itemDoc, _profile.ProfileId, _profile.IsAdmin);

        var itemTemplateDoc = await _dbContext.GetDocAsync<ItemTemplateDoc>(itemUuid, cancellationToken);
        if (itemTemplateDoc.Exists)
        {
            throw new ErrorBadRequest("Duplicate template itemid " + request.Itemid);
        }

        itemTemplateDoc = ItemTemplateDoc.Create(itemUuid);
        _dbContext.Add(itemTemplateDoc);
        itemTemplateDoc.SetTemplate(itemDoc, _profile.ProfileId, _profile.WorldId, _profile.UserId, _profile.UserId);
        await itemTemplateDoc.SaveAsync(cancellationToken);

        var response = itemTemplateDoc.To<ItemTemplateResponse>();
        response.Resource ??= new ItemTemplateResourceResponse();
        response.Resource.Manifest = PublicUrlHelper.GetItemManifestUri(response.Id, response.Option.Version);
        response.Resource.Preview = response.Resource.Preview != null ? PublicUrlHelper.GetResourceImageUri(response.Resource.Preview) : null;
        response.Resource.Thumbnail = response.Resource.Thumbnail != null || response.Resource.Thumbnail != "" ? PublicUrlHelper.GetResourceImageUri(response.Resource.Thumbnail!) : PublicUrlHelper.GetItemThumbnailtUri(response.Id, response.Option.Version);
        return response;
    }

    /// <summary>
    /// 템플릿 리스트
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<CvListResponse<ItemTemplateResponse>> Handle(GetItemTemplateListRequest request, CancellationToken cancellationToken)
    {
        var filter = Builders<ItemTemplateResponse>.Filter.Eq("worldid", Uuid.FromBase62(request.W).ToBsonBinaryData());
        
        if (request.Category != null)
        {
            var cateArray = request.Category.Split(",");
            foreach (var cate in cateArray)
            {
                if (string.IsNullOrEmpty(cate)) throw new ErrorInvalidParam(cate);
                if (!int.TryParse(cate, out int result)) throw new ErrorInvalidParam(cate);
            }

            filter &= Builders<ItemTemplateResponse>.Filter.In("option.category", new BsonArray() { request.Category });
        }

        var options = new FindOptions<ItemTemplateResponse>
        {
            Skip = request.Page - 1,
            Limit = request.Limit
        };

        var itemTemplateList = await _dbContext.GetListAsync("item_template", filter, options, cancellationToken);
        foreach (var itemTemplate in itemTemplateList)
        {
            itemTemplate.Resource ??= new ItemTemplateResourceResponse();
            itemTemplate.Resource.Manifest = PublicUrlHelper.GetItemManifestUri(itemTemplate.Id, itemTemplate.Option.Version);
            itemTemplate.Resource.Preview = itemTemplate.Resource.Preview != null ? PublicUrlHelper.GetResourceImageUri(itemTemplate.Resource.Preview!) : null;
            itemTemplate.Resource.Thumbnail = itemTemplate.Resource.Thumbnail != null || itemTemplate.Resource.Thumbnail != "" ? PublicUrlHelper.GetResourceImageUri(itemTemplate.Resource.Thumbnail!) : PublicUrlHelper.GetItemThumbnailtUri(itemTemplate.Id, itemTemplate.Option.Version);
        }
        var Count = new CvListCountResponse
        {
            Current = request.Page,
            Page = request.Page,
            Limit = request.Limit,
        };
        var response = new CvListResponse<ItemTemplateResponse>(Count, itemTemplateList);
        return response;
    }

    /// <summary>
    /// 템플릿 삭제
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ErrorNotFound"></exception>
    public async Task<string> Handle(DeleteItemTemplateRequest request, CancellationToken cancellationToken)
    {
        if (!_profile.IsAdmin)
        {
            throw new ErrorBadRequest("User Not Delete Template.");
        }

        var itemTemplateDoc = await _dbContext.GetDocAsync<ItemTemplateDoc>(Uuid.FromBase62(request.Itemid), cancellationToken);
        if (!itemTemplateDoc.Exists)
        {
            throw new ErrorNotFound("Not Found Item Template " + request.Itemid);
        }
        await itemTemplateDoc.DeleteAsync(true, cancellationToken);

        return "";
    }
}


