using Colorverse.Application.Mediator;
using Colorverse.Application.Session;
using Colorverse.Common;
using Colorverse.Common.DataTypes;
using Colorverse.Common.Errors;
using Colorverse.Common.Exceptions;
using Colorverse.Common.Helper;
using Colorverse.Database;
using Colorverse.Meta.Apis.DataTypes.Market;
using Colorverse.Meta.Documents;
using Colorverse.Meta.Events.DataTypes;
using CvFramework.Common;
using CvFramework.MongoDB.Extensions;
using MediatR;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Colorverse.Meta.Mediators;

/// <summary>
/// 유저아이템 Mediator
/// </summary>
[MediatorHandler]
public class MarketMediatorHandler : MediatorHandlerBase,
    IMediatorHandler<GetMarketRequest, MarketResponse>,
    IMediatorHandler<GetMarketListRequest, CvListResponse<MarketResponse>>,
    IMediatorHandler<PurchaseMarketListRequest, MarketOrderResponse>
{
    private readonly IContextUserProfile _profile;
    private readonly IDbContext _dbContext;
    private readonly IMediator _mediator;

    /// <summary>
    /// Market mediator handler
    /// </summary>
    /// <param name="profile"></param>
    /// <param name="dbContext"></param>
    /// <param name="mediator"></param>
    public MarketMediatorHandler(
        IContextUserProfile profile, 
        IDbContext dbContext,
        IMediator mediator)
    {
        _profile = profile;
        _dbContext = dbContext;
        _mediator = mediator;
    }

    /// <summary>
    /// 단일 마켓 조회
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<MarketResponse> Handle(GetMarketRequest request, CancellationToken cancellationToken)
    {
        var marketProductDoc = await _dbContext.GetDocAsync<MarketProductDoc>(Uuid.FromBase62(request.Productid), cancellationToken);
        if (!marketProductDoc.Exists)
        {
            throw new ErrorNotfoundId(request.Productid);
        }
        
        if (marketProductDoc.GetBoolean("option","delete")) //삭제요청상태
        {
            throw new ErrorNotfoundId(request.Productid);
        }
        if(marketProductDoc.GetInt32("status") != 3) // 판매중이아닐시 에러
        {
            throw new ErrorNotFound("Sale stop product "+ request.Productid);
        }

        var response = marketProductDoc.To<MarketResponse>();
        response.Resource ??= new MarketResourceResponse();
        if (response.Type == 1) //아이템
        {
            response.Resource.Manifest = PublicUrlHelper.GetItemManifestUri(response.Id, response.Option.SaleVersion);
            response.Resource.Preview = response.Resource.Preview != null ? PublicUrlHelper.GetResourceImageUri(response.Resource.Preview) : null;
            response.Resource.Thumbnail = response.Resource.Thumbnail != null || response.Resource.Thumbnail != "" ? PublicUrlHelper.GetResourceImageUri(response.Resource.Thumbnail!) : PublicUrlHelper.GetItemThumbnailtUri(response.Id, response.Option.SaleVersion);
        }
        else // 애셋
        {
            response.Resource.Manifest = PublicUrlHelper.GetAssetManifestUri(response.Id, response.Option.SaleVersion);
            response.Resource.Thumbnail = response.Resource.Thumbnail != null || response.Resource.Thumbnail != "" ? PublicUrlHelper.GetResourceImageUri(response.Resource.Thumbnail!) : PublicUrlHelper.GetAssetThumbnailtUri(response.Id, response.Option.SaleVersion);
        }
        return response;
    }

    /// <summary>
    /// 마켓 리스트
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<CvListResponse<MarketResponse>> Handle(GetMarketListRequest request, CancellationToken cancellationToken)
    {
        if (_profile.WorldId != Uuid.FromBase62(request.W))
        {
            throw new ErrorInvalidParam(request.W + "Not Matching Token worldid");
        }

        var filter = Builders<MarketResponse>.Filter.Eq("worldid", Uuid.FromBase62(request.W).ToBsonBinaryData());
            filter &= Builders<MarketResponse>.Filter.Eq("status", 3); // 판매중
            filter &= Builders<MarketResponse>.Filter.Eq("option.delete", false); // 삭제요청상태

        if (request.Category != null)
        {
            filter &= Builders<MarketResponse>.Filter.In("option.category", new BsonArray() { request.Category });
        }
        if(request.NoneSale != null)
        {
            filter &= Builders<MarketResponse>.Filter.Eq("option.none_sale", request.NoneSale);
        }
        
        var options = new FindOptions<MarketResponse>
        {
            Skip = request.Page - 1,
            Limit = request.Limit
        };
        var marketList = await _dbContext.GetListAsync("market_product", filter, options, cancellationToken);
        foreach (var market in marketList)
        {
            market.Resource ??= new MarketResourceResponse();
            if (market.Type == 1) //아이템
            {
                market.Resource.Manifest = PublicUrlHelper.GetItemManifestUri(market.Id, market.Option.SaleVersion);
                market.Resource.Preview = market.Resource.Preview != null ? PublicUrlHelper.GetResourceImageUri(market.Resource.Preview) : null;
                market.Resource.Thumbnail = market.Resource.Thumbnail != null || market.Resource.Thumbnail != "" ? PublicUrlHelper.GetResourceImageUri(market.Resource.Thumbnail!) : PublicUrlHelper.GetItemThumbnailtUri(market.Id, market.Option.SaleVersion);
            }
            else // 애셋
            {
                market.Resource.Manifest = PublicUrlHelper.GetAssetManifestUri(market.Id, market.Option.SaleVersion);
                market.Resource.Thumbnail = market.Resource.Thumbnail != null || market.Resource.Thumbnail != "" ? PublicUrlHelper.GetResourceImageUri(market.Resource.Thumbnail!) : PublicUrlHelper.GetAssetThumbnailtUri(market.Id, market.Option.SaleVersion);
            }
        }
        var Count = new CvListCountResponse
        {
            Current = request.Page,
            Page = request.Page,
            Limit = request.Limit,
        };
        var response = new CvListResponse<MarketResponse>(Count, marketList);
        return response;
    }

    /// <summary>
    /// 마켓 목록 구매
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<MarketOrderResponse> Handle(PurchaseMarketListRequest request, CancellationToken cancellationToken)
    {
        var userItemIds = new List<Uuid>(request.Products.Length);
        var marketArray = new MarketResponse[request.Products.Length];
        var totalPrice = new Dictionary<int, int>();
        // 상품아이디로 상품을 조회. 있는지체크
        for (int i = 0; i < request.Products.Length; i++)
        {
            var product = request.Products[i];
            var marketProductDoc = await _dbContext.GetDocAsync<MarketProductDoc>(Uuid.FromBase62(product.Productid), cancellationToken);
            if (!marketProductDoc.Exists)
            {
                throw new ErrorNotfoundId(product.Productid);
            }

            // 유저아이템에 속한지 체크
            var userItemFilter = Builders<BsonDocument>.Filter.Eq("itemid", Uuid.FromBase62(product.Productid));
            userItemFilter &= Builders<BsonDocument>.Filter.Eq("profileid", _profile.ProfileId.ToBsonBinaryData());
            var userItemCheck = await _dbContext.GetDocAsync<UserItemDoc>(userItemFilter, cancellationToken);
            if (userItemCheck.Exists)
            {
                throw new ErrorBadRequest("Duplicate UserItem " + request.Products[i].Productid);
            }

            var market = marketProductDoc.To<MarketResponse>();
            // 비매품체크
            if (!market.Option.NoneSale)
            {
                totalPrice.Add(market.Option.Price.Type, market.Option.Price.Amount);
            }

            marketArray[i] = market;
        }

        foreach(var price in request.TotalPrice)
        {
            if ((price.Type == 2 && !totalPrice.ContainsKey(2)) || (price.Type == 2 && totalPrice.ContainsKey(2) && price.Amount != totalPrice[2]))
            {
                throw new ErrorBadRequest("Not Matching priceType , priceAmount"); // 최종 상품가격이 일치하는않는경우 badrequest
            }
            if ((price.Type == 3 && !totalPrice.ContainsKey(3)) || (price.Type == 3 && totalPrice.ContainsKey(3) && price.Amount != totalPrice[3]))
            {
                throw new ErrorBadRequest("Not Matching priceType , priceAmount"); // 최종 상품가격이 일치하는않는경우 badrequest
            }
            if ((price.Type == 4 && !totalPrice.ContainsKey(4)) || (price.Type == 4 && totalPrice.ContainsKey(4) && price.Amount != totalPrice[4]))
            {
                throw new ErrorBadRequest("Not Matching priceType , priceAmount"); // 최종 상품가격이 일치하는않는경우 badrequest
            }
        }

        // 빌링 이벤트로 차감이벤트진행





        //유저아이템생성
        foreach (var market in marketArray)
        {
            if (market.Type == 1 && !market.Option.NoneSale) // 아이템인지, 비매품이아닌지 체크(비매품은 유저아이템에 포함되지않는다.)
            {
                //유저 아이템 생성
                var userItemnDoc = UserItemDoc.Create(Uuid.FromBase62(market.Id), _profile.ProfileId);
                _dbContext.Add(userItemnDoc);
                userItemnDoc.SetUserItem(_profile.UserId, _profile.WorldId, new BsonArray(market.Option.Category));
                ////userItemRevisionDoc.SetQuantity(param.Products[j].Quantity); 
                userItemnDoc.SetQuantity(1);
                await userItemnDoc.SaveAsync(cancellationToken); // create

                userItemIds.Add(market.Id);
            }
        }
        // 카운트 동기화
        if (userItemIds.Any())
        {
            _ = _mediator.Publish(new EventUserItemPurchased(_profile.ProfileId, userItemIds));
        }

        // 오더컬렉션 결과를 반환
        var marketOrderDoc = MarketOrderDoc.Create(SvcDomainType.MetaMarketOrder);
        _dbContext.Add(marketOrderDoc);
        marketOrderDoc.SetMarketOrder(marketArray, totalPrice, _profile.UserId, null ,_profile.ProfileId);  // 추후 타운아이디추가되면 넣을것
        await marketOrderDoc.SaveAsync(cancellationToken); // create

        // 주문결과 반환
        return marketOrderDoc.To<MarketOrderResponse>();
    }
}
