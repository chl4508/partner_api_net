using CvFramework.Common;
using MongoDB.Bson;

namespace Colorverse.MetaAdmin.DataTypes.Asset;

/// <summary>
/// 
/// </summary>
public class AssetRevisionResult
{
    /// <summary>
    /// 
    /// </summary>
    public Uuid Id { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    public Uuid Assetid { get; set; } = null!;

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
    public AssetRevisionResultStat Stat { get; set; } = null!;

}

public class AssetRevisionResultStat
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