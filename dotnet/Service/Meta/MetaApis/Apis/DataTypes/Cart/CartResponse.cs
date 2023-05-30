using Colorverse.Common.Apis.DataTypes;
using CvFramework.Common;
using MongoDB.Bson.Serialization.Attributes;

namespace Colorverse.Meta.Apis.DataTypes.Cart;

/// <summary>
/// CartResponse
/// </summary>
[BsonIgnoreExtraElements]
public class CartResponse
{
    /// <summary>
    /// items
    /// </summary>
    public CartItemsResponse[]? Items { get; set; }

    /// <summary>
    /// stat
    /// </summary>
    public DateResponse? Stat { get; set; }
}

/// <summary>
/// CartItemsResponse
/// </summary>
[BsonIgnoreExtraElements]
public class CartItemsResponse
{
    /// <summary>
    /// 상품데이터구분 (아이템인지 애셋인지)
    /// </summary>
    public int? Type { get; set; }

    /// <summary>
    /// 구매해야될 아이디 (아이템아이디, 애셋아이디)
    /// </summary>
    public Uuid? Productid { get; set; }

    /// <summary>
    /// 수량
    /// </summary>
    public int? Quantity { get; set; }

    /// <summary>
    /// 판매재화타입 ( 1 : 비매품 , 2: 무료(0) , 3: gold , 4 : 컬러벅스 )
    /// </summary>
    [BsonElement("price_type")]
    public int PriceType { get; set; }

    /// <summary>
    /// 판매 가격
    /// </summary>
    [BsonElement("price_amount")]
    public int PriceAmount { get; set; }

    /// <summary>
    /// Category
    /// </summary>
    public string[] Category { get; set; } = null!;
}