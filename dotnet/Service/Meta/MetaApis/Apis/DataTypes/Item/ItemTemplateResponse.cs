using Colorverse.Common.Apis.DataTypes;
using CvFramework.Common;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace Colorverse.Meta.Apis.DataTypes.Item;

/// <summary>
/// 
/// </summary>
[BsonIgnoreExtraElements]
public class ItemTemplateResponse
{
    /// <summary>
    /// 아이템 아이디
    /// </summary>
    [JsonPropertyName("_id")]
    public Uuid Id { get; set; } = null!;

    /// <summary>
    /// 프로필 아이디
    /// </summary>
    public Uuid Profileid { get; set; } = null!;

    /// <summary>
    /// 컨텐츠 월드 아이디
    /// </summary>
    public Uuid Worldid { get; set; } = null!;

    /// <summary>
    /// manifestjson의 애셋타입 (item, avatar, land... )
    /// </summary>
    public int AssetType { get; set; }

    /// <summary>
    /// 제목,상세내용
    /// </summary>
    public ItemTemplateTxtResponse Txt { get; set; } = null!;

    /// <summary>
    /// 아이템 Resource 정보
    /// </summary>
    public ItemTemplateResourceResponse Resource { get; set; } = null!;

    /// <summary>
    /// option
    /// </summary>
    public ItemTemplateOptionResponse Option { get; set; } = null!;

    /// <summary>
    /// 아이템 생성및수정 날짜
    /// </summary>
    public DateResponse Stat { get; set; } = null!;
}

/// <summary>
/// 
/// </summary>
[BsonIgnoreExtraElements]
public class ItemTemplateOptionResponse
{
    /// <summary>
    /// 버전
    /// </summary>
    public int Version { get; set; }

    /// <summary>
    /// 아이템 카테고리 정보
    /// </summary>
    public string[] Category { get; set; } = null!;
}

/// <summary>
/// 
/// </summary>
[BsonIgnoreExtraElements]
public class ItemTemplateResourceResponse
{
    /// <summary>
    /// Manifest Url
    /// </summary>
    public string Manifest { get; set; } = null!;

    /// <summary>
    /// Thumbnail Url
    /// </summary>
    public string? Thumbnail { get; set; }

    /// <summary>
    /// Image cdn url
    /// </summary>
    public string? Preview { get; set; }
}

/// <summary>
/// 
/// </summary>
[BsonIgnoreExtraElements]
public class ItemTemplateTxtResponse
{
    /// <summary>
    /// 아이템 제목
    /// </summary>
    public LangResponse Title { get; set; } = null!;

    /// <summary>
    /// 아이템 설명
    /// </summary>
    public LangResponse? Desc { get; set; }

    /// <summary>
    /// 태그 최대 6개
    /// </summary>
    public string[] Hashtag { get; set; } = null!;
}