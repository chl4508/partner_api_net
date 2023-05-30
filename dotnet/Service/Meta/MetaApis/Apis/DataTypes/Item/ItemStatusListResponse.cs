namespace Colorverse.Meta.Apis.DataTypes.Item;

/// <summary>
/// GetItemVersionListResponse
/// </summary>
public class ItemStatusListResponse
{
    /// <summary>
    /// List
    /// </summary>
    public IEnumerable<ItemStatusResponse> List { get; set; } = null!;
}
