using Colorverse.Application.Mediator;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json.Serialization;

namespace Colorverse.MetaAdmin.Apis.DataTypes.Asset;

/// <summary>
/// 
/// </summary>
public class SaleRejectionAssetRequest : RequestBase<AssetResponse?>
{
    /// <summary>
    /// 
    /// </summary>
    [JsonIgnore]
    [BindNever]
    public string Assetid { get; set; } = null!;

    /// <summary>
    /// 사후심사 여부 true일경우 사후심사 진행(default : false) 
    /// </summary>
    public bool LaterReview { get; set; } = false;
}
