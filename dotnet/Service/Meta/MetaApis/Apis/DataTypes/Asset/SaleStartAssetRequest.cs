using Colorverse.Application.Mediator;
using Colorverse.Common;
using CvFramework.Common;
using FluentValidation;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json.Serialization;

namespace Colorverse.Meta.Apis.DataTypes.Asset;

/// <summary>
/// SaleStartAssetRequest
/// </summary>
public class SaleStartAssetRequest : RequestBase<AssetResponse>
{
    /// <summary>
    /// 애셋 아이디
    /// </summary>
    [BindNever]
    [JsonIgnore]
    public string Assetid { get; set; } = null!;
}

/// <summary>
/// 
/// </summary>
public class SaleStartAssetRequestValidator : AbstractValidator<SaleStartAssetRequest>
{
    public SaleStartAssetRequestValidator()
    {
        RuleFor(x => Uuid.FromBase62(x.Assetid).DomainType).NotNull().Equal((ushort)SvcDomainType.MetaAsset);
    }
}