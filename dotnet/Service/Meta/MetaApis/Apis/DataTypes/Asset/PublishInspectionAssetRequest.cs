namespace Colorverse.Meta.Apis.DataTypes.Asset;

/// <summary>
/// PublishInspectionAssetRequest
/// </summary>
public class PublishInspectionAssetRequest
{
    /// <summary>
    /// 애셋 아이디
    /// </summary>
    public string Assetid { get; set; } = null!;

    /// <summary>
    /// 버전
    /// </summary>
    public int Version { get; set; }
}
