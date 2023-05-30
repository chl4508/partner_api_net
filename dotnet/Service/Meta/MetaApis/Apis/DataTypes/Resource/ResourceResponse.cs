using System.Text.Json.Serialization;
using CvFramework.Common;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Colorverse.Meta.Apis.DataTypes.Resource;

/// <summary>
/// 
/// </summary>
[BsonIgnoreExtraElements]
public class ResourceResponse
{
    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("_id")]
    [BsonElement("_id")]
    public Uuid Id { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    /// <value></value>
    public byte Type { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int? Version { get; set; }

    /// <summary>
	/// 
	/// </summary>
	public class OptionDocument
    {
        /// <summary>
        /// 
        /// </summary>
        [BsonElement("origin_file_name")]
        public string? OriginFileName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [BsonElement("file_size")]
        public long? FileSize { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [BsonElement("file_content_type")]
        public string? FileContentType { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    [BsonIgnoreExtraElements]
    public class ResourceDocument
    {
        /// <summary>
        /// Files
        /// </summary>
        /// <value></value>
        public string[]? Files { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <value></value>
    public ResourceDocument? Resource { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public OptionDocument Option { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    public class StatDocument
    {
        /// <summary>
        /// 생성 날짜
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// 수정날짜
        /// </summary>
        public DateTime Updated { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public StatDocument Stat { get; set; } = null!;
}
