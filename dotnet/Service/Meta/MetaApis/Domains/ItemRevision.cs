using CvFramework.Common;
using MongoDB.Bson;

namespace Colorverse.Meta.Domains;

/// <summary>
/// ItemRevision
/// </summary>
public class ItemRevision
{
    /// <summary>
    /// item revison id
    /// </summary>
    public Uuid Id { get; set; } = null!;

    /// <summary>
    /// 아이템아이디
    /// </summary>
    public Uuid Itemid { get; set; } = null!;

    /// <summary>
    /// 버전
    /// </summary>
    public int Version { get; set; }

    /// <summary>
    /// 상태값 ( 0 : 저장됨, 1 : 판매허용)
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public BsonDocument? Manifest { get; set; }

    /// <summary>
    /// Stat
    /// </summary>
    public ItemRevisionStat Stat { get; set; } = null!;
}

/// <summary>
/// ItemRevisionStat
/// </summary>
public class ItemRevisionStat
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
