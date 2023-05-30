using Colorverse.MetaAdmin.Apis.DataTypes.Item;
using CvFramework.Common;
using CvFramework.MongoDB.Documents;
using CvFramework.MongoDB.Extensions;
using MongoDB.Bson;

namespace Colorverse.MetaAdmin.Documents;

public class ItemTemplateDoc : MongoDoc
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="templateid"></param>
    /// <returns></returns>
    public static ItemTemplateDoc Create(Uuid templateid) => new(templateid);

    /// <summary>
    /// 
    /// </summary>
    public ItemTemplateDoc() : base() { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="doc"></param>
    public ItemTemplateDoc(BsonDocument doc) : base(doc) { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="uuid"></param>
    public ItemTemplateDoc(Uuid uuid) : base(uuid) { }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override string GetDatabaseName() => "meta";

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override string GetCollectionName() => "item_template";

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
    /// SetTemplate
    /// </summary>
    /// <param name="itemDoc"></param>
    public void SetTemplate(ItemDoc itemDoc)
    {
        Set("creatorid", itemDoc.GetUuid("profileid").ToBsonBinaryData());
        Set("profileid", itemDoc.GetUuid("profileid").ToBsonBinaryData());
        if(itemDoc.GetNullableUuid("userid") != null) Set("userid", itemDoc.GetUuid("userid").ToBsonBinaryData());
        ////if(townid !=null) Set("twonid", townid.ToBsonBinaryData());
        Set("worldid", itemDoc.GetUuid("worldid").ToBsonBinaryData());
        Set("asset_type", 1); // 아이템은 1
        Set("txt.title.ko", itemDoc.GetString("txt","title","ko"));
        if (itemDoc.GetNullableString("txt", "title", "desc") != null)
        {
            Set("txt.desc.ko", itemDoc.GetString("txt", "title", "desc"));
        }
        if (itemDoc.GetNullableBsonArray("txt", "hashtag") != null)
        {
            Set("txt.hashtag", itemDoc.GetBsonArray("txt", "hashtag"));
        }
        
        Set("resource.thumbnail", itemDoc.GetString("resource","thumbnail"));
        Set("option.version", itemDoc.GetInt32("option","version"));
        if (itemDoc.GetNullableBsonArray("option", "category") != null)
        {
            Set("option.category", itemDoc.GetBsonArray("option", "category"));
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    public void UpdateTempalte(UpdateItemRequest request)
    {
        if (request.Txt != null)
        {
            if (request.Txt.Title != null)
            {
                Set("txt.title.ko", request.Txt.Title.Ko);
            }
            if (request.Txt.Desc != null)
            {
                Set("txt.desc.ko", request.Txt.Desc.Ko);
            }
            if (request.Txt.Hashtag != null)
            {
                Set("txt.hashtag", new BsonArray(request.Txt.Hashtag));
            }
        }
        if (request.Option != null && request.Option.Version > 0)
        {
            Set("option.version", request.Option.Version);
        }
        if (request.Resource != null)
        {
            if (request.Resource.Thumbnail != null)
            {
                Set("resource.thumbnail", request.Resource.Thumbnail);
            }
            if(request.Resource.Preview != null)
            {
                Set("resource.preview", request.Resource.Preview);
            }
        }
    }

    /// <summary>
    /// UpdateSaleTemplate
    /// </summary>
    /// <param name="status"></param>
    public void UpdateSaleTemplate(int status)
    {
        Set("status", status); // 판매상태 (없음, 판매심사중, 판매불가, 판매허용, 판매중, 판매중지)
    }
}
