using Colorverse.Common.DataTypes;

namespace Colorverse.Meta.Apis.DataTypes.Item;

/// <summary>
/// UserItemListResponse
/// </summary>
public class UserItemListResponse
{
    /// <summary>
    /// Totalcount
    /// </summary>
    public CvListCountResponse Count { get; set; } = null!;

    /// <summary>
    /// list
    /// </summary>
    public IEnumerable<UserItemResponse> List { get; set; } = null!;
}