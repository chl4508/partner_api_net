using Colorverse.Application.Mediator;
using Colorverse.Common.DataTypes;
using Colorverse.Common.DataTypes.World;
using Colorverse.Common.Helper;
using CvFramework.Apis.Extensions.Attributes;
using CvFramework.Common;
using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace Colorverse.Meta.Apis.DataTypes.Market;

/// <summary>
/// GetMarketListRequest
/// </summary>
public class GetMarketListRequest : RequestBase<CvListResponse<MarketResponse>>
{
    /// <summary>
    /// 월드 아이디
    /// </summary>
    [Required]
    [UuidBase62]
    public string W { get; set; } = null!;

    /// <summary>
    /// Page
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Limit
    /// </summary>
    public int Limit { get; set; } = 50;

    /// <summary>
    /// 프로필 아이디
    /// </summary>
    public string? Profileid { get; set; }

    /// <summary>
    /// Category
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// 비매품 여부
    /// </summary>
    public bool? NoneSale { get; set; }
}

/// <summary>
/// 
/// </summary>
public class GetMarketListRequestValidator : AbstractValidator<GetMarketListRequest>
{
    public GetMarketListRequestValidator(IWorldSettingHelper worldSettingHelper)
    {
        WorldSetting _worldSetting = worldSettingHelper.GetSetting(Uuid.FromBase62("1mkSuyvWsZAZKE2cuDzmzI"));

        RuleFor(x => Uuid.FromBase62(x.W)).NotNull();
        RuleFor(x => x.Category).Must(cateogry => _worldSetting.Category.Check(cateogry!)).When(x => x.Category != null);
    }
}
