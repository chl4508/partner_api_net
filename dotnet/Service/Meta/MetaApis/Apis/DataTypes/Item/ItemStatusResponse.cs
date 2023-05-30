using System.Text.Json.Serialization;

namespace Colorverse.Meta.Apis.DataTypes.Item;

/// <summary>
/// GetItemVersionResponse
/// </summary>
public class ItemStatusResponse
{
    /// <summary>
    /// 아이템 아이디
    /// </summary>
    [JsonPropertyName("_id")]
    public string Id { get; set; } = null!;

    /// <summary>
    /// 소유자 아이디
    /// </summary>
    public string Ownerid { get; set; } = null!;

    /// <summary>
    /// 소유자 타입 ( 프로필인지 플라자인지 등등)
    /// </summary>
    public int OwnerType { get; set; }

    /// <summary>
    /// 상태값 ( 준비중, 게시 심사중, 게시불가, 게시허용, 게시중, 삭제)
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 판매상태 (없음, 판매심사중, 판매불가, 판매허용, 판매중, 판매중지)
    /// </summary>
    public int SaleStatus { get; set; }

    /// <summary>
    /// 판매심사상태(없음, 판매심사요청, 판매심사중, 판매심사반려, 판매심사완료)
    /// </summary>
    public int SaleReviewStatus { get; set; }

    /// <summary>
    /// 아이템/애셋 버전
    /// </summary>
    public int PublishVersion { get; set; }

    /// <summary>
    /// Manifest Url 경로
    /// </summary>
    public string? Manifest { get; set; }
}