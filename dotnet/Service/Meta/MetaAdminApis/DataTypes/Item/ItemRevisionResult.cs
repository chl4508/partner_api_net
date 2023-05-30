using CvFramework.Common;
using MongoDB.Bson;

namespace Colorverse.MetaAdmin.DataTypes.Item;

/// <summary>
/// 
/// </summary>
public class ItemRevisionResult
{
    /// <summary>
    /// 
    /// </summary>
    public Uuid Id { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    public Uuid Itemid { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    public int Version { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public BsonDocument? Manifest { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public ItemRevisionResultStat Stat { get; set; } = null!;
}

/// <summary>
/// 
/// </summary>
public class ItemRevisionResultStat
{
    /// <summary>
    /// 
    /// </summary>
    public DateTime Created { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public DateTime Updated { get; set; }
}