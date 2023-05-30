using Colorverse.Application.Mediator;
using Colorverse.Common.DataTypes;
using CvFramework.Apis.Extensions.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Colorverse.MetaAdmin.Apis.DataTypes.Item;

/// <summary>
/// 
/// </summary>
public class GetItemTemplateListRequest : RequestBase<CvAdminListResponse<ItemTemplateResponse>>
{
    /// <summary>
    /// 월드아이디
    /// </summary>
    [Required]
    [UuidBase62]
    public string W { get; set; } = null!;

    /// <summary>
    /// Page
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Limit
    /// </summary>
    public int Limit { get; set; } = 50;

    /// <summary>
    /// Category
    /// </summary>
    public string? Category { get; set; }
}
