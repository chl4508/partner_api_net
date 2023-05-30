using CvFramework.Common;
using CvFramework.MongoDB.Documents;
using MongoDB.Bson;

namespace Colorverse.Meta.Documents;

public class AssetDeleteRequestDoc : MongoDoc
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="assetid"></param>
    /// <returns></returns>
    public static AssetDeleteRequestDoc Create(Uuid assetid) => new(assetid);

    /// <summary>
    /// 
    /// </summary>
    public AssetDeleteRequestDoc() : base() { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="doc"></param>
    public AssetDeleteRequestDoc(BsonDocument doc) : base(doc) { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="uuid"></param>
    public AssetDeleteRequestDoc(Uuid uuid) : base(uuid) { }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override string GetDatabaseName() => "meta";

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override string GetCollectionName() => "asset_delete_request";

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
