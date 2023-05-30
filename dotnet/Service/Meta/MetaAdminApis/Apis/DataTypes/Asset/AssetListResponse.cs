using Colorverse.Common.DataTypes;

namespace Colorverse.MetaAdmin.Apis.DataTypes.Asset;

/// <summary>
/// 
/// </summary>
public class AssetListResponse
{
    /// <summary>
    /// TotalCount
    /// </summary>
    public CvAdminListCountResponse Count { get; set; } = null!;

    /// <summary>
    /// List
    /// </summary>
    public IEnumerable<AssetResponse> List { get; set; } = null!;
}
