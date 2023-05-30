using Colorverse.Application.Mediator;
using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace Colorverse.Meta.Apis.DataTypes.Cart;

/// <summary>
/// DeleteCartListRequest
/// </summary>
public class DeleteCartListRequest : RequestBase<CartResponse>
{
    /// <summary>
    /// Items
    /// </summary>
    [Required]
    public string Productids { get; set; } = null!;
}

/// <summary>
/// 
/// </summary>
public class DeleteCartListRequestValidator : AbstractValidator<DeleteCartListRequest>
{
    public DeleteCartListRequestValidator()
    {
        RuleFor(x => x.Productids).NotNull();
    }
}