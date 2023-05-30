using Colorverse.Application.Mediator;
using Colorverse.Common.DataTypes;
using Colorverse.Common.DataTypes.World;
using Colorverse.Common.Helper;
using CvFramework.Common;
using FluentValidation;

namespace Colorverse.Meta.Apis.DataTypes.Item;

/// <summary>
/// GetUserItemListRequest
/// </summary>
public class GetUserItemListRequest : RequestBase<CvListResponse<UserItemResponse>>
{
    /// <summary>
    /// Page
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Limit
    /// </summary>
    public int Limit { get; set; } = 50;

    /// <summary>
    /// 카테고리 ( , 로구분지어서 표현 ex) category=1,10000,10001 )
    /// </summary>
    public string? Category { get; set; }
}

/// <summary>
/// 
/// </summary>
public class GetUserItemListRequestValidator : AbstractValidator<GetUserItemListRequest>
{
    public GetUserItemListRequestValidator(IWorldSettingHelper worldSettingHelper)
    {
        WorldSetting _worldSetting = worldSettingHelper.GetSetting(Uuid.FromBase62("1mkSuyvWsZAZKE2cuDzmzI"));

        RuleFor(x => x.Category).Must(cateogry => _worldSetting.Category.Check(cateogry!)).When(x => x.Category != null);
    }
}
