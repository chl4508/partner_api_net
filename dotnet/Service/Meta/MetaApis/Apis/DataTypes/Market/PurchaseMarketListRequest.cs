using Colorverse.Application.Mediator;
using Colorverse.Common.DataTypes.World;
using Colorverse.Common.Helper;
using CvFramework.Apis.Extensions.Attributes;
using CvFramework.Common;
using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace Colorverse.Meta.Apis.DataTypes.Market;

/// <summary>
/// PurchaseMarketListRequest
/// </summary>
public class PurchaseMarketListRequest : RequestBase<MarketOrderResponse>
{
    /// <summary>
    /// Items
    /// </summary>
    [Required]
    [MinLength(1)]
    public PurchaseMarketListProductsRequest[] Products { get; set; } = null!;

    /// <summary>
    /// 상품들의 최종 가격
    /// </summary>
    [Required]
    [MinLength(1)]
    public PurchaseMakretListTotalPriceRequest[] TotalPrice { get; set; } = null!;
}

/// <summary>
/// PurchaseCartListItemsParam
/// </summary>
public class PurchaseMarketListProductsRequest
{
    /// <summary>
    /// 상품아이디 (구매해야될 아이디 (아이템아이디, 애셋아이디))
    /// </summary>
    [Required]
    [UuidBase62]
    public string Productid { get; set; } = null!;
}

/// <summary>
/// PurchaseMakretListTotalPriceRequest
/// </summary>
public class PurchaseMakretListTotalPriceRequest
{
    /// <summary>
    /// 판매재화타입 ( 1 : 비매품 , 2: 무료(0) , 3: gold , 4 : 컬러벅스 )
    /// </summary>
    public int Type { get; set; }

    /// <summary>
    /// 가격
    /// </summary>
    [Range(0, int.MaxValue)]
    public int Amount { get; set; }
}

/// <summary>
/// 
/// </summary>
public class PurchaseMarketListRequestValidator : AbstractValidator<PurchaseMarketListRequest>
{
    public PurchaseMarketListRequestValidator(IWorldSettingHelper worldSettingHelper)
    {
        WorldSetting _worldSetting = worldSettingHelper.GetSetting(Uuid.FromBase62("1mkSuyvWsZAZKE2cuDzmzI"));

        RuleFor(x => x.Products).NotNull().ForEach(x => x.ChildRules(x =>
        {
            x.RuleFor(x => x.Productid).NotNull();
        }));

        RuleFor(x => x.TotalPrice).NotNull().ForEach(x => x.ChildRules(x =>
        {
            x.RuleFor(x => x.Type).NotNull().LessThanOrEqualTo(_worldSetting.PriceTypeLength);
            x.RuleFor(x => x.Amount).NotNull();
        }));
    }
}