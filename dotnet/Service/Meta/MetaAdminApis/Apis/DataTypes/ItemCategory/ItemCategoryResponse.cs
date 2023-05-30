using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Colorverse.Common.Apis.DataTypes;
using MongoDB.Bson.Serialization.Attributes;

namespace Colorverse.MetaAdmin.Apis.DataTypes.ItemCategory;

/// <summary>
/// 
/// </summary>
[BsonIgnoreExtraElements]
public class ItemCategoryResponse
{
    /// <summary>
    /// 
    /// </summary>
    /// <value></value>
    [BsonNoId]
    public class ItemCategoryId
    {
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        [BsonElement("id")]
        public int[] Id { get; set;} = null!;
    }

    [JsonIgnore]
    [BsonElement("_id")]
    public ItemCategoryId _Id {get; set;} = null!;

    /// <summary>
    /// 
    /// </summary>
    /// <value></value>
    [JsonPropertyName("_id")]
    [BsonIgnore]
    public string Id { get => string.Join(',', _Id.Id);}

    /// <summary>
    /// 
    /// </summary>
    /// <value></value>
    [JsonPropertyName("currentid")]
    public string CurrentId { get; set;} = null!;

    /// <summary>
    /// 
    /// </summary>
    /// <value></value>
    [JsonPropertyName("parentid")]
    public string ParentId { get; set;} = null!;

    /// <summary>
    /// 카테고리 텍스트 객체
    /// </summary>
    public class CategoryTxt
    {
        /// <summary>
        /// Name
        /// </summary>
        public LangResponse Name { get; set; } = null!;

        /// <summary>
        /// Desc
        /// </summary>
        public LangResponse? Desc { get; set; }
    }

    /// <summary>
    /// 텍스트
    /// </summary>
    /// <value></value>
    public CategoryTxt Txt { get; set;} = null!;

    /// <summary>
    /// 
    /// </summary>
    public class CategoryOption
    {
            
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        [JsonPropertyName("name")]
        public string Name { get; set;} = null!;

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        [JsonPropertyName("parent")]
        public string Parent { get; set;} = null!;

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        [JsonPropertyName("depth")]
        public int Depth { get; set;}
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [JsonPropertyName("market_availability")]
        public bool MarketAvailability { get; set;}

        /// <summary>
        /// 실시간 배치 가능 여부
        /// </summary>
        /// <returns></returns>
        [JsonPropertyName("rt_batch")]
        public bool RealTimeBatch { get; set;}

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [JsonPropertyName("extra")]
        public JsonObject? Extra { get; set;}
    }

    /// <summary>
    /// 
    /// </summary>
    /// <value></value>
    public CategoryOption Option { get; set;} = null!;

    /// <summary>
    /// Stat 정보
    /// </summary>
    public DateResponse Stat { get; set; } = null!;
}