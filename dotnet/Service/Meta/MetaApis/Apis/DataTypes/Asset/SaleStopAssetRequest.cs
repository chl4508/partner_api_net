using Colorverse.Application.Mediator;
using Colorverse.Common;
using CvFramework.Common;
using FluentValidation;

namespace Colorverse.Meta.Apis.DataTypes.Asset;

/// <summary>
/// SaleStopAssetRequest
/// </summary>
public class SaleStopAssetRequest : RequestBase<AssetResponse>
{
    /// <summary>
    /// 애셋 아이디
    /// </summary>
    public string Assetid { get; set; } = null!;
}

/// <summary>
/// 
/// </summary>
public class SaleStopAssetRequestValidator : AbstractValidator<SaleStopAssetRequest>
{
    public SaleStopAssetRequestValidator()
    {
        RuleFor(x => Uuid.FromBase62(x.Assetid).DomainType).NotNull().Equal((ushort)SvcDomainType.MetaAsset);
    }
}