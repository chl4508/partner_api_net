using Colorverse.Apis.Controller.Models;
using Colorverse.Application.Mediator;

namespace Colorverse.MetaAdmin.Apis.DataTypes.ItemCategory;

/// <summary>
/// 
/// </summary>

public class SyncItemCategory : RequestBase<ListResponse<ItemCategoryResponse>>
{
    public string? WorksheetId;
    public string? DriveId;
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="worksheetId"></param>
    /// <param name="driveId"></param>
    public SyncItemCategory(string? worksheetId, string? driveId = null)
    {
        WorksheetId = worksheetId;
        DriveId = driveId;
    }
}