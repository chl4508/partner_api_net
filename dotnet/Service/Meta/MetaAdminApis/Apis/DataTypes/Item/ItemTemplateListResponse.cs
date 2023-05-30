using Colorverse.Common.DataTypes;

namespace Colorverse.MetaAdmin.Apis.DataTypes.Item;

/// <summary>
/// 
/// </summary>
public class ItemTemplateListResponse
{
    /// <summary>
    /// Count
    /// </summary>
    public CvAdminListCountResponse Count { get; set; } = null!;

    /// <summary>
    /// List
    /// </summary>
    public IEnumerable<ItemTemplateResponse> List { get; set; } = null!;
}