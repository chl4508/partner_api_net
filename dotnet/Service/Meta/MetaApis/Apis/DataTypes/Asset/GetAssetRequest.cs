using Colorverse.Application.Mediator;
using Colorverse.Common;
using CvFramework.Common;
using FluentValidation;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json.Serialization;

namespace Colorverse.Meta.Apis.DataTypes.Asset;

/// <summary>
/// GetAssetRequest
/// </summary>
public class GetAssetRequest : RequestBase<AssetResponse>
{
    /// <summary>
    /// 애셋 아이디
    /// </summary>
    [JsonIgnore]
    [BindNever]
    public string Assetid { get; set; } = null!;

    /// <summary>
    /// 나의 데이터인지 상대방의 데이터인지 체크
    /// </summary>
    [JsonIgnore]
    [BindNever]
    public bool MeCheck { get; set; }
}

/// <summary>
/// 
/// </summary>
public class GetAssetRequestValidator : AbstractValidator<GetAssetRequest>
{
    public GetAssetRequestValidator()
    {
        RuleFor(x => Uuid.FromBase62(x.Assetid).DomainType).Equal((ushort)SvcDomainType.MetaAsset);
    }
}