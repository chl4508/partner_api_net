using Colorverse.Meta.Apis.DataTypes.Asset;
using Colorverse.Meta.Apis.DataTypes.Item;
using CvFramework.Common;
using CvFramework.MongoDB.Documents;
using CvFramework.MongoDB.Extensions;
using MongoDB.Bson;

namespace Colorverse.Meta.Documents;

/// <summary>
/// MarketDoc
/// </summary>
public class MarketProductDoc : MongoDoc
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="marketid"></param>
    /// <returns></returns>
    public static MarketProductDoc Create(Uuid marketid) => new(marketid);

    /// <summary>
    /// 
    /// </summary>
    public MarketProductDoc() : base() { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="doc"></param>
    public MarketProductDoc(BsonDocument doc) : base(doc) { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="uuid"></param>
    public MarketProductDoc(Uuid uuid) : base(uuid) { }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override string GetDatabaseName() => "meta";

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override string GetCollectionName() => "market_product";

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
    /// SetItemMarketProduct
    /// </summary>
    /// <param name="itemDoc"></param>
    public void SetItemMarketProduct(ItemDoc itemDoc)
    {
        Set("type", 1); // 1: 아이템 , 2 : 애셋
        Set("profileid", itemDoc.GetBsonBinaryData("profileid"));
        if (itemDoc.GetNullableBsonBinaryData("userid") != null)
        {
            Set("userid", itemDoc.GetBsonBinaryData("userid"));
        }
        if(itemDoc.GetNullableBsonBinaryData("townid") != null)
        {
            Set("townid", itemDoc.GetBsonBinaryData("townid"));
        }
        Set("worldid", itemDoc.GetBsonBinaryData("worldid"));
        Set("status", itemDoc.GetInt32("option","sale_status"));
        Set("txt.title.ko", itemDoc.GetString("txt","title","ko"));
        if (itemDoc.GetNullableString("txt", "title", "ko") != null)
        {
            Set("txt.desc.ko", itemDoc.GetString("txt", "title", "ko"));
        }
        if (itemDoc.GetNullableBsonArray("txt", "hashtag") != null)
        {
            Set("txt.hashtag", itemDoc.GetBsonArray("txt", "hashtag"));
        }
        if (itemDoc.GetNullableString("resource", "thumbnail") != null)
        {
            Set("resource.thumbnail", itemDoc.GetString("resource", "thumbnail"));
        }
        if (itemDoc.GetNullableString("resource", "preview") != null)
        {
            Set("resource.preview", itemDoc.GetString("resource", "preview"));
        }
        Set("option.asset_type", itemDoc.GetInt32("asset_type"));
        Set("option.category", itemDoc.GetBsonArray("option","category"));
        Set("option.sale_accept", DateTime.UtcNow);
        Set("option.sale_version", itemDoc.GetInt32("option","sale_version"));
        Set("option.blind", itemDoc.GetBoolean("option", "blind"));
        Set("option.delete", itemDoc.GetBoolean("option", "delete"));
        Set("option.none_sale", itemDoc.GetBoolean("option", "none_sale"));

        if (itemDoc.GetNullableBsonDocument("option", "price") != null)
        {
            Set("option.price", new BsonDocument { { "type", itemDoc.GetInt32("option", "price", "type") }, { "amount", itemDoc.GetInt32("option", "price", "amount") } });
        }

    }

    /// <summary>
    /// SetAssetMarketProduct
    /// </summary>
    /// <param name="assetDoc"></param>
    public void SetAssetMarketProduct(AssetDoc assetDoc)
    {
        Set("type", 2); // 1: 아이템 , 2 : 애셋
        Set("profileid", assetDoc.GetBsonBinaryData("profileid"));
        if (assetDoc.GetNullableBsonBinaryData("userid") != null)
        {
            Set("userid", assetDoc.GetBsonBinaryData("userid"));
        }
        if (assetDoc.GetNullableBsonBinaryData("townid") != null)
        {
            Set("townid", assetDoc.GetBsonBinaryData("townid"));
        }
        Set("worldid", assetDoc.GetBsonBinaryData("worldid"));
        Set("status", assetDoc.GetInt32("option", "sale_status"));
        Set("txt.title.ko", assetDoc.GetString("txt", "title", "ko"));
        if (assetDoc.GetNullableString("txt", "title", "ko") != null)
        {
            Set("txt.desc.ko", assetDoc.GetString("txt", "title", "ko"));
        }
        if (assetDoc.GetNullableBsonArray("txt", "hashtag") != null)
        {
            Set("txt.hashtag", assetDoc.GetBsonArray("txt", "hashtag"));
        }
        if (assetDoc.GetNullableString("resource", "thumbnail") != null)
        {
            Set("resource.thumbnail", assetDoc.GetString("resource", "thumbnail"));
        }
       
        Set("option.asset_type", assetDoc.GetInt32("type"));
        Set("option.category", assetDoc.GetBsonArray("option", "category"));
        Set("option.sale_accept", DateTime.UtcNow);
        Set("option.sale_version", assetDoc.GetInt32("option", "sale_version"));
        Set("option.blind", assetDoc.GetBoolean("option", "blind"));
        Set("option.delete", assetDoc.GetBoolean("option", "delete"));
        Set("option.none_sale", assetDoc.GetBoolean("option", "none_sale"));

        if (assetDoc.GetNullableBsonDocument("option", "price") != null)
        {
            Set("option.price", new BsonDocument { { "type", assetDoc.GetInt32("option", "price", "type") }, { "amount", assetDoc.GetInt32("option", "price", "amount") } });
        }
    }

    /// <summary>
    /// SetItems
    /// </summary>
    /// <param name="type"></param>
    /// <param name="id"></param>
    /// <param name="quantity"></param>
    public void SetItems(int type, Uuid id, int quantity)
    {
        Set("option.items", new BsonArray { new BsonDocument { 
            { "type", type },
            { "id",  id.ToBsonBinaryData()},
            { "quantity", quantity }
        }});
    }

    /// <summary>
    /// UpdateMarketProduct
    /// </summary>
    public void UpdateItemMarketProduct()
    {
        Set("status", 3); // 판매상태 (0 : 없음, 1 : 판매심사중, 2: 판매허용, 3 : 판매중)
    }

    /// <summary>
    /// UpdateMarketProduct
    /// </summary>
    /// <param name="status"></param>
    /// <param name="option"></param>
    public void UpdateAssetMarketProduct(int status, SaleInspectionAssetOptionRequest option)
    {
        Set("status", status); // 판매상태 (0 : 없음, 1 : 판매심사중, 2: 판매허용, 3 : 판매중)
        Set("option.price", new BsonDocument { {"type", option.Price.Type}, {"amount", option.Price.Amount} });
    }

    /// <summary>
    /// UpdateMarketProduct
    /// </summary>
    public void UpdateAssetMarketProduct()
    {
        Set("status", 3); // 판매상태 (0 : 없음, 1 : 판매심사중, 2: 판매허용, 3 : 판매중)
    }

    /// <summary>
    /// UpdateItemParam
    /// </summary>
    /// <param name="request"></param>
    public void UpdatePriceMarket(UpdateItemRequest request)
    {
        
        if(request.Txt != null)
        {
            if(request.Txt.Desc != null && request.Txt.Desc.Ko != null) Set("txt.desc.ko", request.Txt.Desc.Ko);
            if(request.Txt.Hashtag != null)
            {
                Set("txt.hashtag", new BsonArray(request.Txt.Hashtag));
            }
        }
        
        if(request.Option != null && request.Option.Price != null)
        {
            Set("option.price", new BsonDocument { { "type", request.Option.Price.Type } , { "amount", request.Option.Price.Amount } });
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="prices"></param>
    public void UpdatePriceMarket(UpdateAssetPriceRequest[] prices)
    {
        var priceArray = new BsonArray();
        foreach (var price in prices)
        {
            priceArray.Add(new BsonDocument {
                    {"type", price.Type},       //재화타입
                    {"amount", price.Amount}    //판매가격
                });
        }
        Set("option.price", priceArray);
    }

    /// <summary>
    /// SaleStopStatus
    /// </summary>
    public void SaleStopStatus()
    {
        Set("status", 2); // 판매상태 (0 : 없음, 1 : 판매심사중, 2: 판매허용, 3 : 판매중)
        Unset("option.sale_start");
        Unset("option.sale_end");
    }

    /// <summary>
    /// 
    /// </summary>
    public void UpdateLiked()
    {
        Inc("stat.like", 1);
    }
    
    /// <summary>
    /// 
    /// </summary>
    public void UpdateCommented()
    {
        Inc("stat.comment", 1);
    }
}
