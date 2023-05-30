using Colorverse.MetaAdmin.Apis.DataTypes.Asset;
using CvFramework.Excel.Attributes;
using MongoDB.Bson;
using System.Runtime.Serialization;

namespace Colorverse.MetaAdmin.Excel;

/// <summary>
/// 
/// </summary>
[ExcelDocument(SkipRow = 1)]
public class AssetWorkSheet
{
    /// <summary>
    /// 
    /// </summary>
    /// <value></value>
    [DataMember(Name = "asset")]
    public AssetRow[] Asset { get; set; } = Array.Empty<AssetRow>();
}

/// <summary>
/// 
/// </summary>
public class AssetRow
{
    /// <summary>
    /// 
    /// </summary>
    [ExcelColumn("targetid", Primary = true)]
    public string? Assetid { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [ExcelColumn("use_status", NullAllow = true)]
    public string? UseStatus { get; set; } = "N";

    /// <summary>
    /// 
    /// </summary>
    [ExcelColumn("asset_type")]
    public string? AssetType { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public BsonDocument? Manifest { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int Version { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string Profileid { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    public string Worldid { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    public string Userid { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    [ExcelColumn("title", NullAllow = true, DefaultMode = DefaultModeFlag.StringEmpty)]
    public string? Title { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [ExcelColumn("desc", NullAllow = true, DefaultMode = DefaultModeFlag.StringEmpty)]
    public string? Desc { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [ExcelColumn("hashtag", NullAllow = true, DefaultMode = DefaultModeFlag.StringEmpty)]
    public string? Hashtag { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [ExcelColumn("category1")]
    public string Category1 { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    [ExcelColumn("category2")]
    public string Category2 { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    [ExcelColumn("category3")]
    public string Category3 { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    [ExcelColumn("sale_status")]
    public string? SaleStatus { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [ExcelColumn("price_type")]
    public string? PriceType { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [ExcelColumn("price_amount", NullAllow = true)]
    public string? PriceAmount { get; set; } = "0";

    /// <summary>
    /// 
    /// </summary>
    [ExcelColumn("client_itemid", NullAllow = true, DefaultMode = DefaultModeFlag.StringEmpty)]
    public string? ClientItemid { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    public void SetAsset(CreateAssetForceRequest request)
    {
        Assetid = request.Assetid;
        UseStatus = request.UseStatus;
        Title = request.Title;
        Desc = request.Desc;
        Hashtag = request.HashTag;
        Category1 = request.Category1.ToString();
        Category2 = request.Category2.ToString();
        Category3 = request.Category3.ToString();
        AssetType = request.AssetType.ToString();
        SaleStatus = request.SaleStatus.ToString();
        PriceType = request.PriceType.ToString();
        PriceAmount = request.PriceAmount.ToString();
        ClientItemid = request.ClientItemid;
        Version = request.Version;
    }
}
