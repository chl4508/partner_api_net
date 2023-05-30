using Colorverse.Application.Mediator;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json.Serialization;

namespace Colorverse.MetaAdmin.Apis.DataTypes.Asset;

/// <summary>
/// 
/// </summary>
public class SaleApprovalAssetRequest : RequestBase<AssetResponse>
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

    /// <summary>
    /// 판매 시작일
    /// </summary>
    public DateTime? SaleStart { get; set; }

    /// <summary>
    /// 판매 종료일
    /// </summary>
    public DateTime? SaleEnd { get; set; }
}
