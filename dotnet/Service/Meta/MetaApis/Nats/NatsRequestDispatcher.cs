using Colorverse.Common.Exceptions;
using Colorverse.Meta.DataTypes.Resource;
using Colorverse.Nats.RequestReply;
using CvFramework.Common;
using CvFramework.Nats;
using MediatR;

namespace Colorverse.Meta.Nats;

/// <summary>
/// 
/// </summary>
public class NatsRequestDispatcher : NatsRequestReplyDispatcherBase
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    
    public NatsRequestDispatcher(IServiceScopeFactory serviceScopeFactory, ILogger<NatsRequestDispatcher> logger) : base(logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    /// <see cref="Mediators.ResourceMediatorHandler.Handle(GetResourceParam, CancellationToken)"/>
    /// <see cref="Mediators.ResourceMediatorHandler.Handle(UseResourceParam, CancellationToken)"/>
    /// <see cref="Mediators.ResourceMediatorHandler.Handle(DeleteResourceParam, CancellationToken)"/>
    public override async Task<object?> OnDispatch(NatsReceiveRequest request)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        switch(request.Uri)
        {
            case "resource/get":
            {
                var id = Uuid.FromGuid(request.GetParamValue("id").AsGuid);
                var version = request.GetParamValue("version").AsInt32;

                var response = await mediator.Send(new GetResourceParam(id, version));
                return response;
            }
            case "resource/use":
            {
                var id = Uuid.FromGuid(request.GetParamValue("id").AsGuid);
                var version = request.GetParamValue("version").AsInt32;

                var response = await mediator.Send(new UseResourceParam(id, version));
                return response;
            }
            case "resource/delete":
            {
                var id = Uuid.FromGuid(request.GetParamValue("id").AsGuid);
                var version = request.GetParamValue("version").AsInt32;

                var response = await mediator.Send(new DeleteResourceParam(id, version));
                return response;
            }
        }
        throw new ErrorMethodNotAllowed();
    }
}