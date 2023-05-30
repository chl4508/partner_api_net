using System.ComponentModel.DataAnnotations;

namespace Colorverse.Meta.Apis.DataTypes.Item;

/// <summary>
/// GetItemVersionRequest
/// </summary>
public class GetItemStatusListRequest
{
    /// <summary>
    /// 아이템/애셋 아이디들
    /// </summary>
    [Required]
    public string Itemids { get; set; } = null!;
}
