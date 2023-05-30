using CvFramework.MongoDB.Documents;
using MongoDB.Bson;

namespace Colorverse.Meta.Documents;

/// <summary>
/// 
/// </summary>
public class ItemCategoryDoc : MongoReadOnlyDocBase
{
    public const string COLLECTION_NAME = "item_category";

    public int[]? _id { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    public int[] Id { get => _id ?? throw new InvalidDataException("Undefined id."); }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static BsonDocument CreateDefaultilter(params int[] id)
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
    public ItemCategoryDoc(int[] id) : base()
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
            .Select(x=>x.AsInt32)
            .ToArray();
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override BsonDocument GetDefaultFilter()
    {
        return CreateDefaultilter(Id);
    }
}
