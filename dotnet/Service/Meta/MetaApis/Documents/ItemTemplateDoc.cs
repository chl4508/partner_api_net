using Colorverse.Meta.Apis.DataTypes.Item;
using CvFramework.Common;
using CvFramework.MongoDB.Documents;
using CvFramework.MongoDB.Extensions;
using MongoDB.Bson;

namespace Colorverse.Meta.Documents;

/// <summary>
/// TemplateDoc
/// </summary>
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
    /// <param name="profileid"></param>
    /// <param name="worldid"></param>
    /// <param name="userid"></param>
    /// <param name="townid"></param>
    public void SetTemplate(ItemDoc itemDoc, Uuid profileid, Uuid worldid, Uuid userid, Uuid townid)
    {
        Set("creatorid", profileid.Guid.ToBsonBinaryData());
        Set("profileid", profileid.Guid.ToBsonBinaryData());
        Set("userid", userid.Guid.ToBsonBinaryData());
        ////Set("twonid", townid.Guid.ToBsonBinaryData());
        Set("worldid", worldid.Guid.ToBsonBinaryData());
        Set("asset_type", 1); // 아이템은 1
        Set("txt.title.ko", itemDoc.GetString("txt","title","ko"));
        if (itemDoc.GetString("txt", "desc", "ko") != null)
        {
            Set("txt.desc.ko", itemDoc.GetString("txt", "desc", "ko"));
        }
        if (itemDoc.GetBsonArray("txt","hashtag") != null)
        {
            Set("txt.hashtag", itemDoc.GetBsonArray("txt", "hashtag"));
        }

        if(itemDoc.GetString("resoruce", "thumbnail") != null)
        {
            Set("resource.thumbnail", itemDoc.GetString("resoruce", "thumbnail"));
        }
        if(itemDoc.GetString("resource", "preview") != null)
        {
            Set("resource.preview", itemDoc.GetString("resource", "preview"));
        }
        Set("option.version", itemDoc.GetInt32("option", "version"));
        if (itemDoc.GetBsonArray("option", "category") != null)
        {
            Set("option.category", itemDoc.GetBsonArray("option", "category"));
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

    /// <summary>
    /// UpdateTemplate
    /// </summary>
    /// <param name="request"></param>
    public void UpdateTemplate(UpdateItemRequest request)
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
        if(request.Option != null)
        {
            Set("option.version", request.Option.Version);
            if (request.Option.Price != null)
            {
                Set("option.price", new BsonDocument { { "type", request.Option.Price.Type }, { "amount", request.Option.Price.Amount } });
            }
        }
        if (request.Resource != null)
        {
            Set("resource.thumbnail", request.Resource.Thumbnail);
        }
        
    }

}
