namespace Colorverse.Meta.Apis.DataTypes.Asset;

/// <summary>
/// UserAssetListResponse
/// </summary>
public class UserAssetListResponse
{
    /// <summary>
    /// Count
    /// </summary>
    public UserAssetListResponseCount Count { get; set; } = null!;

    /// <summary>
    /// list
    /// </summary>
    public IEnumerable<UserAssetResponse> List { get; set; } = null!;
}

/// <summary>
/// UserAssetListResponseCount
/// </summary>
public class UserAssetListResponseCount
{
    /// <summary>
    /// Current
    /// </summary>
    public int Current { get; set; }

    /// <summary>
    /// Limit
    /// </summary>
    public int Limit { get; set; }

    /// <summary>
    /// Page
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// Total
    /// </summary>
    public int Total { get; set; }
}
