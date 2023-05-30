using Colorverse.Meta.Config;
using Colorverse.Meta.Nats;
using CvFramework.Nats;
using CvFramework.Nats.EventListeners;

namespace Colorverse.Meta.Worker;

/// <summary>
/// 
/// </summary>
public class NatsEventWorker : IHostedService
{
    protected readonly ILogger _logger;
    protected readonly INatsConnectionFactory _connectionFactory;
    private readonly NatsEventListener _natsEventListener;
    private AppConfig _config;
    private readonly NatsEventDispatcher _natsEventDispatcher;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="loggerFactory"></param>
    /// <param name="connectionFactory"></param>
    /// <param name="config"></param>
    /// <param name="natsEventDispatcher"></param>
    public NatsEventWorker(
        ILoggerFactory loggerFactory,
        INatsConnectionFactory connectionFactory,
        AppConfig config,
        NatsEventDispatcher natsEventDispatcher)
    {
        _logger = loggerFactory.CreateLogger(this.GetType());
        _connectionFactory = connectionFactory;
        _config = config;
        _natsEventDispatcher = natsEventDispatcher;
        _natsEventListener = new NatsEventListener(loggerFactory, connectionFactory);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        StartSocialListener();

        return Task.CompletedTask;
    }

    /// <summary>
    /// 
    /// </summary>
    private void StartSocialListener()
    {
        var natsQueueGroup = _config.Current.NatsQueueGroup!;
        var socialNatsEventSubject = _config.Social.NatsEvent;

        _natsEventListener.SubscribeAsync( 
            $"{socialNatsEventSubject}.comment.item", 
            natsQueueGroup, 
            _natsEventDispatcher.Dispatch
        );

        _natsEventListener.SubscribeAsync( 
            $"{socialNatsEventSubject}.reaction.item", 
            natsQueueGroup, 
            _natsEventDispatcher.Dispatch
        );

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _natsEventListener.DisposeAsync();
    }

}
