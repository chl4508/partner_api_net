using Colorverse.Common.Apis.DataTypes;
using Colorverse.Common.DataTypes;
using CvFramework.Common;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace Colorverse.Meta.Apis.DataTypes.Market;

/// <summary>
/// MarketResponse
/// </summary>
[BsonIgnoreExtraElements]
public class MarketResponse
{
    /// <summary>
    /// product 아이디 (일단 아이템또는 애셋의 아이디 추후엔 마켓아이디)
    /// </summary>
    [JsonPropertyName("_id")]
    [BsonElement("_id")]
    public Uuid Id { get; set; } = null!;

    /// <summary>
    /// 상품데이터 구분 (아이템인지 애셋지)
    /// </summary>
    public int Type { get; set; }

    /// <summary>
    /// 아이템/애셋 권한 아이디 ( profileid.. 추후 , 회사 아이디, plazaid ..등)
    /// </summary>
    public Uuid Profileid { get; set; } = null!;

    /// <summary>
    /// 월드 아이디
    /// </summary>
    public Uuid Worldid { get; set; } = null!;

    /// <summary>
    /// 판매상태 (없음, 판매심사중, 판매불가, 판매허용, 판매중, 판매중지)
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// Txt
    /// </summary>
    public MarketTxtResponse Txt { get; set; } = null!;

    /// <summary>
    /// resource
    /// </summary>
    public MarketResourceResponse Resource { get; set; } = null!;

    /// <summary>
    /// Option
    /// </summary>
    public MarketOptionResponse Option { get; set; } = null!;

    /// <summary>
    /// 아이템 생성및수정 날짜
    /// </summary>
    public DateResponse Stat { get; set; } = null!;
}

/// <summary>
/// MarketResourceResponse
/// </summary>
[BsonIgnoreExtraElements]
public class MarketResourceResponse
{
    /// <summary>
    /// Manifest Url
    /// </summary>
    public string? Manifest { get; set; }

    /// <summary>
    /// Thumbnail Url
    /// </summary>
    public string? Thumbnail { get; set; }

    /// <summary>
    /// Preview Url
    /// </summary>
    public string? Preview { get; set; }
}

/// <summary>
/// MarketTxtResponse
/// </summary>
[BsonIgnoreExtraElements]
public class MarketTxtResponse
{
    /// <summary>
    /// 제목
    /// </summary>
    public CvLocaleText Title { get; set; } = null!;

    /// <summary>
    /// 설명
    /// </summary>
    public CvLocaleText? Desc { get; set; }

    /// <summary>
    /// 태그 최대 6개
    /// </summary>
    public string[]? Hashtag { get; set; }
}

/// <summary>
/// MarketOptionResponse
/// </summary>
[BsonIgnoreExtraElements]
public class MarketOptionResponse
{
    /// <summary>
    /// 판매승인일 (언제 판매가 승인됬는지)
    /// </summary>
    [BsonElement("sale_accept")]
    public DateTime SaleAccept { get; set; }

    /// <summary>
    /// 판매시작일 (언제부터 판매하고싶은지 즉시판매는 당일날짜로)
    /// </summary>
    [BsonElement("sale_start")]
    public DateTime? SaleStart { get; set; }

    /// <summary>
    /// 판매종료일 (언제 판매종료되는지)
    /// </summary>
    [BsonElement("sale_end")]
    public DateTime? SaleEnd { get; set; }

    /// <summary>
    /// 판매 버전
    /// </summary>
    [BsonElement("sale_version")]
    public int SaleVersion { get; set; }

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
    /// manifestjson의 애셋타입 (item, avatar, land... )
    /// </summary>
    [BsonElement("asset_type")]
    public int AssetType { get; set; }

    /// <summary>
    /// 카테고리
    /// </summary>
    public string[] Category { get; set; } = null!;

    /// <summary>
    /// Price
    /// </summary>
    public MarketPriceResponse Price { get; set; } = null!;

    /// <summary>
    /// Items(아이템/애셋) 패키지를 대비한 처리
    /// </summary>
    public MarketItemsResponse[] Items { get; set; } = null!;
}

/// <summary>
/// MarketItems
/// </summary>
[BsonIgnoreExtraElements]
public class MarketPriceResponse
{
    /// <summary>
    /// 판매재화타입 ( 1 : 비매품 , 2: 무료(0) , 3: gold , 4 : 컬러벅스 )
    /// </summary>
    public int Type { get; set; }

    /// <summary>
    /// 가격
    /// </summary>
    public int Amount { get; set; }
}

/// <summary>
/// MarketPrice
/// </summary>
[BsonIgnoreExtraElements]
[BsonNoId]
public class MarketItemsResponse
{
    /// <summary>
    /// 상품데이터구분 (아이템인지 애셋인지)
    /// </summary>
    public int Type { get; set; }

    /// <summary>
    /// 구매해야될 아이디 (아이템아이디, 애셋아이디)
    /// </summary>
    public Uuid Id { get; set; } = null!;

    /// <summary>
    /// 수량
    /// </summary>
    public int Quantity { get; set; }
}