using Colorverse.Application;
using Colorverse.Common.Config;
using Colorverse.Meta.Apis;

namespace Colorverse.Meta.Config;

/// <summary>
/// 
/// </summary>
public class AppConfig : BaseAppConfig<MetaConfig>
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override string Name => "Meta";
    
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

    /// <summary>
    /// 
    /// </summary>
    public SocialConfig Social { get; private set; } = null!;

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

    /// <summary>
    /// 
    /// </summary>
    [ConfigurationKeyName("aws")]
    public AwsConfig? Aws { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    [ConfigurationKeyName("asset_s3")]
    public AwsS3Config? AssetS3 { get; private set; }

}


/// <summary>
/// 
/// </summary>
public class SocialConfig
{
    /// <summary>
    /// 
    /// </summary>
    /// <value></value>
    [ConfigurationKeyName("nats_event")]
    public string NatsEvent { get; private set; } = null!;

}

public class AwsConfig
{
    /// <summary>
    /// 
    /// </summary>
    [ConfigurationKeyName("access_key")]
    public string? AccessKey { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [ConfigurationKeyName("secret_key")]
    public string? SecretKey { get; set; }
}

public class AwsS3Config
{
    /// <summary>
    /// 
    /// </summary>
    [ConfigurationKeyName("region")]
    public string Region { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    [ConfigurationKeyName("bucket")]
    public string Bucket { get; set; } = null!;
}
