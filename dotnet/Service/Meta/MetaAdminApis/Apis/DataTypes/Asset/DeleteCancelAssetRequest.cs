using Colorverse.Application.Mediator;

namespace Colorverse.MetaAdmin.Apis.DataTypes.Asset;

/// <summary>
/// 
/// </summary>
public class DeleteCancelAssetRequest : RequestBase<AssetResponse>
{
    /// <summary>
    /// 
    /// </summary>
    public string Assetid { get; set; } = null!;
}
