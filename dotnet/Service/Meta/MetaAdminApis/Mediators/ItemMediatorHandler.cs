using Colorverse.Application.Mediator;
using Colorverse.Common.DataTypes;
using Colorverse.Common.Errors;
using Colorverse.Common.Exceptions;
using Colorverse.Common.Helper;
using Colorverse.Database;
using Colorverse.MetaAdmin.Apis.DataTypes.Item;
using Colorverse.MetaAdmin.DataTypes.Item;
using Colorverse.MetaAdmin.DataTypes.Resource;
using Colorverse.MetaAdmin.Documents;
using Colorverse.ResourceLibrary;
using Colorverse.UserLibrary;
using CvFramework.Common;
using CvFramework.MongoDB.Extensions;
using Google.Apis.Sheets.v4.Data;
using MongoDB.Bson;
using MongoDB.Driver;
using ResourceLibrary.Nats.V1.DataTypes;

namespace Colorverse.MetaAdmin.Mediators;

/// <summary>
/// 
/// </summary>
public class ItemMediatorHandler : MediatorHandlerBase,
    IMediatorHandler<GetItemRequest, ItemResponse>,
    IMediatorHandler<GetItemListRequest, CvAdminListResponse<ItemResponse>>,
    IMediatorHandler<UpdateItemRequest, ItemResponse>,
    IMediatorHandler<SaleApprovalItemRequest, ItemResponse>,
    IMediatorHandler<SaleRejectionItemRequest, ItemResponse>,
    IMediatorHandler<DeleteItemRequest, ItemResponse>,
    IMediatorHandler<DeleteCancelItemRequest, ItemResponse>,
    IMediatorHandler<CreateItemTemplateRequest, ItemTemplateResponse>,
    IMediatorHandler<GetItemTemplateListRequest, CvAdminListResponse<ItemTemplateResponse>>,
    IMediatorHandler<DeleteItemTemplateRequest, string>
{
    private readonly ILogger _logger;
    private readonly IUserLibrary _userLibrary;
    private readonly IDbContext _dbContext;
    private readonly IResourceLibrary _resourceLibrary;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="dbContext"></param>
    /// <param name="userLibrary"></param>
    /// <param name="resourceLibrary"></param>
    public ItemMediatorHandler(ILogger<ItemMediatorHandler> logger, IDbContext dbContext, IUserLibrary userLibrary, IResourceLibrary resourceLibrary)
    {
        _logger = logger;
        _userLibrary = userLibrary;
        _dbContext = dbContext;
        _resourceLibrary = resourceLibrary;
    }

    /// <summary>
    /// 아이템 조회
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<ItemResponse> Handle(GetItemRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("ItemDoc GetDocAsync : {itemid}", request.Itemid);
        var itemDoc = await _dbContext.GetDocAsync<ItemDoc>(Uuid.FromBase62(request.Itemid), false, cancellationToken);
        if (!itemDoc.Exists)
        {
            throw new ErrorNotfoundId(request.Itemid);
        }
        _logger.LogInformation("itemDoc.To<ItemResult> : {itemdoc}",itemDoc);
        var response = itemDoc.To<ItemResponse>();

        _logger.LogInformation("ItemResultResource : {result} , {version}",response, response.Option.Version);
        response.Resource ??= new ItemResourceResponse();
        response.Resource.Manifest = PublicUrlHelper.GetItemManifestUri(response.Id, response.Option.Version);
        response.Resource.Preview = response.Resource.Preview != null ? PublicUrlHelper.GetResourceImageUri(response.Resource.Preview) : null;
        response.Resource.Thumbnail = response.Resource.Thumbnail != null || response.Resource.Thumbnail != "" ? PublicUrlHelper.GetResourceImageUri(response.Resource.Thumbnail!) : PublicUrlHelper.GetItemThumbnailtUri(response.Id, response.Option.Version);
        return response;
    }

    /// <summary>
    /// 아이템 리스트
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<CvAdminListResponse<ItemResponse>> Handle(GetItemListRequest request, CancellationToken cancellationToken)
    {
        var filter = Builders<ItemResponse>.Filter.Eq("profileid", Uuid.FromBase62(request.Profileid).ToBsonBinaryData());
        if(request.NoneSale != null)
        {
            _logger.LogInformation("param.NoneSale : {nonesale}", request.NoneSale);
            filter &= Builders<ItemResponse>.Filter.Eq("option.none_sale", request.NoneSale);
        }
        if (request.Category != null)
        {
            var cateArray = request.Category.Split(",");
            for (int i = 0; i < cateArray.Length; i++)
            {
                if (string.IsNullOrEmpty(cateArray[i])) throw new ErrorInvalidParam(cateArray[i]);
            }
            _logger.LogInformation("param.Category : {Category}", new BsonArray() { request.Category });
            filter &= Builders<ItemResponse>.Filter.In("option.category", new BsonArray() { request.Category });
        }
        if(request.RedayStatus != null)
        {
            _logger.LogInformation("param.RedayStatus : {RedayStatus}", request.RedayStatus);
            filter &= Builders<ItemResponse>.Filter.Eq("option.ready_status", request.RedayStatus);
        }
        if(request.SaleStatus != null)
        {
            _logger.LogInformation("param.SaleStatus : {SaleStatus}", request.SaleStatus);
            filter &= Builders<ItemResponse>.Filter.Eq("option.sale_status", request.SaleStatus);
        }
        if(request.SaleReviewStatus != null)
        {
            _logger.LogInformation("param.SaleReviewStatus : {SaleReviewStatus}", request.SaleReviewStatus);
            filter &= Builders<ItemResponse>.Filter.Eq("option.sale_review_status", request.SaleReviewStatus);
        }
        if(request.JudgeStatus != null)
        {
            _logger.LogInformation("param.JudgeStatus : {JudgeStatus}", request.JudgeStatus);
            filter &= Builders<ItemResponse>.Filter.Eq("option.judge_status", request.JudgeStatus);
        }
        if(request.Blind != null)
        {
            _logger.LogInformation("param.Blind : {Blind}", request.Blind);
            filter &= Builders<ItemResponse>.Filter.Eq("option.blind", request.Blind);
        }
        if(request.Delete != null)
        {
            _logger.LogInformation("param.Delete : {Delete}", request.Delete);
            filter &= Builders<ItemResponse>.Filter.Eq("option.delete", request.Delete);
        }

        var options = new FindOptions<ItemResponse>
        {
            Skip = request.Page - 1,
            Limit = request.Limit
        };

        _logger.LogInformation("GetListAsync item");
        var itemList = await _dbContext.GetListAsync("item", filter, options, cancellationToken);
        _logger.LogInformation("GetListCountAsync item");
        var totalCount = await _dbContext.GetListCountAsync("item", Builders<ItemResponse>.Filter.Eq("profileid", Uuid.FromBase62(request.Profileid).ToBsonBinaryData()), null, cancellationToken);
        foreach (var item in itemList)
        {
            item.Resource ??= new ItemResourceResponse();
            item.Resource.Manifest = PublicUrlHelper.GetItemManifestUri(item.Id, item.Option.Version);
            item.Resource.Preview = item.Resource.Preview != null ? PublicUrlHelper.GetResourceImageUri(item.Resource.Preview) : null;
            item.Resource.Thumbnail = item.Resource.Thumbnail != null || item.Resource.Thumbnail != "" ? PublicUrlHelper.GetResourceImageUri(item.Resource.Thumbnail!) : PublicUrlHelper.GetItemThumbnailtUri(item.Id, item.Option.Version);
        }
        var itemListCount = new CvAdminListCountResponse
        {
            Current = request.Page,
            Page = request.Page,
            Limit = request.Limit,
            Total = (int)totalCount
        };
        var response = new CvAdminListResponse<ItemResponse>(itemListCount, itemList);
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
        var itemUuid = Uuid.FromBase62(request.Itemid);
        _logger.LogInformation("GetDocAsync ItemDoc {itemid}", request.Itemid);
        var itemDoc = await _dbContext.GetDocAsync<ItemDoc>(itemUuid, false, cancellationToken);
        if (!itemDoc.Exists)
        {
            throw new ErrorNotfoundId(request.Itemid);
        }
        
        BsonDocument? manifest = null;
        if (request.Option != null && itemDoc.GetInt32("option","version") != request.Option.Version) // 버전이 변동되면 revision 수정으로 감지
        {
            _logger.LogInformation("GetDocAsync ResourceDoc {itemid}", request.Itemid);
            var resourceDoc = await _dbContext.GetDocAsync<ResourceDoc>(itemUuid, false, cancellationToken);
            if (resourceDoc.Exists)
            {
                var resourceDto = resourceDoc.To<ResourceDto>();
                if (resourceDto != null && resourceDto.Manifest != null)
                {
                    manifest = resourceDto.Manifest;

                    // 아이템 resource 사용처리
                    _logger.LogInformation("resourceDoc SaveAsync {itemid}", request.Itemid);
                    resourceDoc.UseReference(); 
                    _ = await resourceDoc.SaveAsync(cancellationToken);
                }
            }

            // item revision 생성
            var itemRevisionDoc = ItemRevisionDoc.Create(itemUuid, request.Option.Version);
            _dbContext.Add(itemRevisionDoc);
            itemRevisionDoc.SetItemRevision(1, manifest);
            _logger.LogInformation("itemRevisionDoc SaveAsync {itemid}", request.Itemid);
            await itemRevisionDoc.SaveAsync(cancellationToken);
        }

        // 아이템 수정
        itemDoc.UpdateItem(request, manifest);
        _logger.LogInformation("itemDoc SaveAsync {itemid}", request.Itemid);
        await itemDoc.SaveAsync(cancellationToken);

        // 리소스서버 섬네일,프리뷰 신규값 사용처리, 기존값 사용중지처리
        if (request.Resource != null)
        {
            if (request.Resource.Thumbnail != null)
            {
                _logger.LogInformation("_resourceLibrary AddReference {Thumbnail}", request.Resource.Thumbnail);
                if (request.Resource.Thumbnail != "")
                {
                    var thumbnailResult = await _resourceLibrary.AddReference(new AddReferenceParam(Uuid.FromBase62(request.Resource.Thumbnail)), cancellationToken);
                    if (!thumbnailResult.Succeeded) throw new ErrorInternal(nameof(thumbnailResult));
                }
                if (itemDoc.GetNullableString("resource", "thumbnail") != null && itemDoc.GetNullableString("resource", "thumbnail") != "")
                {
                    // 실패했을경우 어떤식으로 처리할지 고민필요 .. logging????
                    _logger.LogInformation("_resourceLibrary RemoveReference {Thumbnail}", itemDoc.GetString("resource", "thumbnail"));
                    _ = await _resourceLibrary.RemoveReference(new RemoveReferenceParam() { ResourceId = Uuid.FromBase62(itemDoc.GetString("resource", "thumbnail")) }, cancellationToken);
                }
            }
            if (request.Resource.Preview != null)
            {
                _logger.LogInformation("_resourceLibrary AddReference {Preview}", request.Resource.Preview);
                var previewResult = await _resourceLibrary.AddReference(new AddReferenceParam(Uuid.FromBase62(request.Resource.Preview)), cancellationToken);
                if (!previewResult.Succeeded) throw new ErrorInternal(nameof(previewResult));
                if (itemDoc.GetNullableString("resource", "preview") != null)
                {
                    // 실패했을경우 어떤식으로 처리할지 고민필요 .. logging????
                    _logger.LogInformation("_resourceLibrary RemoveReference {Preview}", itemDoc.GetString("resource", "preview"));
                    _ = await _resourceLibrary.RemoveReference(new RemoveReferenceParam() { ResourceId = Uuid.FromBase62(itemDoc.GetString("resource", "preview")) }, cancellationToken);
                }
            }
        }
        // 마켓 수정
        _logger.LogInformation("MarketProductDoc GetDocAsync {param.Itemid}", request.Itemid);
        var marketProductDoc = await _dbContext.GetDocAsync<MarketProductDoc>(itemUuid, false, cancellationToken);
        if (marketProductDoc.Exists)
        {
            marketProductDoc.UpdateMarket(request, manifest);
            _logger.LogInformation("MarketProductDoc SaveAsync {param.Itemid}", request.Itemid);
            await marketProductDoc.SaveAsync(cancellationToken);
        }

        // 템플릿 수정
        _logger.LogInformation("ItemTemplateDoc GetDocAsync {param.Itemid}", request.Itemid);
        var templateDoc = await _dbContext.GetDocAsync<ItemTemplateDoc>(itemUuid, false, cancellationToken);
        if(templateDoc.Exists)
        {
            templateDoc.UpdateTempalte(request);
            _logger.LogInformation("ItemTemplateDoc SaveAsync {param.Itemid}", request.Itemid);
            await templateDoc.SaveAsync(cancellationToken);
        }

        var response = itemDoc.To<ItemResponse>();
        response.Resource ??= new ItemResourceResponse();
        response.Resource.Manifest = PublicUrlHelper.GetItemManifestUri(response.Id, response.Option.Version);
        response.Resource.Preview = response.Resource.Preview != null ? PublicUrlHelper.GetResourceImageUri(response.Resource.Preview) : null;
        response.Resource.Thumbnail = response.Resource.Thumbnail != null || response.Resource.Thumbnail != "" ? PublicUrlHelper.GetResourceImageUri(response.Resource.Thumbnail!) : PublicUrlHelper.GetItemThumbnailtUri(response.Id, response.Option.Version);
        _logger.LogInformation("itemDoc ItemResultResource {result}, {version}", response, response.Option.Version);
        return response;
    }

    /// <summary>
    /// 아이템 승인
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<ItemResponse> Handle(SaleApprovalItemRequest request, CancellationToken cancellationToken)
    {
        var itemUuid = Uuid.FromBase62(request.Itemid);
        //아이템 존재하나 체크
        _logger.LogInformation("ItemDoc GetDocAsync {param.Itemid}", request.Itemid);
        var itemDoc = await _dbContext.GetDocAsync<ItemDoc>(itemUuid, false, cancellationToken);
        if (!itemDoc.Exists)
        {
            throw new ErrorNotfoundId(request.Itemid);
        }
        
        _logger.LogInformation("marketProductDoc GetDocAsync {param.Itemid}", request.Itemid);
        var marketProductDoc = await _dbContext.GetDocAsync<MarketProductDoc>(itemUuid, false, cancellationToken); // 마켓에 등록되있는지 체크
        var saleAccept = DateTime.UtcNow;
        if (request.LaterReview) //  사후심사
        {
            //사후심사 승인
            itemDoc.SaleApprovalItem();

            if (marketProductDoc.Exists)
            {
                //마켓 사후심사 승인
                marketProductDoc.SaleApprovalMarketProduct();
                _logger.LogInformation("marketProductDoc SaveAsync {param.Itemid}", request.Itemid);
                await marketProductDoc.SaveAsync(cancellationToken);
            }

            // 사후심사 승인되면 신고 count 초기화
            var reportResult = await _userLibrary.ReportReset(itemUuid, cancellationToken);
            if (!reportResult.Succeeded)
            {
                throw new ErrorBadRequest("repory reset Fail");
            }
        }
        else
        {
            var saleStatus = 2;
            if (itemDoc.GetNullableBoolean("option","instant_sale") != null && itemDoc.GetNullableBoolean("option", "instant_sale") == true)
            {
                saleStatus = 3;
            }

            // item_revision 수정 status 1
            var revisionFilter = Builders<BsonDocument>.Filter.Eq("itemid", itemDoc.GetUuid("_id").ToBsonBinaryData());
            revisionFilter &= Builders<BsonDocument>.Filter.Eq("version", itemDoc.GetInt32("option","version"));
            _logger.LogInformation("ItemRevisionDoc GetDocAsync {param.Itemid}", request.Itemid);
            var findRevisionDoc = await _dbContext.GetDocAsync<ItemRevisionDoc>(revisionFilter, false, cancellationToken);
            findRevisionDoc.SaleApprovalRevision();
            _logger.LogInformation("ItemRevisionDoc SaveAsync {param.Itemid}", request.Itemid);
            await findRevisionDoc.SaveAsync(cancellationToken);

            var itemRevision = findRevisionDoc.To<ItemRevisionResult>();
            // 심사 승인 ( 판매승인시 revision에있는 manifest 넣어주기) 
            itemDoc.SaleApprovalItem(request, saleAccept, saleStatus, itemDoc.GetInt32("option", "version"), itemRevision);

            // market product 등록 즉시판매인지 아닌지 판단해서 판매허용 상태또는 판매중으로 등록
            marketProductDoc = MarketProductDoc.Create(itemUuid);
            _dbContext.Add(marketProductDoc);
            marketProductDoc.SaleApprovalMarketProduct(itemDoc, saleAccept, saleStatus);
            _logger.LogInformation("marketProductDoc SaveAsync {param.Itemid}", request.Itemid);
            await marketProductDoc.SaveAsync(cancellationToken);
        }

        _logger.LogInformation("itemDoc SaveAsync {param.Itemid}", request.Itemid);
        await itemDoc.SaveAsync(cancellationToken);

        var response = itemDoc.To<ItemResponse>();
        response.Resource ??= new ItemResourceResponse();
        response.Resource.Manifest = PublicUrlHelper.GetItemManifestUri(response.Id, response.Option.Version);
        response.Resource.Preview = response.Resource.Preview != null ? PublicUrlHelper.GetResourceImageUri(response.Resource.Preview) : null;
        response.Resource.Thumbnail = response.Resource.Thumbnail != null || response.Resource.Thumbnail != "" ? PublicUrlHelper.GetResourceImageUri(response.Resource.Thumbnail!) : PublicUrlHelper.GetItemThumbnailtUri(response.Id, response.Option.Version);
        _logger.LogInformation("itemDoc ItemResultResource {result}, {version}", response, response.Option.Version);
        return response;
    }

    /// <summary>
    /// 아이템 반려
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<ItemResponse> Handle(SaleRejectionItemRequest request, CancellationToken cancellationToken)
    {
        //아이템 존재하나 체크
        _logger.LogInformation("itemDoc GetDocAsync {param.Itemid}", request.Itemid);
        var itemDoc = await _dbContext.GetDocAsync<ItemDoc>(Uuid.FromBase62(request.Itemid), false, cancellationToken);
        if (!itemDoc.Exists)
        {
            throw new ErrorNotfoundId(request.Itemid);
        }
        
        if (!request.LaterReview) //사후심사가아닐경우 판매수수료 환불 
        {
            // 판매수수료 환불



            // 심사반려
            itemDoc.SaleRejectionItem(request);
        }
        else
        {
            //사후심사 반려
            itemDoc.SaleRejectionItem();
        }

        _logger.LogInformation("itemDoc SaveAsync {param.Itemid}", request.Itemid);
        await itemDoc.SaveAsync(cancellationToken);
        
        // user_item 삭제
        var userItemFilter = Builders<BsonDocument>.Filter.Eq("itemid", Uuid.FromBase62(request.Itemid).ToBsonBinaryData());
        userItemFilter &= Builders<BsonDocument>.Filter.Eq("profileid", itemDoc.GetUuid("profileid").ToBsonBinaryData());
        _logger.LogInformation("UserItemDoc GetDocAsync {param.Itemid}", request.Itemid);
        var userItemDoc = await _dbContext.GetDocAsync<UserItemDoc>(userItemFilter, false, cancellationToken);
        if (userItemDoc.Exists)
        {
            _logger.LogInformation("UserItemDoc DeleteAsync {param.Itemid}", request.Itemid);
            await userItemDoc.DeleteAsync(true, cancellationToken);

            // 카운트 동기화
            _logger.LogInformation("_userLibrary UpdateItemCount {param.Itemid}", request.Itemid);
            _ = _userLibrary.UpdateItemCount(itemDoc.GetUuid("_id"), -1, cancellationToken);
        }

        var response = itemDoc.To<ItemResponse>();
        response.Resource ??= new ItemResourceResponse();
        response.Resource.Manifest = PublicUrlHelper.GetItemManifestUri(response.Id, response.Option.Version);
        response.Resource.Preview = response.Resource.Preview != null ? PublicUrlHelper.GetResourceImageUri(response.Resource.Preview) : null;
        response.Resource.Thumbnail = response.Resource.Thumbnail != null || response.Resource.Thumbnail != "" ? PublicUrlHelper.GetResourceImageUri(response.Resource.Thumbnail!) : PublicUrlHelper.GetItemThumbnailtUri(response.Id, response.Option.Version);
        _logger.LogInformation("itemDoc ItemResultResource {result}, {version}", response, response.Option.Version);
        return response;
    }

    /// <summary>
    /// 아이템 삭제요청
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<ItemResponse> Handle(DeleteItemRequest request, CancellationToken cancellationToken)
    {
        // template 아이템인지 확인후 tempalte item이면 template먼저 삭제 권유
        _logger.LogInformation("itemDoc GetDocAsync {param.Itemid}", request.Itemid);
        var itemDoc = await _dbContext.GetDocAsync<ItemDoc>(Uuid.FromBase62(request.Itemid), false, cancellationToken);
        
        if (!itemDoc.Exists)
        {
            throw new ErrorNotfoundId(request.Itemid);
        }
        if(itemDoc.GetBoolean("option", "template"))
        {
            throw new ErrorInvalidParam(nameof(request.Itemid), "dependency template id. template delete please");
        }

        //아이템 상태값 변경 blind, delete = true
        itemDoc.DeleteItem(request);
        _logger.LogInformation("itemDoc SaveAsync {param.Itemid}", request.Itemid);
        await itemDoc.SaveAsync(cancellationToken);
        
        //마켓 상태값 변경 blind, delete = true
        _logger.LogInformation("MarketProductDoc GetDocAsync {param.Itemid}", request.Itemid);
        var marketProductDoc = await _dbContext.GetDocAsync<MarketProductDoc>(Uuid.FromBase62(request.Itemid), false, cancellationToken); // 마켓에 등록되있는지 체크
        if(marketProductDoc.Exists)
        {
            marketProductDoc.DeleteMarketProduct();
            _logger.LogInformation("MarketProductDoc SaveAsync {param.Itemid}", request.Itemid);
            await marketProductDoc.SaveAsync(cancellationToken);
        }

        //아이템 삭제요청 collection 추가
        var itemDeleteRequestDoc = ItemDeleteRequestDoc.Create(Uuid.FromBase62(request.Itemid));
        _dbContext.Add(itemDeleteRequestDoc);
        _logger.LogInformation("itemDeleteRequestDoc SaveAsync {param.Itemid}", request.Itemid);
        await itemDeleteRequestDoc.SaveAsync(cancellationToken);

        var response = itemDoc.To<ItemResponse>();
        response.Resource ??= new ItemResourceResponse();
        response.Resource.Manifest = PublicUrlHelper.GetItemManifestUri(response.Id, response.Option.Version);
        response.Resource.Preview = response.Resource.Preview != null ? PublicUrlHelper.GetResourceImageUri(response.Resource.Preview) : null;
        response.Resource.Thumbnail = response.Resource.Thumbnail != null || response.Resource.Thumbnail != "" ? PublicUrlHelper.GetResourceImageUri(response.Resource.Thumbnail!) : PublicUrlHelper.GetItemThumbnailtUri(response.Id, response.Option.Version);
        _logger.LogInformation("itemDoc ItemResultResource {result}, {version}", response, response.Option.Version);
        return response;
    }

    /// <summary>
    /// 아이템 삭제요청 취소
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<ItemResponse> Handle(DeleteCancelItemRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("ItemDoc GetDocAsync {param.Itemid}", request.Itemid);
        var itemDoc = await _dbContext.GetDocAsync<ItemDoc>(Uuid.FromBase62(request.Itemid), false, cancellationToken);
        if (!itemDoc.Exists)
        {
            throw new ErrorNotfoundId(request.Itemid);
        }

        //아이템 상태값 변경 blind, delete = false
        itemDoc.DeleteCancelItem();
        _logger.LogInformation("ItemDoc SaveAsync {param.Itemid}", request.Itemid);
        await itemDoc.SaveAsync(cancellationToken);
        
        //마켓 상태값 변경 blind, delete = false
        _logger.LogInformation("MarketProductDoc GetDocAsync {param.Itemid}", request.Itemid);
        var marketProductDoc = await _dbContext.GetDocAsync<MarketProductDoc>(Uuid.FromBase62(request.Itemid), false, cancellationToken); // 마켓에 등록되있는지 체크
        if (marketProductDoc.Exists)
        {
            marketProductDoc.DeleteCancelMarketProduct();
            _logger.LogInformation("MarketProductDoc SaveAsync {param.Itemid}", request.Itemid);
            await marketProductDoc.SaveAsync(cancellationToken);
        }

        //아이템 삭제요청 collection 삭제
        _logger.LogInformation("ItemDeleteRequestDoc GetDocAsync {param.Itemid}", request.Itemid);
        var itemDeleteRequestDoc = await _dbContext.GetDocAsync<ItemDeleteRequestDoc>(Uuid.FromBase62(request.Itemid), false, cancellationToken);
        if (itemDeleteRequestDoc.Exists)
        {
            _logger.LogInformation("ItemDeleteRequestDoc DeleteAsync {param.Itemid}", request.Itemid);
            await itemDeleteRequestDoc.DeleteAsync(true, cancellationToken);
        }

        var response = itemDoc.To<ItemResponse>();
        response.Resource ??= new ItemResourceResponse();
        response.Resource.Manifest = PublicUrlHelper.GetItemManifestUri(response.Id, response.Option.Version);
        response.Resource.Preview = response.Resource.Preview != null ? PublicUrlHelper.GetResourceImageUri(response.Resource.Preview) : null;
        response.Resource.Thumbnail = response.Resource.Thumbnail != null || response.Resource.Thumbnail != "" ? PublicUrlHelper.GetResourceImageUri(response.Resource.Thumbnail!) : PublicUrlHelper.GetItemThumbnailtUri(response.Id, response.Option.Version);
        _logger.LogInformation("itemDoc ItemResultResource {result}, {version}", response, response.Option.Version);
        return response;
    }

    /// <summary>
    /// 템플릿 생성
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ErrorNotfoundId"></exception>
    /// <exception cref="ErrorBadRequest"></exception>
    public async Task<ItemTemplateResponse> Handle(CreateItemTemplateRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("ItemDoc GetDocAsync {param.Itemid}", request.Itemid);
        var itemDoc = await _dbContext.GetDocAsync<ItemDoc>(Uuid.FromBase62(request.Itemid), false, cancellationToken); // 아이템있는지 체크
        if (!itemDoc.Exists)
        {
            throw new ErrorNotfoundId(request.Itemid);
        }

        _logger.LogInformation("ItemTemplateDoc GetDocAsync {param.Itemid}", request.Itemid);
        var itemTemplateDoc = await _dbContext.GetDocAsync<ItemTemplateDoc>(Uuid.FromBase62(request.Itemid), false, cancellationToken);
        if (itemTemplateDoc.Exists)
        {
            throw new ErrorBadRequest("Duplicate template itemid " + nameof(request.Itemid));
        }
        itemTemplateDoc = ItemTemplateDoc.Create(Uuid.FromBase62(request.Itemid));
        _dbContext.Add(itemTemplateDoc);
        itemTemplateDoc.SetTemplate(itemDoc);
        _logger.LogInformation("ItemTemplateDoc SaveAsync {param.Itemid}", request.Itemid);
        await itemTemplateDoc.SaveAsync(cancellationToken);

        var response = itemTemplateDoc.To<ItemTemplateResponse>();
        response.Resource ??= new ItemTemplateResourceResponse();
        response.Resource.Manifest = PublicUrlHelper.GetItemManifestUri(response.Id, response.Option.Version);
        response.Resource.Preview = response.Resource.Preview != null ? PublicUrlHelper.GetResourceImageUri(response.Resource.Preview) : null;
        response.Resource.Thumbnail = response.Resource.Thumbnail != null || response.Resource.Thumbnail != "" ? PublicUrlHelper.GetResourceImageUri(response.Resource.Thumbnail!) : PublicUrlHelper.GetItemThumbnailtUri(response.Id, response.Option.Version);
        return response;
    }

    /// <summary>
    /// 템플릿 조회
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ErrorInvalidParam"></exception>
    public async Task<CvAdminListResponse<ItemTemplateResponse>> Handle(GetItemTemplateListRequest request, CancellationToken cancellationToken)
    {
        var filter = Builders<ItemTemplateResponse>.Filter.Eq("worldid", Uuid.FromBase62(request.W).ToBsonBinaryData());
        if (request.Category != null)
        {
            var cateArray = request.Category.Split(",");
            foreach (var cate in cateArray)
            {
                if (!int.TryParse(cate, out int result)) throw new ErrorInvalidParam(cate);
            }
            _logger.LogInformation("param.Category {param.Category}", new BsonArray() { request.Category });
            filter &= Builders<ItemTemplateResponse>.Filter.In("option.category", new BsonArray() { request.Category });
        }

        var options = new FindOptions<ItemTemplateResponse>
        {
            Skip = request.Page - 1,
            Limit = request.Limit
        };
        _logger.LogInformation("item_template GetListAsync {w}", request.W);
        var itemTemplateList = await _dbContext.GetListAsync("item_template", filter, options, cancellationToken);
        var totalCount = await _dbContext.GetListCountAsync("item", Builders<ItemTemplateResponse>.Filter.Eq("worldid", Uuid.FromBase62(request.W).ToBsonBinaryData()), null, cancellationToken);
        
        foreach (var itemTemplate in itemTemplateList)
        {
            itemTemplate.Resource ??= new ItemTemplateResourceResponse();
            itemTemplate.Resource.Manifest = PublicUrlHelper.GetItemManifestUri(itemTemplate.Id, itemTemplate.Option.Version);
            itemTemplate.Resource.Preview = itemTemplate.Resource.Preview != null ? PublicUrlHelper.GetResourceImageUri(itemTemplate.Resource.Preview!) : null;
            itemTemplate.Resource.Thumbnail = itemTemplate.Resource.Thumbnail != null || itemTemplate.Resource.Thumbnail != "" ? PublicUrlHelper.GetResourceImageUri(itemTemplate.Resource.Thumbnail!) : PublicUrlHelper.GetItemThumbnailtUri(itemTemplate.Id, itemTemplate.Option.Version);
        }
        var itemListCount = new CvAdminListCountResponse
        {
            Current = request.Page,
            Page = request.Page,
            Limit = request.Limit,
            Total = (int)totalCount
        };
        var response = new CvAdminListResponse<ItemTemplateResponse>(itemListCount, itemTemplateList);
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
        _logger.LogInformation("ItemTemplateDoc GetDocAsync {param.Itemid} ", request.Itemid);
        var itemTemplateDoc = await _dbContext.GetDocAsync<ItemTemplateDoc>(Uuid.FromBase62(request.Itemid), false, cancellationToken);
        if (!itemTemplateDoc.Exists)
        {
            throw new ErrorNotFound("Not Found Item Template " + nameof(request.Itemid));
        }
        _logger.LogInformation("ItemTemplateDoc DeleteAsync {param.Itemid} ", request.Itemid);
        await itemTemplateDoc.DeleteAsync(true, cancellationToken);
        return "";
    }
}
