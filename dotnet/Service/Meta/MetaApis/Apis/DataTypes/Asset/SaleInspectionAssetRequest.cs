using Colorverse.Application.Mediator;
using Colorverse.Common;
using Colorverse.Common.DataTypes.World;
using Colorverse.Common.Helper;
using CvFramework.Common;
using FluentValidation;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Colorverse.Meta.Apis.DataTypes.Asset;

/// <summary>
/// SaleInspectionAssetRequest
/// </summary>
public class SaleInspectionAssetRequest : RequestBase<AssetResponse>
{
    /// <summary>
    /// 애셋 아이디
    /// </summary>
    [BindNever]
    [JsonIgnore]
    public string Assetid { get; set; } = null!;

    /// <summary>
    /// option
    /// </summary>
    [Required]
    public SaleInspectionAssetOptionRequest Option { get; set; } = null!;
}

/// <summary>
/// SaleInspectionAssetOptionRequest
/// </summary>
public class SaleInspectionAssetOptionRequest
{
    /// <summary>
    /// 즉시판매 여부
    /// </summary>
    public bool InstantSale { get; set; }

    /// <summary>
    /// Price
    /// </summary>
    [Required]
    public SaleInspectionAssetPriceRequest Price { get; set; } = null!;
}

/// <summary>
/// SaleInspectionAssetPriceRequest
/// </summary>
public class SaleInspectionAssetPriceRequest
{
    /// <summary>
    /// 판매재화타입 ( 1 : 비매품 , 2: 무료(0) , 3: gold , 4 : 컬러벅스 )
    /// </summary>
    [Range(1, 4)]
    public int Type { get; set; }

    /// <summary>
    /// 판매 가격
    /// </summary>
    [Range(0, int.MaxValue)]
    public int Amount { get; set; }
}

/// <summary>
/// 
/// </summary>
public class SaleInspectionAssetRequestValidator : AbstractValidator<SaleInspectionAssetRequest>
{
    public SaleInspectionAssetRequestValidator(IWorldSettingHelper worldSettingHelper)
    {
        WorldSetting _worldSetting = worldSettingHelper.GetSetting(Uuid.FromBase62("1mkSuyvWsZAZKE2cuDzmzI"));

        RuleFor(x => Uuid.FromBase62(x.Assetid).DomainType).NotNull().Equal((ushort)SvcDomainType.MetaAsset);
        RuleFor(x => x.Option).NotNull();
        RuleFor(x => x.Option.Price).NotNull();
        RuleFor(x => x.Option.Price.Type).LessThanOrEqualTo(_worldSetting.PriceTypeLength);
    }
}