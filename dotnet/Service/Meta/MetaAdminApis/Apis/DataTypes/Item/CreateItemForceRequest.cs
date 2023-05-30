using Colorverse.Application.Mediator;
using Colorverse.Common;
using CvFramework.Apis.Extensions.Attributes;
using CvFramework.Common;
using FluentValidation;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Colorverse.MetaAdmin.Apis.DataTypes.Item;

/// <summary>
/// 
/// </summary>
public class CreateItemForceRequest : RequestBase<ItemResponse>
{

    ////public CreateItemForceRequestList[] List { get; set; } = null!;

    /// <summary>
    /// 아이템 아이디
    /// </summary>
    [JsonIgnore]
    [BindNever]
    public string Itemid { get; set; } = null!;

    /// <summary>
    /// 서버 사용여부
    /// </summary>
    public string UseStatus { get; set; } = "N";

    /// <summary>
    /// 제목
    /// </summary>
    public string Title { get; set; } = null!;

    /// <summary>
    /// 설명
    /// </summary>
    public string? Desc { get; set; }

    /// <summary>
    /// 해시태그 ( 여러개를 넣을경우 , 로구분)
    /// </summary>
    public string? HashTag { get; set; }

    /// <summary>
    /// Category1
    /// </summary>
    public int Category1 { get; set; }

    /// <summary>
    /// Category2
    /// </summary>
    public int Category2 { get; set; }

    /// <summary>
    /// Category3
    /// </summary>
    public int Category3 { get; set; }

    /// <summary>
    /// 판매 상태
    /// </summary>
    public int SaleStatus { get; set; }

    /// <summary>
    /// 판매 타입
    /// </summary>
    public int PriceType { get; set; }

    /// <summary>
    /// 판매 가격
    /// </summary>
    public int PriceAmount { get; set; }

    /// <summary>
    /// client_itemid
    /// </summary>
    public string? ClientItemid { get; set; }

    /// <summary>
    /// 버전
    /// </summary>
    public int Version { get; set; }
}

/// <summary>
/// 
/// </summary>
public class CreateItemForceRequestValidator : AbstractValidator<CreateItemForceRequest>
{
    public CreateItemForceRequestValidator()
    {
        RuleFor(x => Uuid.FromBase62(x.Itemid).DomainType).NotNull().Equal((ushort)SvcDomainType.MetaItem);
    }

}

/// <summary>
/// 
/// </summary>
public class CreateItemForceRequestList
{
    /// <summary>
    /// 
    /// </summary>
    [Required]
    [UuidBase62]
    public string Itemid { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    public int Version { get; set; }
}
