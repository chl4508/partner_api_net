using System.ComponentModel.DataAnnotations;
using Colorverse.Common.Enums;
using CvFramework.Apis.Extensions.Attributes;

namespace Colorverse.Meta.Apis.DataTypes.Resource;

/// <summary>
/// 
/// </summary>
public class UploadResourceRequest
{
    /// <summary>
    /// 파일
    /// </summary>
    [Required]
    public IFormFile File { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    [AllowValues(EResourceType.Asset, EResourceType.Item, EResourceType.Land)]
    public EResourceType Type { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [UuidBase62]
    public string? TargetId { get; set; }

}

