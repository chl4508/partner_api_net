using Colorverse.Application.Mediator;
using Colorverse.Common;
using CvFramework.Common;
using FluentValidation;

namespace Colorverse.Meta.Apis.DataTypes.Item;

/// <summary>
/// 
/// </summary>
public class CreateItemTemplateRequest : RequestBase<ItemTemplateResponse>
{
    /// <summary>
    /// 아이템 아이디
    /// </summary>
    public string Itemid { get; set; } = null!;
}

/// <summary>
/// 
/// </summary>
public class CreateItemTemplateRequestValidator : AbstractValidator<CreateItemTemplateRequest>
{
    public CreateItemTemplateRequestValidator()
    {
        RuleFor(x => Uuid.FromBase62(x.Itemid).DomainType).NotNull().Equal((ushort)SvcDomainType.MetaItem);
    }
}