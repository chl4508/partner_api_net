using CvFramework.Common;
using CvFramework.MongoDB.Documents;
using CvFramework.MongoDB.Domain;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Colorverse.MetaAdmin.DataTypes.Resource;

/// <summary>
/// 
/// </summary>
[BsonIgnoreExtraElements]
public class ResourceDto : BaseDoamin
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="doc"></param>
    public ResourceDto(MongoDoc doc) : base(doc)
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <value></value>
    public Uuid Id { get; private set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    /// <value></value>
    public byte Type { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    /// <value></value>
    public int Version { get; private set; }

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
    public OptionDocument Option { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    [BsonIgnoreExtraElements]
    public class ResourceDocument
    {
        /// <summary>
        /// 
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
    /// <value></value>
    public BsonDocument? Manifest { get; set; }

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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="manifest"></param>
    public ResourceDto TestData(Uuid id, BsonDocument manifest)
    {
        return new ResourceDto(null!)
        {
            Id = id,
            Type = 102,
            Version = 1,
            Option = new OptionDocument()
            {
                OriginFileName = "test",
                FileSize = 100,
                FileContentType = "application/x-zip-compressed",

            },
            Resource = new ResourceDocument()
            {
                Files = new[] { "manifest.json.zip" }
            },
            Manifest = manifest
        };
    }
}
