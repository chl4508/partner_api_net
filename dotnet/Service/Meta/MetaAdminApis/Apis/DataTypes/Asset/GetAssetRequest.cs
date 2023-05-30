using Colorverse.Application.Mediator;

namespace Colorverse.MetaAdmin.Apis.DataTypes.Asset;

/// <summary>
/// 
/// </summary>
public class GetAssetRequest : RequestBase<AssetResponse>
{
    /// <summary>
    /// 
    /// </summary>
    public string Assetid { get; set; } = null!;
}
