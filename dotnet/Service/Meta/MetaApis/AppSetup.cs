using Colorverse.Aws.S3;
using Colorverse.Common.Exceptions;
using Colorverse.Database.Extensions;
using Colorverse.Database.Mongo;
using Colorverse.Database.Redis;
using Colorverse.Meta.Config;
using Colorverse.Meta.Events;
using Colorverse.Meta.Nats;
using Colorverse.Meta.Worker;
using Colorverse.Nats;
using Colorverse.Nats.RequestReply;
using Colorverse.ResourceLibrary.Extensions;
using CvFramework.Aws.S3;
using CvFramework.Caching.Extensions;
using CvFramework.MongoDB;
using CvFramework.MongoDB.Cache;
using CvFramework.Nats;
using CvFramework.Redis;
using MetaLibrary.Extensions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using UserLibrary.Extensions;

namespace Colorverse.Meta;

/// <summary>
/// 
/// </summary>
public static class AppSetup
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static void Setup(WebApplicationBuilder builder)
    {
        SetupDatabase(builder.Services);
        SetupCaching(builder.Services);
        SetupNats(builder);
        SetupLibrary(builder.Services);
        SetupAws(builder);
        SetupFormFile(builder.Services);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    private static void SetupDatabase(IServiceCollection services)
    {
        // Redis
        services.AddSingleton<IRedisConnectionFactory>(sp => {
            var config = sp.GetRequiredService<AppConfig>();
            return new RedisConnectionFactoryImpl(config.Common.Redis);
        });

        // Mongo
        services.AddSingleton<IMongoCache>(sp => {
            var redis = sp.GetRequiredService<IRedisConnectionFactory>();
            return new MongoCacheImpl(redis);
        });

        services.AddSingleton<IMongoDb>(sp => {
            var cache = sp.GetRequiredService<IMongoCache>();
            var config = sp.GetRequiredService<AppConfig>();
            return new MongoDbImpl(config.Common.MongoDb, config.Current.MongoDb, cache);
        });

        // DbContext
        services.AddDbContext();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    private static void SetupCaching(IServiceCollection services)
    {
        services.AddDistributedCaching((sp, options) => {
            var config = sp.GetRequiredService<AppConfig>();
            options.InstanceName = config.ApiPath.ToLower()+"-cache:";
            options.Configuration = config.Common.Redis.Uri;
        });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="builder"></param>
    private static void SetupNats(WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<INatsConnectionFactory>(sp => {
            var config = sp.GetRequiredService<AppConfig>();
            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            return new NatsConnectionFactoryImpl(loggerFactory, config.Common.Nats.GetConnectionString());
        });
        
        builder.Services.AddNatsRequestReplyDispatcher<NatsRequestDispatcher>(sp => {
            var config = sp.GetRequiredService<AppConfig>();
            var current = config.Current;
            return new NatsRequestReplyOptions(current.NatsSubject, current.NatsQueueGroup);
        });

        builder.Services.AddHostedService<NatsEventWorker>();
        builder.Services.AddSingleton<ITestSocialEventPublisher, TestSocialEventPublisherImpl>();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    private static void SetupLibrary(IServiceCollection services)
    {
        services.AddResourceLibrary();
        services.AddMetaLibrary();
        services.AddUserLibrary();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="builder"></param>
    private static void SetupAws(WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IAwsS3Factory>(sp => {
            var config = sp.GetRequiredService<AppConfig>();
            if (config.Meta.Aws == null)
            {
                throw new ErrorInternal($"Invalid config. config.Meta.Aws");
            }
            if (config.Meta.AssetS3 == null)
            {
                throw new ErrorInternal($"Invalid config. config.Meta.AssetS3");
            }

            var awsConfig = config.Meta.Aws;
            var s3Config = config.Meta.AssetS3;
            var options = new AwsS3ConfigOptions
            {
                AccessKey = awsConfig.AccessKey,
                SecretKey = awsConfig.SecretKey,
                Region = s3Config.Region,
                Bucket = s3Config.Bucket
            };
            var factory = new AwsS3FactoryImpl("asset", options);
            return factory;
        });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    private static void SetupFormFile(IServiceCollection services)
    {
        services.Configure<KestrelServerOptions>(option => {
            option.Limits.MaxRequestBodySize = 1073741824;
        });

        services.Configure<FormOptions>(option => {
            option.MultipartBodyLengthLimit = 1073741824;
        });
    }

}

