using Colorverse.Application.Mediator;
using Colorverse.Common.DataTypes;
using Colorverse.Common.DataTypes.World;
using Colorverse.Common.Helper;
using CvFramework.Apis.Extensions.Attributes;
using CvFramework.Common;
using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace Colorverse.Meta.Apis.DataTypes.Item;

/// <summary>
/// 
/// </summary>
public class GetItemTemplateListRequest : RequestBase<CvListResponse<ItemTemplateResponse>>
{
    /// <summary>
    /// 월드아이디
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
    /// Category
    /// </summary>
    public string? Category { get; set; }
}

/// <summary>
/// 
/// </summary>
public class GetItemTemplateListRequestValidator : AbstractValidator<GetItemTemplateListRequest>
{
    public GetItemTemplateListRequestValidator(IWorldSettingHelper worldSettingHelper)
    {
        WorldSetting _worldSetting = worldSettingHelper.GetSetting(Uuid.FromBase62("1mkSuyvWsZAZKE2cuDzmzI"));
        RuleFor(x => x.W).Matches(_worldSetting.Base62Regex).NotNull();
    }
}