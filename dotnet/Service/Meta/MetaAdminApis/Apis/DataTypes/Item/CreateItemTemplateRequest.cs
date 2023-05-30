using Colorverse.Application.Mediator;

namespace Colorverse.MetaAdmin.Apis.DataTypes.Item;

/// <summary>
/// 
/// </summary>
public class CreateItemTemplateRequest : RequestBase<ItemTemplateResponse>
{
    /// <summary>
    /// 아이템 아이디
    /// </summary>
    public string Itemid { get; set; } = null!;
}
