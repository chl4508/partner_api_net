using Colorverse.Application.Mediator;
using CvFramework.Common;
using MongoDB.Bson;

namespace Colorverse.MetaAdmin.DataTypes.Excel;

/// <summary>
/// 
/// </summary>
public class CreateExcelRequest : RequestBase<string>
{
    /// <summary>
    /// 
    /// </summary>
    public Uuid Profileid { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    public Uuid Worldid { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    public Uuid Userid { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    public string Targetid { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    public int Version { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string Title { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    public string Desc { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    public string[] Hashtag { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    public string[] Category { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    public int AssetType { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int SaleStatus { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int PriceType { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int PriceAmount { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string ReqeustId { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    public BsonDocument? Manifest { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string? ClientItemid { get; set; }
}

/// <summary>
/// 
/// </summary>
public class DeleteExcelParam
{
    /// <summary>
    /// 
    /// </summary>
    public string Targetid { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string ReqeustId { get; set; } = null!;
}
