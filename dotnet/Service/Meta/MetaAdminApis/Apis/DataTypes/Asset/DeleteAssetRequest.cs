using Colorverse.Application.Mediator;

namespace Colorverse.MetaAdmin.Apis.DataTypes.Asset;

/// <summary>
/// 
/// </summary>
public class DeleteAssetRequest : RequestBase<AssetResponse>
{
    /// <summary>
    /// 
    /// </summary>
    public string Assetid { get; set; } = null!;
}
