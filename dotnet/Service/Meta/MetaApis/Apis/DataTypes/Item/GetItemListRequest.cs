using Colorverse.Application.Mediator;
using Colorverse.Common.DataTypes;
using Colorverse.Common.DataTypes.World;
using Colorverse.Common.Helper;
using CvFramework.Common;
using FluentValidation;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json.Serialization;

namespace Colorverse.Meta.Apis.DataTypes.Item;

/// <summary>
/// GetItemListRequest
/// </summary>
public class GetItemListRequest : RequestBase<CvListResponse<ItemResponse>>
{
    /// <summary>
    /// 프로필아이디
    /// </summary>
    [JsonIgnore]
    [BindNever]
    public string Profileid { get; set; } = null!;

    /// <summary>
    /// Page
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// limit
    /// </summary>
    public int Limit { get; set; } = 50;

    /// <summary>
    /// 비매품 유무
    /// </summary>
    public bool? NoneSale { get; set; }

    /// <summary>
    /// 카테고리 ( , 로구분지어서 표현 ex) category=1,10000,10001 )
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// 나의 데이터인지 상대방의 데이터인지 체크
    /// </summary>
    [JsonIgnore]
    [BindNever]
    public bool MeCheck { get; set; } = false;
}

/// <summary>
/// 
/// </summary>
public class GetItemListRequestValidator : AbstractValidator<GetItemListRequest>
{
    public GetItemListRequestValidator(IWorldSettingHelper worldSettingHelper)
    {
        WorldSetting _worldSetting = worldSettingHelper.GetSetting(Uuid.FromBase62("1mkSuyvWsZAZKE2cuDzmzI"));

        RuleFor(x => x.Profileid).NotNull();
        RuleFor(x => x.Category).Must(cateogry => _worldSetting.Category.Check(cateogry!)).When(x => x.Category != null);
    }
}