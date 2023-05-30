using CvFramework.Common;
using CvFramework.Nats;

namespace Colorverse.Meta.Events.DataTypes;

/// <summary>
/// 
/// </summary>
public class EventItemLiked : NatsEventNotificationBase
{
    public Uuid Id { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    public EventItemLiked(NatsReceiveRequest request) : base(request)
    {
        Id = Uuid.FromBase62(request.GetParamValue("id").AsString);
    }
}
