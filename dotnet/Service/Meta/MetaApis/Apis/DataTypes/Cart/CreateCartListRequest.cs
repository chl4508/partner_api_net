using Colorverse.Application.Mediator;
using Colorverse.Common.DataTypes.World;
using Colorverse.Common.Helper;
using CvFramework.Apis.Extensions.Attributes;
using CvFramework.Common;
using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace Colorverse.Meta.Apis.DataTypes.Cart;

/// <summary>
/// CreateCartRequest
/// </summary>
public class CreateCartListRequest : RequestBase<CartResponse>
{
    /// <summary>
    /// Items
    /// </summary>
    [Required]
    [MinLength(1)]
    public CreateCartItemsRequest[] Items { get; set; } = null!;
}

/// <summary>
/// CreateCartItemsRequest
/// </summary>
public class CreateCartItemsRequest
{
    /// <summary>
    /// 마켓아이디 (구매해야될 아이디 (아이템아이디, 애셋아이디))
    /// </summary>
    [UuidBase62]
    [Required]
    public string Productid { get; set; } = null!;

    /// <summary>
    /// 판매재화타입 ( 1 : 비매품 , 2: 무료(0) , 3: gold , 4 : 컬러벅스 )
    /// </summary>
    [Range(1, 4)]
    public int PriceType { get; set; }

    /// <summary>
    /// 판매 가격
    /// </summary>
    [Range(0, int.MaxValue)]
    public int PriceAmount { get; set; }

    /// <summary>
    /// Category
    /// </summary>
    [Required]
    public string Category { get; set; } = null!;
}

/// <summary>
/// 
/// </summary>
public class CreateCartListRequestValidator : AbstractValidator<CreateCartListRequest>
{
    public CreateCartListRequestValidator(IWorldSettingHelper worldSettingHelper)
    {
        WorldSetting _worldSetting = worldSettingHelper.GetSetting(Uuid.FromBase62("1mkSuyvWsZAZKE2cuDzmzI"));

        RuleFor(x => x.Items).NotNull().ForEach(x => x.ChildRules(x =>
        {
            x.RuleFor(x => x.Productid).NotNull().Matches(_worldSetting.Base62Regex);
            x.RuleFor(x => x.Category).NotNull().Must(cateogry => _worldSetting.Category.Check(cateogry));
        }));
    }
}
