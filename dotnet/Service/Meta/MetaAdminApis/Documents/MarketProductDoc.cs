using Colorverse.MetaAdmin.Apis.DataTypes.Asset;
using Colorverse.MetaAdmin.Apis.DataTypes.Item;
using Colorverse.MetaAdmin.DataTypes.Excel;
using Colorverse.MetaAdmin.Excel;
using CvFramework.Common;
using CvFramework.MongoDB.Documents;
using CvFramework.MongoDB.Extensions;
using MongoDB.Bson;

namespace Colorverse.MetaAdmin.Documents;

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
    /// 마켓 아이템 수정
    /// </summary>
    /// <param name="request"></param>
    /// <param name="manifest"></param>
    public void UpdateMarket(UpdateItemRequest request, BsonDocument? manifest)
    {
        if (request.Txt != null)
        {
            if (request.Txt.Title != null && request.Txt.Title.Ko != null)
            {
                Set("txt.title.ko", request.Txt.Title.Ko);
            }
            if (request.Txt.Desc != null && request.Txt.Desc.Ko != null)
            {
                Set("txt.desc.ko", request.Txt.Desc.Ko);
            }
            if (request.Txt.Hashtag != null)
            {
                Set("txt.hashtag", new BsonArray(request.Txt.Hashtag));
            }
        }
        if (manifest != null)
        {
            Set("manifest", manifest);
        }
        if (request.Option != null)
        {
            if (request.Option.Version > 0)
            {
                Set("option.version", request.Option.Version);
            }
            if (request.Option.Price != null)
            {
                if (request.Option.Price.Type != null)
                {
                    Set("option.price.type", request.Option.Price.Type);
                }
                if (request.Option.Price.Amount != null)
                {
                    Set("option.price.amount", request.Option.Price.Amount);
                }
            }
        }
        if (request.Resource != null)
        {
            if (request.Resource.Thumbnail != null)
            {
                Set("option.resoruce.thumbnail", request.Resource.Thumbnail);
            }
            if (request.Resource.Preview != null)
            {
                Set("option.resoruce.preview", request.Resource.Preview);
            }
        }
    }

    /// <summary>
    /// 마켓 애셋 수정
    /// </summary>
    /// <param name="request"></param>
    /// <param name="manifest"></param>
    public void UpdateMarket(UpdateAssetRequest request, BsonDocument? manifest)
    {
        if (request.Txt != null)
        {
            if (request.Txt.Title != null && request.Txt.Title.Ko != null)
            {
                Set("txt.title.ko", request.Txt.Title.Ko);
            }
            if (request.Txt.Desc != null && request.Txt.Desc.Ko != null)
            {
                Set("txt.desc.ko", request.Txt.Desc.Ko);
            }
            if (request.Txt.Hashtag != null)
            {
                Set("txt.hashtag", new BsonArray(request.Txt.Hashtag));
            }
        }
        if (manifest != null)
        {
            Set("manifest", manifest);
        }
        if (request.Option != null)
        {
            if (request.Option.Version > 0)
            {
                Set("option.version", request.Option.Version);
            }
            if (request.Option.Price != null)
            {
                if (request.Option.Price.Type != null)
                {
                    Set("option.price.type", request.Option.Price.Type);
                }
                if (request.Option.Price.Amount != null)
                {
                    Set("option.price.amount", request.Option.Price.Amount);
                }
            }
        }
        if (request.Resource != null && request.Resource.Thumbnail != null)
        {
            Set("option.resoruce.thumbnail", request.Resource.Thumbnail);
        }
    }

    /// <summary>
    /// 사후심사 승인
    /// </summary>
    public void SaleApprovalMarketProduct()
    {
        Set("option.blind", false);
    }

    /// <summary>
    /// 아이템 심사 승인
    /// </summary>
    /// <param name="itemDoc"></param>
    /// <param name="saleAccept"></param>
    /// <param name="saleStatus"></param>
    public void SaleApprovalMarketProduct(ItemDoc itemDoc, DateTime saleAccept, int saleStatus)
    {
        Set("type", 1);
        Set("profileid", itemDoc.GetUuid("profileid").ToBsonBinaryData());
        if (itemDoc.GetUuid("userid") != null)
        {
            Set("userid", itemDoc.GetUuid("userid").ToBsonBinaryData());
        }
        if (itemDoc.GetUuid("townid") != null)
        {
            Set("townid", itemDoc.GetUuid("townid").ToBsonBinaryData());
        }
        Set("worldid", itemDoc.GetUuid("worldid").ToBsonBinaryData());
        Set("status", saleStatus); // 판매허용 또는 판매중
        Set("txt.titile.ko", itemDoc.GetString("txt","title","ko"));
        if (itemDoc.GetString("txt", "desc", "ko") != null)
        {
            Set("txt.desc.ko", itemDoc.GetString("txt", "desc", "ko"));
        }
        if (itemDoc.GetBsonArray("txt", "hashtag") != null)
        {
            Set("txt.hashtag", itemDoc.GetBsonArray("txt", "hashtag"));
        }

        if (itemDoc.GetString("resource", "thumbnail") != null)
        {
            Set("resource.thumbnail", itemDoc.GetString("resource", "thumbnail"));
        }
        if(itemDoc.GetString("resource", "preview")  != null)
        {
            Set("resource.preview", itemDoc.GetString("resource", "preview"));
        }

        Set("option.sale_accept", saleAccept); //승인일
        if (itemDoc.GetNullableDateTime("option", "sale_start") != null)
        {
            Set("option.sale_start", itemDoc.GetNullableDateTime("option", "sale_start"));
        }    
        if (itemDoc.GetNullableDateTime("option", "sale_end") != null)
        {
            Set("option.sale_end", itemDoc.GetNullableDateTime("option", "sale_end"));
        }
        Set("option.sale_version", itemDoc.GetInt32("option", "sale_version")); //sale_version 
        Set("option.blind", false);
        Set("option.delete", false);
        Set("option.none_sale", itemDoc.GetBoolean("option", "none_sale"));
        Set("option.asset_type", itemDoc.GetInt32("asset_type"));
        Set("option.category", itemDoc.GetBsonArray("option", "category"));

        Set("option.price", new BsonDocument { { "type", itemDoc.GetInt32("option", "price", "type") }, { "amount", itemDoc.GetInt32("option", "price", "amount") } });

        var itemsArray = new BsonArray()
            {
                new BsonDocument { {"type", 1 }, {"id", itemDoc.GetUuid("_id").ToBsonBinaryData() }, {"quantity", 1 } }
            };
        Set("option.items", itemsArray);
    }

    /// <summary>
    /// 애셋 심사 승인
    /// </summary>
    /// <param name="assetDoc"></param>
    /// <param name="saleAccept"></param>
    /// <param name="saleStatus"></param>
    public void SaleApprovalMarketProduct(AssetDoc assetDoc, DateTime saleAccept, int saleStatus)
    {
        Set("type", 2);
        Set("profileid", assetDoc.GetUuid("profileid").ToBsonBinaryData());
        if (assetDoc.GetUuid("userid") != null)
        {
            Set("userid", assetDoc.GetUuid("userid").ToBsonBinaryData());
        }
        if (assetDoc.GetUuid("townid") != null)
        {
            Set("townid", assetDoc.GetUuid("townid").ToBsonBinaryData());
        }
        Set("worldid", assetDoc.GetUuid("worldid").ToBsonBinaryData());
        Set("status", saleStatus); // 판매허용 또는 판매중
        Set("txt.titile.ko", assetDoc.GetString("txt", "title", "ko"));
        if (assetDoc.GetString("txt", "desc", "ko") != null)
        {
            Set("txt.desc.ko", assetDoc.GetString("txt", "desc", "ko"));
        }
        if (assetDoc.GetBsonArray("txt", "hashtag") != null)
        {
            Set("txt.hashtag", assetDoc.GetBsonArray("txt", "hashtag"));
        }

        if (assetDoc.GetString("resource", "thumbnail") != null)
        {
            Set("resource.thumbnail", assetDoc.GetString("resource", "thumbnail"));
        }
       
        Set("option.sale_accept", saleAccept); //승인일
        if (assetDoc.GetNullableDateTime("option", "sale_start") != null)
        {
            Set("option.sale_start", assetDoc.GetNullableDateTime("option", "sale_start"));
        }
        if (assetDoc.GetNullableDateTime("option", "sale_end") != null)
        {
            Set("option.sale_end", assetDoc.GetNullableDateTime("option", "sale_end"));
        }
        Set("option.sale_version", assetDoc.GetInt32("option", "sale_version")); //sale_version 
        Set("option.blind", false);
        Set("option.delete", false);
        Set("option.none_sale", assetDoc.GetBoolean("option", "none_sale"));
        Set("option.asset_type", assetDoc.GetInt32("asset_type"));
        Set("option.category", assetDoc.GetBsonArray("option", "category"));

        Set("option.price", new BsonDocument { { "type", assetDoc.GetInt32("option", "price", "type") }, { "amount", assetDoc.GetInt32("option", "price", "amount") } });

        var itemsArray = new BsonArray()
            {
                new BsonDocument { {"type", 1 }, {"id", assetDoc.GetUuid("_id").ToBsonBinaryData() }, {"quantity", 1 } }
            };
        Set("option.items", itemsArray);
    }

    /// <summary>
    /// 마켓 삭제요청
    /// </summary>
    public void DeleteMarketProduct()
    {
        Set("option.blind", true);
        Set("option.delete", true);
    }

    /// <summary>
    /// 마켓 삭제요청 취소
    /// </summary>
    public void DeleteCancelMarketProduct()
    {
        Set("option.blind", false);
        Set("option.delete", false);
    }

    /// <summary>
    /// 워크 시트 마켓 생성
    /// </summary>
    /// <param name="param"></param>
    /// <param name="targetSheet"></param>
    public void SetWorksheetMarket(CreateExcelRequest param, string targetSheet)
    {
        if (targetSheet.Equals("item"))
        {
            Set("type", 1); // 아이템/애셋 구분
        }
        else
        {
            Set("type", 2); // 아이템/애셋 구분
        }
        Set("profileid", param.Profileid.ToBsonBinaryData());
        Set("userid", param.Userid.ToBsonBinaryData());
        Set("worldid", param.Worldid.ToBsonBinaryData());
        Set("status", param.SaleStatus);
        Set("txt.title.ko", param.Title);
        Set("txt.desc.ko", param.Desc);
        Set("txt.hashtag", new BsonArray(param.Hashtag));
        CurrentDate("option.sale_accept"); // 판매심사승인일
        Set("option.instant_sale", true);
        Set("option.sale_version", param.Version);
        Set("option.blind", false); //블라인드
        Set("option.delete", false); //delete
        if (param.PriceType == 1)
        {
            Set("option.none_sale", true); //비매품여부
        }
        else
        {
            Set("option.none_sale", false); //비매품여부
        }

        if (targetSheet.Equals("item"))
        {
            Set("option.asset_type", 1);
        }
        else
        {
            Set("option.asset_type", param.AssetType);
        }
        Set("option.category", new BsonArray(param.Category));
        Set("option.price", new BsonDocument() { { "type", param.PriceType }, { "amount", param.PriceAmount } });

        var itemArray = new BsonArray();
        if (targetSheet.Equals("item"))
        {
            itemArray.Add(new BsonDocument() { { "type", 1 }, { "id", new BsonBinaryData(Uuid.FromBase62(param.Targetid), GuidRepresentation.Standard) }, { "quantity", 1 } });
        }
        else
        {
            itemArray.Add(new BsonDocument() { { "type", param.AssetType }, { "id", new BsonBinaryData(Uuid.FromBase62(param.Targetid), GuidRepresentation.Standard) }, { "quantity", 1 } });
        }
        Set("option.items", itemArray);
    }

    /// <summary>
    /// 워크 시트 마켓 생성
    /// </summary>
    /// <param name="param"></param>
    public void SetWorksheetMarket(ItemRow param)
    {
        Set("type", 1); // 아이템/애셋 구분
        Set("profileid", Uuid.FromBase62(param.Profileid).ToBsonBinaryData());
        Set("userid", Uuid.FromBase62(param.Userid).ToBsonBinaryData());
        Set("worldid", Uuid.FromBase62(param.Worldid).ToBsonBinaryData());
        Set("status", Convert.ToInt32(param.SaleStatus));
        Set("txt.title.ko", param.Title);
        if (param.Desc != null)
        {
            Set("txt.desc.ko", param.Desc);
        }
        if (param.Hashtag != null)
        {
            var hashTagArray = param.Hashtag.Split(",");
            Set("txt.hashtag", new BsonArray(hashTagArray));
        }
        CurrentDate("option.sale_accept"); // 판매심사승인일
        
        Set("option.sale_version", param.Version);
        Set("option.blind", false); //블라인드
        Set("option.delete", false); //delete
        if (param.PriceType!.Equals("1"))
        {
            Set("option.none_sale", true); //비매품여부
        }
        else
        {
            Set("option.none_sale", false); //비매품여부
        }

        Set("option.asset_type", Convert.ToInt32(param.AssetType));
        string[] categoryArray = new string[3] { param.Category1!.ToString(),
                                                 param.Category1!.ToString() + "," + param.Category2!.ToString(),
                                                 param.Category1!.ToString() + "," + param.Category2!.ToString() + "," + param.Category3!.ToString() };
        Set("option.category", new BsonArray(categoryArray));
        Set("option.price", new BsonDocument() { { "type", Convert.ToInt32(param.PriceType) }, { "amount", Convert.ToInt32(param.PriceAmount) } });

        var itemArray = new BsonArray
        {
            new BsonDocument() { { "type", 1 }, { "id", new BsonBinaryData(Uuid.FromBase62(param.Itemid!), GuidRepresentation.Standard) }, { "quantity", 1 } }
        };
        Set("option.items", itemArray);
    }

    /// <summary>
    /// 워크 시트 마켓 생성
    /// </summary>
    /// <param name="param"></param>
    public void SetWorksheetMarket(AssetRow param)
    {
        Set("type", 2); // 아이템/애셋 구분
        Set("profileid", Uuid.FromBase62(param.Profileid).ToBsonBinaryData());
        Set("userid", Uuid.FromBase62(param.Userid).ToBsonBinaryData());
        Set("worldid", Uuid.FromBase62(param.Worldid).ToBsonBinaryData());
        Set("status", Convert.ToInt32(param.SaleStatus));
        Set("txt.title.ko", param.Title);
        if (param.Desc != null)
        {
            Set("txt.desc.ko", param.Desc);
        }
        if (param.Hashtag != null)
        {
            var hashTagArray = param.Hashtag.Split(",");
            Set("txt.hashtag", new BsonArray(hashTagArray));
        }
        CurrentDate("option.sale_accept"); // 판매심사승인일
        
        Set("option.sale_version", param.Version);
        Set("option.blind", false); //블라인드
        Set("option.delete", false); //delete
        if (param.PriceType!.Equals("1"))
        {
            Set("option.none_sale", true); //비매품여부
        }
        else
        {
            Set("option.none_sale", false); //비매품여부
        }

        Set("option.asset_type", Convert.ToInt32(param.AssetType));
        string[] categoryArray = new string[3] { param.Category1!.ToString(),
                                                 param.Category1!.ToString() + "," + param.Category2!.ToString(),
                                                 param.Category1!.ToString() + "," + param.Category2!.ToString() + "," + param.Category3!.ToString() };
        Set("option.category", new BsonArray(categoryArray));
        Set("option.price", new BsonDocument() { { "type", Convert.ToInt32(param.PriceType) }, { "amount", Convert.ToInt32(param.PriceAmount) } });

        var assetArray = new BsonArray
        {
            new BsonDocument() { { "type", Convert.ToInt32(param.PriceType) }, { "id", new BsonBinaryData(Uuid.FromBase62(param.Assetid!), GuidRepresentation.Standard) }, { "quantity", 1 } }
        };
        Set("option.items", assetArray);
    }

    /// <summary>
    /// 워크시트 판매허용으로 변경
    /// </summary>
    public void DeleteWorksheetMarket()
    {
        Set("status", 2);
    }

}
