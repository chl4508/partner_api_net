using CvFramework.Excel.Attributes;

namespace Colorverse.MetaAdmin.Excel;

/// <summary>
/// 
/// </summary>
[ExcelDocument(SkipRow = 1)]
public class ItemCategoryExcel
{
    /// <summary>
    /// 
    /// </summary>
    /// <value></value>
    [ExcelSheet("EItemCategory1")]
    public IDictionary<string, ItemCategory1Row> Category1 { get; private set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    /// <value></value>
    [ExcelSheet("EItemCategory2")]
    public IDictionary<string, ItemCategory2Row> Category2 { get; private set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    /// <value></value>
    [ExcelSheet("EItemCategory3")]
    public IDictionary<string, ItemCategory3Row> Category3 { get; private set; } = null!;
}

/// <summary>
/// 
/// </summary>
public class ItemCategory1Row
{
    [ExcelColumn("Id", Primary = true)]
    public string Id { get; set;} = null!;

    [ExcelColumn("Name", NullMode = NullModeFlag.RowSkip )]
    public string Name { get; set;} = null!;

    [ExcelColumn("Text", NullAllow = true, DefaultMode = DefaultModeFlag.StringEmpty)]
    public string Text { get; set;} = null!;
    
    [ExcelColumn("Desc", NullAllow = true, DefaultMode = DefaultModeFlag.StringEmpty)]
    public string Desc { get; set;} = null!;
}

/// <summary>
/// 
/// </summary>
public class ItemCategory2Row
{
    [ExcelColumn("Id", Primary = true)]
    public string Id { get; set;} = null!;

    [ExcelColumn("Name", NullMode = NullModeFlag.RowSkip )]
    public string Name { get; set;} = null!;

    [ExcelColumn("Parent", NullMode = NullModeFlag.RowSkip )]
    public string Parent { get; set;} = null!;

    [ExcelColumn("Text", NullAllow = true, DefaultMode = DefaultModeFlag.StringEmpty)]
    public string Text { get; set;} = null!;

    [ExcelColumn("Desc", NullAllow = true, DefaultMode = DefaultModeFlag.StringEmpty)]
    public string Desc { get; set;} = null!;
}

/// <summary>
/// 
/// </summary>
public class ItemCategory3Row
{
    [ExcelColumn("Id", Primary = true)]
    public string Id { get; set;} = null!;

    [ExcelColumn("Name", NullMode = NullModeFlag.RowSkip )]
    public string Name { get; set;} = null!;

    [ExcelColumn("Parent", NullMode = NullModeFlag.RowSkip )]
    public string Parent { get; set;} = null!;

    [ExcelColumn("Text", NullAllow = true, DefaultMode = DefaultModeFlag.StringEmpty)]
    public string Text { get; set;} = null!;

    [ExcelColumn("Desc", NullAllow = true, DefaultMode = DefaultModeFlag.StringEmpty)]
    public string Desc { get; set;} = null!;
}