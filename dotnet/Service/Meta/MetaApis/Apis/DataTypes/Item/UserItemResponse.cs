using Colorverse.Common.Apis.DataTypes;
using CvFramework.Common;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace Colorverse.Meta.Apis.DataTypes.Item;

/// <summary>
/// UserItemResponse
/// </summary>
[BsonIgnoreExtraElements]
public class UserItemResponse
{
    /// <summary>
    /// useritem id
    /// </summary>
    [JsonPropertyName("_id")]
    public Uuid Id { get; set; } = null!;

    /// <summary>
    /// 유저아이디
    /// </summary>
    public Uuid Userid { get; set; } = null!;

    /// <summary>
    /// 프로필아이디
    /// </summary>
    public Uuid Profileid { get; set; } = null!;

    /// <summary>
    /// 월드아이디
    /// </summary>
    public Uuid Worldid { get; set; } = null!;

    /// <summary>
    /// 아이템아이디
    /// </summary>
    public Uuid Itemid { get; set; } = null!;

    /// <summary>
    /// Option
    /// </summary>
    public UserItemOptionResponse Option { get; set; } = null!;

    /// <summary>
    /// 카테고리
    /// </summary>
    public string[] Category { get; set; } = null!;

    /// <summary>
    /// Stat
    /// </summary>
    public DateResponse Stat { get; set; } = null!;
}

/// <summary>
/// UserItemOptionResponse
/// </summary>
[BsonIgnoreExtraElements]
public class UserItemOptionResponse
{

    /// <summary>
    /// 아이템 갯수 ( 이아이템의 보유갯수)
    /// </summary>
    public int Quantity { get; set; }
}