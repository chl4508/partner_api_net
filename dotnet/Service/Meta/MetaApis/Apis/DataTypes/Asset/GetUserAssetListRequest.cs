namespace Colorverse.Meta.Apis.DataTypes.Asset;

/// <summary>
/// GetUserAssetListRequest
/// </summary>
public class GetUserAssetListRequest
{
    /// <summary>
    /// Page
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Limit
    /// </summary>
    public int Limit { get; set; } = 50;

    /// <summary>
    /// 카테고리 ( , 로구분지어서 표현 ex) category=1,10000,10001 )
    /// </summary>
    public string? Category { get; set; }
}
