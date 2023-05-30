using Colorverse.Common.Config;
using Colorverse.Application;
using Colorverse.MetaAdmin.Apis;

namespace Colorverse.MetaAdmin.Config;

/// <summary>
/// 
/// </summary>
public class AppConfig : BaseAppConfig<MetaConfig>
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override string Name => "MetaAdmin";

    /// <summary>
    /// 
    /// </summary>
    public override string ApiPath => CvRouteAttribute.SERVICE_NAME;

    /// <summary>
    /// 
    /// </summary>
    public override string ApiVersion => CvRouteAttribute.API_VERSION;

    /// <summary>
    /// 
    /// </summary>
    public CommonConfig Common { get; private set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    public override MetaConfig Current => Meta ?? new MetaConfig();

    /// <summary>
    /// 
    /// </summary>
    public MetaConfig Meta { get; private set; } = null!;
}

/// <summary>
/// 
/// </summary>
public class MetaConfig
{
    /// <summary>
    /// 
    /// </summary>
    public MongoDbConfig MongoDb { get; private set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    [ConfigurationKeyName("nats_event")]
    public string NatsEvent { get; private set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    [ConfigurationKeyName("nats_queue_group")]
    public string NatsQueueGroup { get; private set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    [ConfigurationKeyName("nats_subject")]
    public string NatsSubject { get; private set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    [ConfigurationKeyName("rest_port")]
    public ushort RestPort { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    [ConfigurationKeyName("sentry_url")]
    public string? SentryUrl { get; private set; }
}