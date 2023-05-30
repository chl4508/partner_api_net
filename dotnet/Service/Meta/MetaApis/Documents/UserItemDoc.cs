using Colorverse.Common;
using CvFramework.Bson;
using CvFramework.Common;
using CvFramework.MongoDB.Documents;
using CvFramework.MongoDB.Extensions;
using MongoDB.Bson;

namespace Colorverse.Meta.Documents;

/// <summary>
/// UserItemDoc
/// </summary>
public class UserItemDoc : MongoDocBase
{
    private Uuid _itemid = null!;

    private Uuid _profileid = null!;

    /// <summary>
    /// 
    /// </summary>
    public Uuid Itemid { get => _itemid ?? throw new InvalidDataException("Undefined Itemid."); }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public Uuid Profileid { get => _profileid ?? throw new InvalidDataException("Undefined Profileid."); }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="itemid"></param>
    /// <param name="profileid"></param>
    /// <returns></returns>
    public static UserItemDoc Create(Uuid itemid, Uuid profileid) => new(itemid, profileid);

    /// <summary>
    /// 
    /// </summary>
    public UserItemDoc() : base() { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="doc"></param>
    public UserItemDoc(BsonDocument doc) : base(doc) { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="itemid"></param>
    /// <param name="profileid"></param>
    public UserItemDoc(Uuid itemid, Uuid profileid) : base()
    {
        _itemid = itemid;
        _profileid = profileid;
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
    public override string GetCollectionName() => "user_item";

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    ////public override string? GetCacheKey() => string.Join(':', GetDatabaseName(), GetCollectionName(), Profileid, Itemid);
    public override string? GetCacheKey() => null;

    /// <summary>
    /// 
    /// </summary>
    public override void OnCreate()
    {
        Set("_id", UuidGenerater.NewUuid(SvcDomainType.MetaUserItem).Guid.ToBsonBinaryData());
        CurrentDate("stat.created");
        CurrentDate("stat.updated");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="defaultFilter"></param>
    /// <exception cref="NotImplementedException"></exception>
    protected override void OnConstructor(BsonDocument defaultFilter)
    {
        _profileid = defaultFilter.GetUuid("profileid");
        _itemid = defaultFilter.GetUuid("itemid");
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
            {"profileid" , _profileid.Guid.ToBsonBinaryData()},
            {"itemid",  _itemid.Guid.ToBsonBinaryData()}
        };
        return filter;
    }

    /// <summary>
    /// 
    /// </summary>
    public override void OnUpdate()
    {
        CurrentDate("stat.updated");
    }

    /// <summary>
    /// SetUserItem
    /// </summary>
    /// <param name="userid"></param>
    /// <param name="worldid"></param>
    /// <param name="category"></param>
    public void SetUserItem(Uuid userid, Uuid worldid, BsonArray category)
    {
        Set("userid", userid.Guid.ToBsonBinaryData());
        Set("worldid", worldid.Guid.ToBsonBinaryData());
        Set("category", category);
    }

    /// <summary>
    /// SetQuantity
    /// </summary>
    /// <param name="quantity"></param>
    public void SetQuantity(int quantity)
    {
        Set("option.quantity", quantity);
    }
}
