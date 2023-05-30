using Colorverse.Common.DataTypes;

namespace Colorverse.MetaAdmin.Apis.DataTypes.Item;

/// <summary>
/// 
/// </summary>
public class ItemListResponse
{
    /// <summary>
    /// TotalCount
    /// </summary>
    public CvAdminListCountResponse Count { get; set; } = null!;

    /// <summary>
    /// List
    /// </summary>
    public IEnumerable<ItemResponse> List { get; set; } = null!;
}