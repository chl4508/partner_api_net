using Colorverse.Common;
using CvFramework.Bson;
using CvFramework.Common;
using CvFramework.MongoDB.Documents;
using CvFramework.MongoDB.Extensions;
using MongoDB.Bson;

namespace Colorverse.MetaAdmin.Documents;

public class ItemRevisionDoc : MongoDocBase
{
    private Uuid _itemid = null!;

    private int? _version = null!;

    /// <summary>
    /// 
    /// </summary>
    public Uuid Itemid { get => _itemid ?? throw new InvalidDataException("Undefined Itemid."); }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public int Version { get => _version ?? throw new InvalidDataException("Undefined version."); }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="itemid"></param>
    /// <param name="version"></param>
    /// <returns></returns>
    public static ItemRevisionDoc Create(Uuid itemid, int version) => new(itemid, version);

    /// <summary>
    /// 
    /// </summary>
    public ItemRevisionDoc() : base() { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="doc"></param>
    public ItemRevisionDoc(BsonDocument doc) : base(doc) { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="itemid"></param>
    /// <param name="version"></param>
    /// <returns></returns>
    public ItemRevisionDoc(Uuid itemid, int version) : base()
    {
        _itemid = itemid;
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
    public override string GetCollectionName() => "item_revision";

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    ////public override string? GetCacheKey() => string.Join(':', GetDatabaseName(), GetCollectionName(), Itemid, Version);
    public override string? GetCacheKey() => null;

    /// <summary>
    /// 
    /// </summary>
    public override void OnCreate()
    {
        Set("_id", UuidGenerater.NewUuid(SvcDomainType.MetaItemRevison).ToBsonBinaryData());
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
        _itemid = defaultFilter.GetUuid("itemid");
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
            {"itemid" , _itemid.Guid.ToBsonBinaryData()},
            {"version",  _version}
        };
        return filter;
    }

    /// <summary>
    /// SetItemRevision
    /// </summary>
    /// <param name="status"></param>
    /// <param name="manifest"></param>
    public void SetItemRevision(int status, BsonDocument? manifest)
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
