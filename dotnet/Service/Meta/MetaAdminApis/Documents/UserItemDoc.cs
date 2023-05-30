using Colorverse.Common;
using Colorverse.MetaAdmin.DataTypes.Excel;
using Colorverse.MetaAdmin.Excel;
using CvFramework.Bson;
using CvFramework.Common;
using CvFramework.MongoDB.Documents;
using CvFramework.MongoDB.Extensions;
using MongoDB.Bson;

namespace Colorverse.MetaAdmin.Documents;

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
    public override string? GetCacheKey() => string.Join(':', GetDatabaseName(), GetCollectionName(), Profileid, Itemid);

    /// <summary>
    /// 
    /// </summary>
    public override void OnCreate()
    {
        Set("_id", UuidGenerater.NewUuid(SvcDomainType.MetaUserItem).ToBsonBinaryData());
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
            {"profileid" , _profileid.ToBsonBinaryData()},
            {"itemid",  _itemid.ToBsonBinaryData()}
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
    /// 워크시트 useritem 생성
    /// </summary>
    /// <param name="param"></param>
    public void SetWorksheetUserItem(CreateExcelRequest param)
    {
        Set("userid", param.Userid.ToBsonBinaryData());
        Set("worldid", param.Worldid.ToBsonBinaryData());

        Set("option.quantity", 1);
        Set("category", new BsonArray(param.Category));
    }

    /// <summary>
    /// 워크시트 useritem 생성
    /// </summary>
    /// <param name="param"></param>
    public void SetWorksheetUserItem(ItemRow param)
    {
        Set("userid", Uuid.FromBase62(param.Userid).ToBsonBinaryData());
        Set("worldid", Uuid.FromBase62(param.Worldid).ToBsonBinaryData());

        Set("option.quantity", 1);
        string[] categoryArray = new string[3] { param.Category1!.ToString(),
                                                 param.Category1!.ToString() + "," + param.Category2!.ToString(),
                                                 param.Category1!.ToString() + "," + param.Category2!.ToString() + "," + param.Category3!.ToString() };
        Set("category", new BsonArray(categoryArray));
    }
}
