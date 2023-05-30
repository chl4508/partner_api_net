using Colorverse.Application.Mediator;
using Colorverse.Common;
using CvFramework.Common;
using FluentValidation;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json.Serialization;

namespace Colorverse.Meta.Apis.DataTypes.Item;

/// <summary>
/// 
/// </summary>
public class SaleStartItemRequest : RequestBase<ItemResponse>
{
    /// <summary>
    /// 아이템 아이디
    /// </summary>
    [BindNever]
    [JsonIgnore]
    public string Itemid { get; set; } = null!;
}

/// <summary>
/// 
/// </summary>
public class SaleStartItemRequestValidator : AbstractValidator<SaleStartItemRequest>
{
    public SaleStartItemRequestValidator()
    {
        RuleFor(x => Uuid.FromBase62(x.Itemid).DomainType).NotNull().Equal((ushort)SvcDomainType.MetaItem);
    }
}