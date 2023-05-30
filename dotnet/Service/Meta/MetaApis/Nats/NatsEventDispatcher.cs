using Colorverse.Common.DependencyInjection;
using Colorverse.Meta.Events.DataTypes;
using CvFramework.Nats;
using MediatR;

namespace Colorverse.Meta.Nats;

/// <summary>
/// 
/// </summary>
[Singleton]
public class NatsEventDispatcher
{
    protected readonly IMediator _mediator;
    
    public NatsEventDispatcher(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public Task Dispatch(NatsReceiveRequest request)
    {
        switch(request.Uri)
        {
            case "social/comment/item":
                _mediator.Publish(new EventItemCommented(request));
                break;
            case "social/reaction/item":
                _mediator.Publish(new EventItemLiked(request));
                break;
        }

        return Task.CompletedTask;
    }
}