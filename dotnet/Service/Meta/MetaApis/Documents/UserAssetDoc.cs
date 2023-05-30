using Colorverse.Common;
using CvFramework.Bson;
using CvFramework.Common;
using CvFramework.MongoDB.Documents;
using CvFramework.MongoDB.Extensions;
using MongoDB.Bson;

namespace Colorverse.Meta.Documents;

/// <summary>
/// UserAssetDoc
/// </summary>
public class UserAssetDoc : MongoDocBase
{
    private Uuid _assetid = null!;

    private Uuid _profileid = null!;

    /// <summary>
    /// 
    /// </summary>
    public Uuid Assetid { get => _assetid ?? throw new InvalidDataException("Undefined Assetid."); }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public Uuid Profileid { get => _profileid ?? throw new InvalidDataException("Undefined Profileid."); }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="assetid"></param>
    /// <param name="profileid"></param>
    /// <returns></returns>
    public static UserAssetDoc Create(Uuid assetid, Uuid profileid) => new(assetid, profileid);

    /// <summary>
    /// 
    /// </summary>
    public UserAssetDoc() : base() { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="doc"></param>
    public UserAssetDoc(BsonDocument doc) : base(doc) { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="assetid"></param>
    /// <param name="profileid"></param>
    public UserAssetDoc(Uuid assetid, Uuid profileid) : base()
    {
        _assetid = assetid;
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
    public override string GetCollectionName() => "user_asset";

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override string? GetCacheKey() => string.Join(':', GetDatabaseName(), GetCollectionName(), Profileid, Assetid);

    /// <summary>
    /// 
    /// </summary>
    public override void OnCreate()
    {
        Set("_id", UuidGenerater.NewUuid(SvcDomainType.MetaUserAsset).Guid.ToBsonBinaryData());
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
        _assetid = defaultFilter.GetUuid("assetid");
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
            {"assetid",  _assetid.Guid.ToBsonBinaryData()}
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
    /// SetUserAsset
    /// </summary>
    /// <param name="userid"></param>
    /// <param name="worldid"></param>
    /// <param name="category"></param>
    public void SetUserAsset(Uuid userid,Uuid worldid, string[] category)
    {
        Set("userid", userid.Guid.ToBsonBinaryData());
        Set("worldid", worldid.Guid.ToBsonBinaryData());
        var array = new BsonArray();
        var cateSearch = new BsonArray
            {
                category[0].ToString(),
                category[0].ToString() + "," + category[1].ToString(),
                category[0].ToString() + "," + category[1].ToString() + "," + category[2].ToString()
            };
        foreach (var cate in category)
        {
            array.Add(cate);
        }
        Set("category", array);
        Set("_srch.category", cateSearch);
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
