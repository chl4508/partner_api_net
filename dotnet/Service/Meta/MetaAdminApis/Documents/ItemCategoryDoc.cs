using System.Data;
using Colorverse.MetaAdmin.Excel;
using CvFramework.MongoDB.Documents;
using MongoDB.Bson;

namespace Colorverse.MetaAdmin.Documents;

/// <summary>
/// 
/// </summary>
public class ItemCategoryDoc : MongoDocBase
{
    public const string COLLECTION_NAME = "item_category";

    public string[]? _id { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    public string[] Id { get => _id ?? throw new InvalidDataException("Undefined id."); }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static ItemCategoryDoc Create(string[] id) => new(id);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static BsonDocument CreateDefaultFilter(params string[] id)
    {
        return new BsonDocument {
            {
                "_id" , new BsonDocument()
                    {
                        {"id", BsonArray.Create(id)}
                    }
            }
        };
    }

    /// <summary>
    /// 
    /// </summary>
    public ItemCategoryDoc() : base() { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="doc"></param>
    public ItemCategoryDoc(BsonDocument doc) : base(doc) { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public ItemCategoryDoc(string[] id) : base()
    {
        _id = id;
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
    public override string GetCollectionName() => COLLECTION_NAME;

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override string? GetCacheKey()
    {
        if(_id == null)
        {
            return null;
        }
        return string.Join(':', GetDatabaseName(), GetCollectionName(), string.Join(":", Id));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="defaultFilter"></param>
    protected override void OnConstructor(BsonDocument defaultFilter)
    {
        if(!defaultFilter.TryGetValue("_id", out var idValue))
        {
            throw new BsonSerializationException("Invalid value _id.");
        }

        _id = idValue.AsBsonDocument.GetValue("id")
            .AsBsonArray
            .Select(x=>x.AsString)
            .ToArray();
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override BsonDocument GetDefaultFilter()
    {
        return CreateDefaultFilter(Id);
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="parentid"></param>
    private void SetParent(string[] parentid)
    {
        Set("parentid", string.Join(',',parentid));
        Set("_srch.parentid", CreateBsonChainArray(parentid));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="input"></param>
    /// <param name="separator"></param>
    /// <returns></returns>
    private static BsonArray CreateBsonChainArray(string[] input, char separator = ',')
    {
        var finalArry = input!.Select((x,i)=>string.Join(separator,  input, 0, i+1)).ToArray();
        return BsonArray.Create(finalArry);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    public void SetData(ItemCategory1Row source)
    {
        Set("currentid", source.Id);
        Set("option.name", source.Name);
        Set("option.depth", 1);
        Set("txt.name.ko", source.Text ?? $"type_{source.Name.ToLower()}");
        Set("txt.desc.ko", source.Desc ?? "");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="parentid"></param>
    public void SetData(ItemCategory2Row source, string[] parentid)
    {
        Set("currentid", source.Id);
        SetParent(parentid);
        Set("option.name", source.Name);
        Set("option.parent", source.Parent);
        Set("option.depth", 2);
        Set("txt.name.ko", source.Text ?? $"type_{source.Name.ToLower()}");
        Set("txt.desc.ko", source.Desc ?? "");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="parentid"></param>
    public void SetData(ItemCategory3Row source, string[] parentid)
    {
        Set("currentid", source.Id);
        SetParent(parentid);
        Set("option.name", source.Name);
        Set("option.parent", source.Parent);
        Set("option.depth", 3);
        Set("txt.name.ko", source.Text ?? $"type_{source.Name.ToLower()}");
        Set("txt.desc.ko", source.Desc ?? "");
    }
}
