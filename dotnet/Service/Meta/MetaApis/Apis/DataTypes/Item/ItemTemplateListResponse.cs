using Colorverse.Common.DataTypes;

namespace Colorverse.Meta.Apis.DataTypes.Item;

/// <summary>
/// 
/// </summary>
public class ItemTemplateListResponse
{
    /// <summary>
    /// Count
    /// </summary>
    public CvListCountResponse Count { get; set; } = null!;

    /// <summary>
    /// List
    /// </summary>
    public IEnumerable<ItemTemplateResponse> List { get; set; } = null!;
}