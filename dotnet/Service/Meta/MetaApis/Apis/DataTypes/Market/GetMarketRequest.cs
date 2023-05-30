using Colorverse.Application.Mediator;
using FluentValidation;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json.Serialization;

namespace Colorverse.Meta.Apis.DataTypes.Market;

/// <summary>
/// GetMarketRequest
/// </summary>
public class GetMarketRequest : RequestBase<MarketResponse>
{
    /// <summary>
    /// 상품 아이디 (일단 아이템또는 애셋의 아이디 추후엔 마켓아이디)
    /// </summary>
    [BindNever]
    [JsonIgnore]
    public string Productid { get; set; } = null!;
}

/// <summary>
/// 
/// </summary>
public class GetMarketRequestValidator : AbstractValidator<GetMarketRequest>
{
    public GetMarketRequestValidator()
    {
        RuleFor(x => x.Productid).NotNull();
    }
}