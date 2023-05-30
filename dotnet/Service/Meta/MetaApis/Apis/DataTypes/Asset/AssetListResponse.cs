using Colorverse.Common.DataTypes;

namespace Colorverse.Meta.Apis.DataTypes.Asset;

/// <summary>
/// GetAssetListResponse
/// </summary>
public class AssetListResponse
{
    /// <summary>
    /// Count
    /// </summary>
    public CvListCountResponse Count { get; set; } = null!;

    /// <summary>
    /// List
    /// </summary>
    public IEnumerable<AssetResponse> List { get; set; } = null!;
}