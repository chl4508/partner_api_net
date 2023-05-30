using Colorverse.Common.Apis.DataTypes;
using CvFramework.Common;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace Colorverse.Meta.Apis.DataTypes.Market;

/// <summary>
/// MarketOrderResponse
/// </summary>
[BsonIgnoreExtraElements]
public class MarketOrderResponse
{
    /// <summary>
    /// orderid
    /// </summary>
    [JsonPropertyName("_id")]
    public Uuid Id { get; set; } = null!;

    /// <summary>
    /// 유저 아이디
    /// </summary>
    public Uuid Userid { get; set; } = null!;

    /// <summary>
    /// 프로필 아이디
    /// </summary>
    public Uuid Profileid { get; set; } = null!;

    /// <summary>
    /// 주문 상태값 (reuqest, paid, deleiverd, cancel )
    /// </summary>
    public int State { get; set; }

    /// <summary>
    /// Txt
    /// </summary>
    public MarketOrderTxtResponse Txt { get; set; } = null!;

    /// <summary>
    /// Option
    /// </summary>
    public MarketOrderOptionResponse Option { get; set; } = null!;

    /// <summary>
    /// Product
    /// </summary>
    public MarketOrderProductResponse[] Product { get; set; } = null!;

    /// <summary>
    /// Stat
    /// </summary>
    public DateResponse Stat { get; set; } = null!;
}

/// <summary>
/// MarketOrderTxtResponse
/// </summary>
[BsonIgnoreExtraElements]
public class MarketOrderTxtResponse
{
    /// <summary>
    /// 대표 상품 제목 ( 첫번째 상품의 제목)
    /// </summary>
    public string Title { get; set; } = null!;
}

/// <summary>
/// MarketOrderOptionResponse
/// </summary>
[BsonIgnoreExtraElements]
public class MarketOrderOptionResponse
{
    /// <summary>
    /// 첫번째 상품의 category
    /// </summary>
    public string[] Category { get; set; } = null!;

    /// <summary>
    /// Payment
    /// </summary>
    public MarketOrderPaymentResponse[] Payment { get; set; } = null!;
}

/// <summary>
/// MarketOrderPaymentResponse
/// </summary>
[BsonIgnoreExtraElements]
public class MarketOrderPaymentResponse
{
    /// <summary>
    /// 판매재화타입 ( 1 : 비매품 , 2: 무료(0) , 3: gold , 4 : 컬러벅스 )
    /// </summary>
    public int Type { get; set; }

    /// <summary>
    /// 상품들의 가격
    /// </summary>
    public int Amount { get; set; }
}

/// <summary>
/// MarketOrderProduct
/// </summary>
[BsonIgnoreExtraElements]
[BsonNoId]
public class MarketOrderProductResponse
{
    /// <summary>
    /// 상품아이디 ( productid )
    /// </summary>
    public Uuid Id { get; set; } = null!;

    /// <summary>
    /// 상품 수량
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// 상품 제목
    /// </summary>
    public string Title { get; set; } = null!;

    /// <summary>
    /// 상품 카테고리
    /// </summary>
    public string[] Category { get; set; } = null!;

    /// <summary>
    /// 판매재화타입 ( 1 : 비매품 , 2: 무료(0) , 3: gold , 4 : 컬러벅스 )
    /// </summary>
    [BsonElement("payment_type")]
    public int PaymentType { get; set; }

    /// <summary>
    /// 상품의 가격
    /// </summary>
    [BsonElement("payment_amount")]
    public int PaymentAmount { get; set; }

    /// <summary>
    /// 아이템들
    /// </summary>
    public MarketOrderItemResponse[] Item { get; set; } = null!;
}

/// <summary>
/// MarketOrderItem
/// </summary>
[BsonIgnoreExtraElements]
[BsonNoId]
public class MarketOrderItemResponse
{
    /// <summary>
    /// 아이템 애셋 구분 ( 1: 아이템 , 2: 애셋)
    /// </summary>
    public int Type { get; set; }

    /// <summary>
    /// 아이템/애셋 아이디
    /// </summary>
    public Uuid Id { get; set; } = null!;

    /// <summary>
    /// 수량
    /// </summary>
    public int Quantity { get; set; }
}