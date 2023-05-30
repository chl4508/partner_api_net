using Colorverse.Application.Mediator;

namespace Colorverse.MetaAdmin.Apis.DataTypes.Item;

/// <summary>
/// 
/// </summary>
public class GetItemRequest : RequestBase<ItemResponse>
{
    /// <summary>
    /// 
    /// </summary>
    public string Itemid { get; set; } = null!;
}
