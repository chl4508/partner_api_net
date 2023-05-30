using Colorverse.Common;
using CvFramework.Bson;
using CvFramework.Common;
using CvFramework.MongoDB.Documents;
using CvFramework.MongoDB.Extensions;
using MongoDB.Bson;

namespace Colorverse.MetaAdmin.Documents;

public class AssetRevisionDoc : MongoDocBase
{
    private Uuid _assetid = null!;

    private int? _version = null!;

    /// <summary>
    /// 
    /// </summary>
    public Uuid Assetid { get => _assetid ?? throw new InvalidDataException("Undefined Assetid."); }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public int Version { get => _version ?? throw new InvalidDataException("Undefined version."); }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="assetid"></param>
    /// <param name="version"></param>
    /// <returns></returns>
    public static AssetRevisionDoc Create(Uuid assetid, int version) => new(assetid, version);

    /// <summary>
    /// 
    /// </summary>
    public AssetRevisionDoc() : base() { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="doc"></param>
    public AssetRevisionDoc(BsonDocument doc) : base(doc) { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="assetid"></param>
    /// <param name="version"></param>
    /// <returns></returns>
    public AssetRevisionDoc(Uuid assetid, int version) : base()
    {
        _assetid = assetid;
        _version = version;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override string GetDatabaseName() => "meta";

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override string GetCollectionName() => "asset_revision";

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    ////public override string? GetCacheKey() => string.Join(':', GetDatabaseName(), GetCollectionName(), Assetid, Version);
    public override string? GetCacheKey() => null;

    /// <summary>
    /// 
    /// </summary>
    public override void OnCreate()
    {
        Set("_id", UuidGenerater.NewUuid(SvcDomainType.MetaAssetRevison).ToBsonBinaryData());
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
    /// 
    /// </summary>
    /// <param name="defaultFilter"></param>
    /// <exception cref="NotImplementedException"></exception>
    protected override void OnConstructor(BsonDocument defaultFilter)
    {
        _assetid = defaultFilter.GetUuid("assetid");
        _version = defaultFilter.GetInt32("version");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public override BsonDocument GetDefaultFilter()
    {
        BsonDocument filter = new()
        {
            {"assetid" , _assetid.Guid.ToBsonBinaryData()},
            {"version",  _version}
        };
        return filter;
    }

    /// <summary>
    /// SetAseetRevision
    /// </summary>
    /// <param name="status"></param>
    public void SetAssetRevision(int status)
    {
        Set("status", status);
    }

    /// <summary>
    /// SetAseetRevision
    /// </summary>
    /// <param name="status"></param>
    /// <param name="manifest"></param>
    public void SetAssetRevision(int status, BsonDocument? manifest)
    {
        Set("status", status);
        if (manifest != null)
        {
            Set("manifest", manifest);
        }
    }

    /// <summary>
    /// 판매 승인
    /// </summary>
    public void SaleApprovalRevision()
    {
        Set("status", 1);
    }
}
