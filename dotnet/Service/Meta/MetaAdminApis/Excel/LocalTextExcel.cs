using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Colorverse.MetaAdmin.Excel;

/// <summary>
/// 
/// </summary>
public class LocalTextExcel
{
    /// <summary>
    /// 
    /// </summary>
    /// <value></value>
    [DataMember(Name = "local_sheet")]
    public IDictionary<string, LangTextRow> Texts { get; private set; } = ImmutableDictionary.Create<string, LangTextRow>();

}

/// <summary>
/// 
/// </summary>
public class LangTextRow
{
    [Key, DataMember(Name="Code")]
    public int Id { get; set;}

    [DataMember(Name="KR")]
    public string Kr { get; set;} = null!;

    [DataMember(Name="EN")]
    public string En { get; set;} = null!;
}
