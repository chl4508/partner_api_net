using CvFramework.Common;
using CvFramework.MongoDB.Documents;
using MongoDB.Bson;

namespace Colorverse.MetaAdmin.Documents;

public class ItemDeleteRequestDoc : MongoDoc
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="itemid"></param>
    /// <returns></returns>
    public static ItemDeleteRequestDoc Create(Uuid itemid) => new(itemid);

    /// <summary>
    /// 
    /// </summary>
    public ItemDeleteRequestDoc() : base() { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="doc"></param>
    public ItemDeleteRequestDoc(BsonDocument doc) : base(doc) { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="uuid"></param>
    public ItemDeleteRequestDoc(Uuid uuid) : base(uuid) { }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override string GetDatabaseName() => "meta";

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override string GetCollectionName() => "item_delete_request";

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
}
