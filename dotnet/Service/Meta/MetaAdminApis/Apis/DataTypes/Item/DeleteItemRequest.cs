using Colorverse.Application.Mediator;

namespace Colorverse.MetaAdmin.Apis.DataTypes.Item;

/// <summary>
/// 
/// </summary>
public class DeleteItemRequest : RequestBase<ItemResponse>
{
    /// <summary>
    /// 
    /// </summary>
    public string Itemid { get; set; } = null!;
}
