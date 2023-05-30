using Colorverse.Application.Mediator;
using CvFramework.Common;
using CvFramework.Nats;

namespace Colorverse.Meta.Events.DataTypes;

public abstract class NatsEventNotificationBase : NotificationBase
{
    public NatsReceiveRequest Request { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    protected NatsEventNotificationBase(NatsReceiveRequest request)
    {
        Request = request;
    }

    /// <summary>
    /// 
    /// </summary>
    public async Task Finish(bool natsAck = true)
    {
        await OnFinish();
        if(natsAck)
        {
            Request.GetMsg()?.Ack();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public virtual Task OnFinish()
    {
        return Task.CompletedTask;
    }

}

/// <summary>
/// 
/// </summary>
public class EventItemCommented : NatsEventNotificationBase
{
    public Uuid Id { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    public EventItemCommented(NatsReceiveRequest request) : base(request)
    {
        Id = Uuid.FromBase62(request.GetParamValue("id").AsString);
    }
}
