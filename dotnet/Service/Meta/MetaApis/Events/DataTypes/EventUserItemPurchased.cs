using Colorverse.Application.Mediator;
using CvFramework.Common;

namespace Colorverse.Meta.Events.DataTypes;

/// <summary>
/// 
/// </summary>
public class EventUserItemPurchased : NotificationBase
{
    public Uuid Profileid { get;}

    public ICollection<Uuid> Itemids { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="profileid"></param>
    /// <param name="itemids"></param>
    public EventUserItemPurchased(Uuid profileid, ICollection<Uuid> itemids)
    {
        Profileid = profileid;
        Itemids = itemids;
    }
}
