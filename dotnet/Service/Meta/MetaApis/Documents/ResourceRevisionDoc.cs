using CvFramework.Common;
using CvFramework.MongoDB.Documents;
using MongoDB.Bson;
using Colorverse.Common;
using CvFramework.MongoDB.Extensions;
using CvFramework.Bson;

namespace Colorverse.Meta.Documents;

/// <summary>
/// 
/// </summary>
public class ResourceRevisionDoc : MongoDocBase
{
    private Uuid _resourceid = null!;

    private int? _version = null!;

    /// <summary>
    /// 
    /// </summary>
    public Uuid ResouceId { get => _resourceid ?? throw new InvalidDataException("Undefined resourceid."); }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public int Version { get => _version ?? throw new InvalidDataException("Undefined version."); }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="domainType"></param>
    /// <param name="version"></param>
    /// <returns></returns>
    public static ResourceRevisionDoc Create(SvcDomainType domainType, int version) => new (UuidGenerater.NewUuid(domainType), version);

    /// <summary>
    /// 
    /// </summary>
    public ResourceRevisionDoc() : base(){ }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="doc"></param>
    public ResourceRevisionDoc(BsonDocument doc) : base(doc) { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="resourceid"></param>
    /// <param name="version"></param>
    /// <returns></returns>
    public ResourceRevisionDoc(Uuid resourceid, int version) : base() 
    { 
        _resourceid = resourceid;
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
    public override string GetCollectionName() => "resource_revision";

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override string? GetCacheKey() => string.Join(':', GetDatabaseName(), GetCollectionName(), _resourceid, _version);
    
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="defaultFilter"></param>
    protected override void OnConstructor(BsonDocument defaultFilter)
    {
        if(!defaultFilter.TryGetBsonDocuemnt("_id", out BsonDocument idDoc))
        {
            throw new BsonSerializationException("Invalid value _id.");
        }
        _resourceid = idDoc.GetUuid("resourceid");
        _version = idDoc.GetInt32("version");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override BsonDocument GetDefaultFilter()
    {
        BsonDocument filter = new()
        {
            { "_id", new BsonDocument
                {
                    {"resourceid" , _resourceid.Guid.ToBsonBinaryData()},
                    {"version",  _version},
                }
            }
        };
        return filter;
    }

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