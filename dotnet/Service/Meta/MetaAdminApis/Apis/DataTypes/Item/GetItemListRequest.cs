using Colorverse.Application.Mediator;
using Colorverse.Common.DataTypes;
using CvFramework.Apis.Extensions.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Colorverse.MetaAdmin.Apis.DataTypes.Item;

/// <summary>
/// 
/// </summary>
public class GetItemListRequest : RequestBase<CvAdminListResponse<ItemResponse>>
{
    /// <summary>
    /// 프로필아이디
    /// </summary>
    [Required]
    [UuidBase62]
    public string Profileid { get; set; } = null!;

    /// <summary>
    /// Page
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// limit
    /// </summary>
    public int Limit { get; set; } = 50;

    /// <summary>
    /// 비매품 유무 ( default : false)
    /// </summary>
    public bool? NoneSale { get; set; }

    /// <summary>
    /// 카테고리 ( , 로구분지어서 표현 ex) category=1,10000,10001 )
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// 준비상태 (0 : 제작중, 1 : 저장됨)
    /// </summary>
    public int? RedayStatus { get; set; }

    /// <summary>
    /// 판매상태 (0 : 없음, 1 : 판매심사중, 2: 판매허용, 3 : 판매중)
    /// </summary>
    public int? SaleStatus { get; set; }

    /// <summary>
    /// 판매심사상태 (0 : 없음, 1 : 판매심사요청, 2 : 판매심사중, 3 : 판매심사반려, 4 : 판매심사승인)
    /// </summary>
    public int? SaleReviewStatus { get; set; }

    /// <summary>
    /// 사후심사상태(0 : 없음, 1: 사후심사요청, 2 : 사후심사중, 3: 사후심사반려, 4 : 사후심사승인)
    /// </summary>
    public int? JudgeStatus { get; set; }

    /// <summary>
    /// blind
    /// </summary>
    public bool? Blind { get; set; }

    /// <summary>
    /// delete
    /// </summary>
    public bool? Delete { get; set; }
}
