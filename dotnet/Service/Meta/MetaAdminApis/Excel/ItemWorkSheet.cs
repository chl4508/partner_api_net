using Colorverse.Common.Exceptions;
using Colorverse.MetaAdmin.Apis.DataTypes.Item;
using CvFramework.Excel.Attributes;
using MongoDB.Bson;

namespace Colorverse.MetaAdmin.Excel;

/// <summary>
/// 
/// </summary>
[ExcelDocument(SkipRow = 1)]
public class ItemWorkSheet
{
    /// <summary>
    /// 
    /// </summary>
    /// <value></value>
    [ExcelSheet("item")]
    public ItemRow[] Item { get; set; } = Array.Empty<ItemRow>();
}

/// <summary>
/// 
/// </summary>
public class ItemRow
{
    /// <summary>
    /// 
    /// </summary>
    [ExcelColumn("targetid", Primary = true)]
    public string? Itemid { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [ExcelColumn("use_status", NullAllow = true)]
    public string? UseStatus { get; set; } = "N";

    /// <summary>
    /// 아이템은 애셋타입 1로 고정
    /// </summary>
    public string AssetType { get; set; } = "1";

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
    public void ItemWorkSheetValidation(ItemRow row)
    {
        if (row.Itemid == null) throw new ErrorInvalidParam("itemid : " + row.Itemid);
        if (row.Category1 == null) throw new ErrorInvalidParam("Category1 : " + row.Category1);
        if (row.Category2 == null) throw new ErrorInvalidParam("Category2 : " + row.Category2);
        if (row.Category3 == null) throw new ErrorInvalidParam("Category3 : " + row.Category3);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    public void SetItem(CreateItemForceRequest request)
    {
        Itemid = request.Itemid;
        UseStatus = request.UseStatus;
        Title = request.Title;
        Desc = request.Desc;
        Hashtag = request.HashTag;
        Category1 = request.Category1.ToString();
        Category2 = request.Category2.ToString();
        Category3 = request.Category3.ToString();
        SaleStatus = request.SaleStatus.ToString();
        PriceType = request.PriceType.ToString();
        PriceAmount = request.PriceAmount.ToString();
        ClientItemid = request.ClientItemid;
        Version = request.Version;
    }
}
