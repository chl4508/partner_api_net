using System.Text.Json.Serialization;

namespace Colorverse.MetaAdmin.Apis.DataTypes.Excel;

/// <summary>
/// 
/// </summary>
public class WorkSheetResponse
{
    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("successids")]
    public IList<string>? SuccessIds { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("errorids")]
    public IList<string>? ErrorIds { get; set; }
}
