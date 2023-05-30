using Colorverse.Application.Mediator;
using Colorverse.Common.DataTypes.World;
using Colorverse.Common.Helper;
using CvFramework.Apis.Extensions.Attributes;
using CvFramework.Common;
using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace Colorverse.Meta.Apis.DataTypes.Market;

/// <summary>
/// PurchaseCartListRequest
/// </summary>
public class PurchaseCartListRequest : RequestBase<MarketOrderResponse>
{
    /// <summary>
    /// Items
    /// </summary>
    [Required]
    [MinLength(1)]
    public PurchaseCartListItemsRequest[] Items { get; set; } = null!;

    /// <summary>
    /// Price
    /// </summary>
    [Required]
    [MinLength(1)]
    public PurchaseCartListPriceRequest[] TotalPrice { get; set; } = null!;
}

/// <summary>
/// PurchaseCartListItemsRequest
/// </summary>
public class PurchaseCartListItemsRequest
{
    /// <summary>
    /// 상품아이디 (구매해야될 아이디 (아이템아이디, 애셋아이디))
    /// </summary>
    [Required]
    [UuidBase62]
    public string Productid { get; set; } = null!;

    /////// <summary>
    /////// 수량
    /////// </summary>
    ////[Required]
    ////public int Quantity { get; set; }

}

public class PurchaseCartListPriceRequest
{
    /// <summary>
    /// 판매재화타입 ( 1 : 비매품 , 2: 무료(0) , 3: gold , 4 : 컬러벅스 )
    /// </summary>
    public int PriceType { get; set; }

    /// <summary>
    /// 판매 가격
    /// </summary>
    [Range(0, int.MaxValue)]
    public int PriceAmount { get; set; }
}

/// <summary>
/// 
/// </summary>
public class PurchaseCartListRequestValidator : AbstractValidator<PurchaseCartListRequest>
{
    public PurchaseCartListRequestValidator(IWorldSettingHelper worldSettingHelper)
    {
        WorldSetting _worldSetting = worldSettingHelper.GetSetting(Uuid.FromBase62("1mkSuyvWsZAZKE2cuDzmzI"));

        RuleFor(x => x.Items).NotNull().ForEach(x => x.ChildRules(x =>
        {
            x.RuleFor(x => x.Productid).NotNull();
        }));

        RuleFor(x => x.TotalPrice).NotNull().ForEach(x => x.ChildRules(x =>
        {
            x.RuleFor(x => x.PriceType).NotNull().LessThanOrEqualTo(_worldSetting.PriceTypeLength);
            x.RuleFor(x => x.PriceAmount).NotNull();
        }));
    }
}