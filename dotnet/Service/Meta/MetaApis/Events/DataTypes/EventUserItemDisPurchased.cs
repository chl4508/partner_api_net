using Colorverse.Application.Mediator;
using CvFramework.Common;

namespace Colorverse.Meta.Events.DataTypes;

/// <summary>
/// 
/// </summary>
public class EventUserItemDisPurchased : NotificationBase
{
    public Uuid Profileid { get; }

    public int Count { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="profileid"></param>
    /// <param name="count"></param>
    public EventUserItemDisPurchased(Uuid profileid, int count)
    {
        Profileid = profileid;
        Count = count;
    }
}
