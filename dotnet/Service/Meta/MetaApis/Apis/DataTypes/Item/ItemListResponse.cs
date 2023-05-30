using Colorverse.Common.DataTypes;

namespace Colorverse.Meta.Apis.DataTypes.Item;

/// <summary>
/// GetItemListResponse
/// </summary>
public class ItemListResponse
{
    /// <summary>
    /// TotalCount
    /// </summary>
    public CvListCountResponse Count { get; set; } = null!;

    /// <summary>
    /// List
    /// </summary>
    public IEnumerable<ItemResponse> List { get; set; } = null!;
}