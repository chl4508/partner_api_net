namespace Colorverse.Meta.Apis.DataTypes.Item;

/// <summary>
/// PublishInspectionItemRequest
/// </summary>
public class PublishInspectionItemRequest
{
    /// <summary>
    /// 아이템 아이디
    /// </summary>
    public string Itemid { get; set; } = null!;

    /// <summary>
    /// 버전
    /// </summary>
    public int Version { get; set; }
}
