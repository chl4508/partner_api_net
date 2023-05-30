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
public class GetItemRequest : RequestBase<ItemResponse>
{
    /// <summary>
    /// 아이템 아이디
    /// </summary>
    [JsonIgnore]
    [BindNever]
    public string Itemid { get; set; } = null!;

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
public class GetItemRequestValidator : AbstractValidator<GetItemRequest>
{
    public GetItemRequestValidator()
    {
        RuleFor(x => Uuid.FromBase62(x.Itemid).DomainType ).Equal((ushort)SvcDomainType.MetaItem);
    }
}
