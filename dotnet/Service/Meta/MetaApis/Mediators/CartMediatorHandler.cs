using Colorverse.Application.Mediator;
using Colorverse.Application.Session;
using Colorverse.Common;
using Colorverse.Common.Errors;
using Colorverse.Common.Exceptions;
using Colorverse.Database;
using Colorverse.Meta.Apis.DataTypes.Cart;
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
/// CartMediatorHandler
/// </summary>
[MediatorHandler]
public class CartMediatorHandler : MediatorHandlerBase,
    IMediatorHandler<GetCartListRequest, CartResponse>,
    IMediatorHandler<CreateCartListRequest, CartResponse>,
    IMediatorHandler<DeleteCartListRequest, CartResponse>,
    IMediatorHandler<PurchaseCartListRequest, MarketOrderResponse>
{
    private readonly IContextUserProfile _profile;
    private readonly IDbContext _dbContext;
    private readonly IMediator _mediator;

    /// <summary>
    /// Cart mediator handler
    /// </summary>
    /// <param name="profile"></param>
    /// <param name="dbContext"></param>
    /// <param name="mediator"></param>
    public CartMediatorHandler(
        IContextUserProfile profile, 
        IDbContext dbContext,
        IMediator mediator
    )
    {
        _profile = profile;
        _dbContext = dbContext;
        _mediator = mediator;
    }

    /// <summary>
    /// 장바구니 목록 조회
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<CartResponse> Handle(GetCartListRequest request, CancellationToken cancellationToken)
    {
        var filter = Builders<BsonDocument>.Filter.Eq("_id", _profile.ProfileId.ToBsonBinaryData());

        var cartDoc = await _dbContext.GetDocAsync<CartDoc>(filter, cancellationToken);
        if (!cartDoc.Exists){ return new CartResponse(); }

        return cartDoc.To<CartResponse>();
    }

    /// <summary>
    /// 장바구니 목록 추가
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<CartResponse> Handle(CreateCartListRequest request, CancellationToken cancellationToken)
    {
        for (int i = 0; i < request.Items.Length; i++)
        {
            if (!Uuid.FromBase62(request.Items[i].Productid).EqualsDomainType((ushort)SvcDomainType.MetaItem) || Uuid.FromBase62(request.Items[i].Productid).EqualsDomainType((ushort)SvcDomainType.MetaAsset))
            {
                throw new ErrorBadRequest("Not Matching DomainType Id= " + request.Items[i].Productid);
            }

            var finditemDoc = await _dbContext.GetDocAsync<ItemDoc>(Uuid.FromBase62(request.Items[i].Productid), cancellationToken);
            if (!finditemDoc.Exists)
            {
                var findAssetDoc = await _dbContext.GetDocAsync<AssetDoc>(Uuid.FromBase62(request.Items[i].Productid), cancellationToken);
                if (!findAssetDoc.Exists)
                {
                    throw new ErrorNotfoundId(request.Items[i].Productid);
                }
            }
        }
       
        var filter = Builders<BsonDocument>.Filter.Eq("_id", _profile.ProfileId.ToBsonBinaryData());
        var findCartDoc = await _dbContext.GetDocAsync<CartDoc>(filter, cancellationToken); //장바구니에 존재하나 체크
        if (findCartDoc.Exists)
        {
            findCartDoc.UpdateCart(request.Items, findCartDoc.To<CartResponse>());
            await findCartDoc.SaveAsync(cancellationToken);
            return findCartDoc.To<CartResponse>();
        }
        else
        {
            CartDoc cartDoc = CartDoc.Create(_profile.ProfileId);
            cartDoc.SetCart(request.Items, _profile.WorldId);
            _dbContext.Add(cartDoc);
            await cartDoc.SaveAsync(cancellationToken); // create
            return cartDoc.To<CartResponse>();
        }
    }

    /// <summary>
    /// 장바구니 목록 삭제
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<CartResponse> Handle(DeleteCartListRequest request, CancellationToken cancellationToken)
    {
        var filter = Builders<BsonDocument>.Filter.Eq("_id", _profile.ProfileId.ToBsonBinaryData());

        var findCartDoc = await _dbContext.GetDocAsync<CartDoc>(filter, cancellationToken); // 프로필아이디 존재하는지 체크
        if (!findCartDoc.Exists)
        {
            throw new ErrorNotfoundId(_profile.ProfileId.Base62);
        }

        var productIdArray = request.Productids.Split(",");
        foreach (var productid in productIdArray)
        {
            findCartDoc.DeleteCart(Uuid.FromBase62(productid));
            await findCartDoc.SaveAsync(cancellationToken);
        }

        return findCartDoc.To<CartResponse>();
    }

    /// <summary>
    /// 장바구니 목록 구매
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<MarketOrderResponse> Handle(PurchaseCartListRequest request, CancellationToken cancellationToken)
    {
        var filter = Builders<BsonDocument>.Filter.Eq("_id", _profile.ProfileId.ToBsonBinaryData());

        var findCartDoc = await _dbContext.GetDocAsync<CartDoc>(filter, cancellationToken); // 프로필아이디 존재하는지 체크
        if (!findCartDoc.Exists)
        {
            throw new ErrorNotfoundId(_profile.ProfileId.Base62);
        }

        var userItemIds = new List<Uuid>(request.Items.Length);
        var marketArray = new MarketResponse[request.Items.Length];
        var totalPrice = new Dictionary<int, int>();
        for (int i = 0; i < request.Items.Length; i++)
        {
            var marketProductDoc = await _dbContext.GetDocAsync<MarketProductDoc>(Uuid.FromBase62(request.Items[i].Productid), cancellationToken); // 마켓에 데이터가 있는지체크
            if (!marketProductDoc.Exists)
            {
                throw new ErrorNotfoundId(request.Items[i].Productid);
            }

            // 유저아이템에 속한지 체크
            var userItemFilter = Builders<BsonDocument>.Filter.Eq("itemid", Uuid.FromBase62(request.Items[i].Productid));
            userItemFilter &= Builders<BsonDocument>.Filter.Eq("profileid", _profile.ProfileId.ToBsonBinaryData());
            var userItemCheck = await _dbContext.GetDocAsync<UserItemDoc>(userItemFilter, cancellationToken);
            if (userItemCheck.Exists)
            {
                throw new ErrorBadRequest("Duplicate UserItem " + request.Items[i].Productid);
            }


            var market = marketProductDoc.To<MarketResponse>();
            // 비매품체크
            if (!market.Option.NoneSale)
            {
                totalPrice.Add(market.Option.Price.Type, market.Option.Price.Amount);
            }

            marketArray[i] = market;
        }

        foreach (var price in request.TotalPrice)
        {
            if((price.PriceType == 2 && !totalPrice.ContainsKey(2) ) || (price.PriceType == 2 && totalPrice.ContainsKey(2) && price.PriceAmount != totalPrice[2]))
            {
                throw new ErrorBadRequest("Not Matching priceType , priceAmount"); // 최종 상품가격이 일치하는않는경우 badrequest
            }
            if ((price.PriceType == 3 && !totalPrice.ContainsKey(3)) || (price.PriceType == 3 && totalPrice.ContainsKey(3) && price.PriceAmount != totalPrice[3]))
            {
                throw new ErrorBadRequest("Not Matching priceType , priceAmount"); // 최종 상품가격이 일치하는않는경우 badrequest
            }
            if ((price.PriceType == 4 && !totalPrice.ContainsKey(4)) || (price.PriceType == 4 && totalPrice.ContainsKey(4) && price.PriceAmount != totalPrice[4]))
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
                var userItemDoc = UserItemDoc.Create(market.Id, _profile.ProfileId);
                _dbContext.Add(userItemDoc);
                userItemDoc.SetUserItem(_profile.UserId, _profile.WorldId, new BsonArray(market.Option.Category));
                userItemDoc.SetQuantity(1);
                await userItemDoc.SaveAsync(cancellationToken); // create

                userItemIds.Add(market.Id);
            }
        }
        // 카운트 동기화
        if (userItemIds.Any())
        {
            _ = _mediator.Publish(new EventUserItemPurchased(_profile.ProfileId, userItemIds));
        }


        // 장바구니 항목 제거
        foreach (var item in request.Items)
        {
            findCartDoc.DeleteCart(Uuid.FromBase62(item.Productid));
            await findCartDoc.SaveAsync(cancellationToken);
        }

        // 오더컬렉션 결과를 반환
        var marketOrderDoc = MarketOrderDoc.Create(SvcDomainType.MetaMarketOrder);
        _dbContext.Add(marketOrderDoc);
        marketOrderDoc.SetMarketOrder(marketArray, totalPrice, _profile.UserId, null, _profile.ProfileId);  // 추후 타운아이디추가되면 넣을것
        await marketOrderDoc.SaveAsync(cancellationToken); // create
        

        return marketOrderDoc.To<MarketOrderResponse>();
    }
}
