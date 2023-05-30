using Colorverse.Common;
using Colorverse.Meta.Apis.DataTypes.Market;
using CvFramework.Common;
using CvFramework.MongoDB.Documents;
using CvFramework.MongoDB.Extensions;
using MongoDB.Bson;

namespace Colorverse.Meta.Documents;

/// <summary>
/// MarketOrderDoc
/// </summary>
public class MarketOrderDoc : MongoDoc
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="domainType"></param>
    /// <returns></returns>
    public static MarketOrderDoc Create(SvcDomainType domainType) => new(UuidGenerater.NewUuid(domainType));

    /// <summary>
    /// 
    /// </summary>
    public MarketOrderDoc() : base() { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="doc"></param>
    public MarketOrderDoc(BsonDocument doc) : base(doc) { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="uuid"></param>
    public MarketOrderDoc(Uuid uuid) : base(uuid) { }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override string GetDatabaseName() => "meta";

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override string GetCollectionName() => "market_order";

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override string? GetCacheKey() => string.Join(':', GetDatabaseName(), GetCollectionName(), Id);

    /// <summary>
    /// 
    /// </summary>
    public override void OnCreate()
    {
        CurrentDate("stat.created");
        CurrentDate("stat.updated");
    }

    /// <summary>
    /// 
    /// </summary>
    public override void OnUpdate()
    {
        CurrentDate("stat.updated");
    }

    /// <summary>
    /// SetMarketOrder
    /// </summary>
    /// <param name="market"></param>
    /// <param name="totalPrice"></param>
    /// <param name="userid"></param>
    /// <param name="townid"></param>
    /// <param name="profileid"></param>
    public void SetMarketOrder(MarketResponse[] market, Dictionary<int, int> totalPrice, Uuid? userid, Uuid? townid, Uuid profileid)
    {
        if(userid != null) Set("userid", userid.ToBsonBinaryData());
        if(townid != null) Set("townid", townid.ToBsonBinaryData());
        Set("profileid", profileid.ToBsonBinaryData());
        Set("state", 2); // 1: request, 2: paid, 3: delivered, 4: cancel
        Set("txt.title", market[0].Txt.Title.Ko);
        var array = new BsonArray();
        foreach (var category in market[0].Option.Category)
        {
            array.Add(category);
        }
        Set("option.category", array);

        var paymentarray = new BsonArray();
        for(var i=1; i <= 4; i++)
        {
            if (totalPrice.ContainsKey(i) && totalPrice[i] != 0)
            {
                paymentarray.Add( new BsonDocument { { "type", i}, { "amount", totalPrice[i]}});
            }
        }
        Set("option.payment", paymentarray);

        var productArray = new BsonArray();
        
        for(int i = 0; i< market.Length; i++)
        {
            var cateArray = new BsonArray();
            foreach (var category in market[i].Option.Category)
            {
                cateArray.Add(category);
            }

            var itemsArray = new BsonArray();
            foreach (var item in market[i].Option.Items)
            {
                if (item != null)
                {
                    itemsArray.Add(new BsonDocument {
                        { "type", item.Type },
                        { "id",  item.Id.ToBsonBinaryData()},
                        { "quantity", item.Quantity }
                    });
                }
            }

            productArray.Add(new BsonDocument
            {
                {"id",  market[i].Id.ToBsonBinaryData()},
                {"quantity",  1},
                {"title",  market[i].Txt.Title.Ko},
                {"category", cateArray },
                {"payment_type", market[i].Option.Price.Type },
                {"payment_amount", market[i].Option.Price.Amount },
                {"item", itemsArray },
            });
        }
        Set("product", productArray);
    }
}
