using CvFramework.Common;
using CvFramework.MongoDB.Documents;
using CvFramework.MongoDB.Extensions;
using MongoDB.Bson;

namespace Colorverse.MetaAdmin.Documents;

public class AssetDeleteDoc : MongoDoc
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="assetid"></param>
    /// <returns></returns>
    public static AssetDeleteDoc Create(Uuid assetid) => new(assetid);

    /// <summary>
    /// 
    /// </summary>
    public AssetDeleteDoc() : base() { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="doc"></param>
    public AssetDeleteDoc(BsonDocument doc) : base(doc) { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="uuid"></param>
    public AssetDeleteDoc(Uuid uuid) : base(uuid) { }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override string GetDatabaseName() => "meta";

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override string GetCollectionName() => "asset_delete";

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
    /// 삭제 collection 추가
    /// </summary>
    /// <param name="assetDoc"></param>
    public void SetDelete(AssetDoc assetDoc)
    {
        Set("creatorid", assetDoc.GetUuid("creatorid").ToBsonBinaryData());
        Set("profileid", assetDoc.GetUuid("profileid").ToBsonBinaryData());
        if (assetDoc.GetNullableUuid("userid") != null) { Set("userid", assetDoc.GetUuid("userid").ToBsonBinaryData()); }
        //Set("townid", userid.ToBsonBinaryData()); // 타운아이디 나중에 생기면 넣을것
        Set("worldid", assetDoc.GetUuid("worldid").ToBsonBinaryData());
        Set("type", assetDoc.GetInt32("type"));
        Set("txt.title.ko", assetDoc.GetString("txt","title","ko"));
        if (assetDoc.GetNullableString("txt","title","desc") != null)
        {
            Set("txt.desc.ko", assetDoc.GetString("txt", "title", "desc"));
        }
        if (assetDoc.GetNullableBsonArray("txt","hashtag") != null)
        {
            Set("txt.hashtag", assetDoc.GetBsonArray("txt", "hashtag"));
        }
        else
        {
            Set("txt.hashtag", new BsonArray());
        }
        if (assetDoc.GetNullableString("resource","thumbnail") != null)
        {
            Set("resource.thumbnail", assetDoc.GetString("resource","thumbnail"));
        }
        if(assetDoc.GetNullableDateTime("option", "sale_accept") != null) Set("option.sale_accpet", assetDoc.GetNullableDateTime("option", "sale_accept"));
        if(assetDoc.GetNullableDateTime("option", "sale_start") != null) Set("option.sale_start", assetDoc.GetNullableDateTime("option", "sale_start"));
        if(assetDoc.GetNullableDateTime("option", "sale_end") != null) Set("option.sale_end", assetDoc.GetNullableDateTime("option", "sale_end"));
        Set("option.version", assetDoc.GetInt32("option", "version"));
        Set("option.sale_version", assetDoc.GetInt32("option", "sale_version"));
        Set("option.ready_status", assetDoc.GetInt32("option", "ready_status")); // 준비상태 (0 : 제작중, 1 : 저장됨)
        Set("option.sale_status", assetDoc.GetInt32("option", "sale_status")); // 판매상태 (0 : 없음, 1 : 판매심사중, 2: 판매허용, 3 : 판매중)
        Set("option.sale_review_status", 3); // 판매심사상태 (0 : 없음, 1 : 판매심사요청, 2 : 판매심사중, 3 : 판매심사반려, 4 : 판매심사승인)
        Set("option.judge_status", assetDoc.GetInt32("option", "judge_status")); // 사후심사상태(0 : 없음, 1: 사후심사요청, 2 : 사후심사중, 3: 사후심사반려, 4 : 사후심사승인)
        Set("option.blind", true);
        Set("option.delete", true);

        if (assetDoc.GetNullableBsonArray("option", "category") != null)
        {
            Set("option.category", assetDoc.GetBsonArray("option", "category"));
        }
        
        if (assetDoc.GetNullableBsonDocument("option", "price") != null)
        {
            Set("option.price", new BsonDocument { { "type", assetDoc.GetInt32("option", "price", "type") }, { "amount", assetDoc.GetInt32("option", "price", "amount") } });
        }
        Set("option.none_sale", assetDoc.GetBoolean("option", "none_sale"));
    }
}
