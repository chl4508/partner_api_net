using System.Text.Json.Serialization;

namespace Colorverse.Meta.Apis.DataTypes.Asset;

/// <summary>
/// UserAssetResponse
/// </summary>
public class UserAssetResponse
{
    /// <summary>
    /// userasset id
    /// </summary>
    [JsonPropertyName("_id")]
    public string Id { get; set; } = null!;

    /// <summary>
    /// 유저아이디
    /// </summary>
    public string Userid { get; set; } = null!;

    /// <summary>
    /// 프로필아이디
    /// </summary>
    public string Profileid { get; set; } = null!;

    /// <summary>
    /// 월드아이디
    /// </summary>
    public string Worldid { get; set; } = null!;

    /// <summary>
    /// 애셋아이디
    /// </summary>
    public string Assetid { get; set; } = null!;

    /// <summary>
    /// Option
    /// </summary>
    public UserAssetOptionResponse Option { get; set; } = null!;

    /// <summary>
    /// 카테고리
    /// </summary>
    public string[] Category { get; set; } = null!;

    /// <summary>
    /// Stat
    /// </summary>
    public UserAssetStatResponse Stat { get; set; } = null!;
}

/// <summary>
/// UserAssetOption
/// </summary>
public class UserAssetOptionResponse
{

    /// <summary>
    /// 애셋 갯수 ( 이애셋의 보유갯수)
    /// </summary>
    public int Quantity { get; set; }
}

/// <summary>
/// UserAssetStat
/// </summary>
public class UserAssetStatResponse
{
    /// <summary>
    /// 생성 날짜
    /// </summary>
    public long Created { get; set; }

    /// <summary>
    /// 수정날짜
    /// </summary>
    public long Updated { get; set; }
}