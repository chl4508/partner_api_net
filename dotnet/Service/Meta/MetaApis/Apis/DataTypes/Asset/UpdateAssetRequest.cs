using Colorverse.Common.DataTypes;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Colorverse.Meta.Apis.DataTypes.Asset;

/// <summary>
/// UpdateAssetRequest
/// </summary>
public class UpdateAssetRequest
{
    /// <summary>
    /// 애셋아이디
    /// </summary>
    [BindNever]
    [JsonIgnore]
    public string Assetid { get; set; } = null!;

    /// <summary>
    /// manifestjson의 애셋타입 (item, avatar, land... )
    /// </summary>
    public int Type { get; set; }

    /// <summary>
    /// txt
    /// </summary>
    public UpdateAssetTxtRequest? Txt { get; set; }

    /// <summary>
    /// option
    /// </summary>
    [Required]
    public UpdateAssetOptionRequest Option { get; set; } = null!;

    /// <summary>
    /// Resource
    /// </summary>
    [Required]
    public UpdateAssetResourceRequest Resource { get; set; } = null!;
}

/// <summary>
/// UpdateAssetResourceRequest
/// </summary>
public class UpdateAssetResourceRequest
{
    /// <summary>
    /// Thumbnail cdn url
    /// </summary>
    public string? Thumbnail { get; set; }

    /// <summary>
    /// Image cdn url
    /// </summary>
    public IEnumerable<string> Image { get; set; } = null!;
}

/// <summary>
/// UpdateAssetTxtRequest
/// </summary>
public class UpdateAssetTxtRequest
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
/// UpdateAssetOptionRequest
/// </summary>
public class UpdateAssetOptionRequest
{
    /// <summary>
    /// 버전
    /// </summary>
    [Range(1, int.MaxValue)]
    public int Version { get; set; }

    /// <summary>
    /// Price
    /// </summary>
    public UpdateAssetPriceRequest? Price { get; set; }
}

/// <summary>
/// UpdateAssetPriceRequest
/// </summary>
public class UpdateAssetPriceRequest
{
    /// <summary>
    /// 판매재화타입 ( 1 : 비매품 , 2: 무료(0) , 3: gold , 4 : 컬러벅스 )
    /// </summary>
    [Range(1, 4)]
    public int? Type { get; set; }

    /// <summary>
    /// 판매 가격S
    /// </summary>
    public int? Amount { get; set; }
}
