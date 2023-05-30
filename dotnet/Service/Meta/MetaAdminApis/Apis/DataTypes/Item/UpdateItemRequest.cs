using Colorverse.Application.Mediator;
using Colorverse.Common.DataTypes;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Colorverse.MetaAdmin.Apis.DataTypes.Item;

/// <summary>
/// 
/// </summary>
public class UpdateItemRequest : RequestBase<ItemResponse>
{
    /// <summary>
    /// 
    /// </summary>
    [JsonIgnore]
    [BindNever]
    public string Itemid { get; set; } = null!;

    /// <summary>
    /// txt
    /// </summary>
    public UpdateItemTxtRequest? Txt { get; set; }

    /// <summary>
    /// option
    /// </summary>
    public UpdateItemOptionRequest? Option { get; set; }

    /// <summary>
    /// Resource
    /// </summary>
    public UpdateItemResourceRequest? Resource { get; set; }
}

/// <summary>
/// 
/// </summary>
public class UpdateItemTxtRequest
{
    /// <summary>
    /// 제목
    /// </summary>
    public CvLocaleText? Title { get; set; }

    /// <summary>
    /// 설명
    /// </summary>
    public CvLocaleText? Desc { get; set; }

    /// <summary>
    /// 태그 최대갯수 6개
    /// </summary>
    public string[]? Hashtag { get; set; }
}

/// <summary>
/// 
/// </summary>
public class UpdateItemOptionRequest
{
    /// <summary>
    /// 
    /// </summary>
    public int Version { get; set; }

    /// <summary>
    /// Price
    /// </summary>
    public UpdateItemPriceRequest? Price { get; set; }
}

/// <summary>
/// 
/// </summary>
public class UpdateItemResourceRequest
{
    /// <summary>
    /// 
    /// </summary>
    public string? Thumbnail { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string? Preview { get; set; }
}

public class UpdateItemPriceRequest
{
    /// <summary>
    /// 판매재화타입 ( 1 : 비매품 , 2: 무료(0) , 3: gold , 4 : 컬러벅스 )
    /// </summary>
    [Range(1, 4)]
    public int? Type { get; set; }

    /// <summary>
    /// 판매 가격
    /// </summary>
    [Range(0, int.MaxValue)]
    public int? Amount { get; set; }
}