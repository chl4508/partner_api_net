using System.Text.Json.Serialization;
using Colorverse.Apis.Controller.Models;
using Colorverse.Application.Mediator;

namespace Colorverse.Meta.Apis.DataTypes.ItemCategory;

/// <summary>
/// CreateItemRequest
/// </summary>
public class GetItemCategoryListRequest : RequestBase<ListResponse<ItemCategoryResponse>>
{
    /// <summary>
    /// 
    /// </summary>
    /// <value></value>
    [JsonPropertyName("depth")]
    public int? Depth { get;}

    /// <summary>
    /// 
    /// </summary>
    /// <value></value>
    [JsonPropertyName("parentid")]
    public string? ParentId { get;}

    /// <summary>
    /// 
    /// </summary>
    /// <param name="depth"></param>
    /// <param name="parentId"></param>
    public GetItemCategoryListRequest(int? depth, string? parentId)
    {
        Depth = depth;
        ParentId = parentId;
    }
}