using Colorverse.Common.DataTypes;

namespace Colorverse.Meta.Apis.DataTypes.Market;

/// <summary>
/// MarketListResponse
/// </summary>
public class MarketListResponse
{
    /// <summary>
    /// Count
    /// </summary>
    public CvListCountResponse Count { get; set; } = null!;

    /// <summary>
    /// List
    /// </summary>
    public IEnumerable<MarketResponse> List { get; set; } = null!;
}