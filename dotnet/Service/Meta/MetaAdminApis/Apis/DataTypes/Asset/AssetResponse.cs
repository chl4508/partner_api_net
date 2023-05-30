using Colorverse.Common.Apis.DataTypes;
using Colorverse.Common.DataTypes;
using CvFramework.Common;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace Colorverse.MetaAdmin.Apis.DataTypes.Asset;

/// <summary>
/// 
/// </summary>
[BsonIgnoreExtraElements]
public class AssetResponse
{
    /// <summary>
    /// 애셋 아이디
    /// </summary>
    [JsonPropertyName("_id")]
    public Uuid Id { get; set; } = null!;

    /// <summary>
    /// 최초생성 아이디
    /// </summary>
    public Uuid Creatorid { get; set; } = null!;

    /// <summary>
    /// 프로필 아이디
    /// </summary>
    public Uuid Profileid { get; set; } = null!;

    /// <summary>
    /// 유저 아이디
    /// </summary>
    public Uuid? Userid { get; set; }

    /// <summary>
    /// 타운 아이디
    /// </summary>
    public Uuid? Townid { get; set; }

    /// <summary>
    /// 월드 아이디
    /// </summary>
    public Uuid Worldid { get; set; } = null!;

    /// <summary>
    /// manifestjson의 애셋타입 (item, avatar, land... )
    /// </summary>
    public int Type { get; set; }

    /// <summary>
    /// 제목,상세내용
    /// </summary>
    public AssetTxtResponse Txt { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    public AssetManifestResponse? Manifest { get; set; }

    /// <summary>
    /// Resource (manifest url 정보)
    /// </summary>
    public AssetResourceResponse Resource { get; set; } = null!;

    /// <summary>
    /// option
    /// </summary>
    public AssetOptionResponse Option { get; set; } = null!;

    /// <summary>
    /// 아이템 생성및수정 날짜
    /// </summary>
    public DateResponse Stat { get; set; } = null!;
}

/// <summary>
/// 
/// </summary>
[BsonIgnoreExtraElements]
public class AssetTxtResponse
{
    /// <summary>
    /// 아이템 제목
    /// </summary>
    public CvLocaleText Title { get; set; } = null!;

    /// <summary>
    /// 아이템 설명
    /// </summary>
    public CvLocaleText? Desc { get; set; }

    /// <summary>
    /// 태그 최대 6개
    /// </summary>
    public string[]? Hashtag { get; set; }
}

/// <summary>
/// 
/// </summary>
[BsonIgnoreExtraElements]
public class AssetResourceResponse
{
    /// <summary>
    /// Manifest Url
    /// </summary>
    public string Manifest { get; set; } = null!;

    /// <summary>
    /// Thumbnail Url
    /// </summary>
    public string? Thumbnail { get; set; }
}

/// <summary>
/// 
/// </summary>
[BsonIgnoreExtraElements]
public class AssetOptionResponse
{
    /// <summary>
    /// 판매승인일 (언제 판매가 승인됬는지)
    /// </summary>
    [BsonElement("sale_accept")]
    public DateTime? SaleAccept { get; set; }

    /// <summary>
    /// 판매시작일 (언제부터 판매하고싶은지 즉시판매는 당일날짜)
    /// </summary>
    [BsonElement("sale_start")]
    public DateTime? SaleStart { get; set; }

    /// <summary>
    /// 판매종료일 (언제 판매종료되는지)
    /// </summary>
    [BsonElement("sale_end")]
    public DateTime? SaleEnd { get; set; }

    /// <summary>
    /// 즉시판매 여부 (default : false)
    /// </summary>
    [BsonElement("instant_sale")]
    public bool? InstantSale { get; set; }

    /// <summary>
    /// 버전
    /// </summary>
    public int Version { get; set; }

    /// <summary>
    /// 발행 버전
    /// </summary>
    [BsonElement("sale_version")]
    public int SaleVersion { get; set; }

    /// <summary>
    /// 준비상태 (0 : 제작중, 1 : 저장됨)
    /// </summary>
    [BsonElement("ready_status")]
    public int RedayStatus { get; set; }

    /// <summary>
    /// 판매상태 (0 : 없음, 1 : 판매심사중, 2: 판매허용, 3 : 판매중)
    /// </summary>
    [BsonElement("sale_status")]
    public int SaleStatus { get; set; }

    /// <summary>
    /// 판매심사상태 (0 : 없음, 1 : 판매심사요청, 2 : 판매심사중, 3 : 판매심사반려, 4 : 판매심사승인)
    /// </summary>
    [BsonElement("sale_review_status")]
    public int SaleReviewStatus { get; set; }

    /// <summary>
    /// // 사후심사상태(0 : 없음, 1: 사후심사요청, 2 : 사후심사중, 3: 사후심사반려, 4 : 사후심사승인)
    /// </summary>
    [BsonElement("judge_status")]
    public int JudgeStatus { get; set; }

    /// <summary>
    /// 블라인드 (default : false)
    /// </summary>
    public bool Blind { get; set; }

    /// <summary>
    /// 삭제 요청여부 (default : false)
    /// </summary>
    public bool Delete { get; set; }

    /// <summary>
    /// 비매품 여부 (default : false)
    /// </summary>
    [BsonElement("none_sale")]
    public bool NoneSale { get; set; }

    /// <summary>
    /// 애셋 카테고리 정보
    /// </summary>
    public string[] Category { get; set; } = null!;

    /// <summary>
    /// 애셋 price 정보
    /// </summary>
    public AssetPriceResponse? Price { get; set; }
}

/// <summary>
/// 
/// </summary>
[BsonIgnoreExtraElements]
public class AssetPriceResponse
{
    /// <summary>
    /// 판매 재화 타입
    /// </summary>
    public int Type { get; set; }

    /// <summary>
    /// 가격
    /// </summary>
    public int Amount { get; set; }
}

/// <summary>
/// 
/// </summary>
[BsonIgnoreExtraElements]
public class AssetManifestResponse
{
    public AssetManifestMainResponse? Main { get; set; }
}

/// <summary>
/// 
/// </summary>
[BsonIgnoreExtraElements]
public class AssetManifestMainResponse
{
    public string? Type { get; set; }
}