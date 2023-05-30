using Colorverse.Database.Extensions;
using Colorverse.Database.Mongo;
using Colorverse.Database.Redis;
using Colorverse.MetaAdmin.Config;
using Colorverse.Nats;
using Colorverse.ResourceLibrary.Extensions;
using CvFramework.Bson;
using CvFramework.Caching.Extensions;
using CvFramework.GoogleDrive.Extensions;
using CvFramework.MongoDB;
using CvFramework.MongoDB.Cache;
using CvFramework.Nats;
using CvFramework.Redis;
using CvFramework.Worksheet.Extensions;
using UserLibrary.Extensions;

namespace Colorverse.MetaAdmin;

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
    public static void Setup(this WebApplicationBuilder builder)
    {
        SetupDatabase(builder.Services);
        SetupCaching(builder.Services);
        SetupNats(builder.Services);
        SetupLibrary(builder.Services);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    private static void SetupDatabase(IServiceCollection services)
    {
        // MongoDb
        BsonInitializer.Init();

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
    /// <param name="services"></param>
    private static void SetupNats(IServiceCollection services)
    {
        services.AddSingleton<INatsConnectionFactory>(sp => {
            var config = sp.GetRequiredService<AppConfig>();
            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            return new NatsConnectionFactoryImpl(loggerFactory, config.Common.Nats.GetConnectionString());
        });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    private static void SetupLibrary(IServiceCollection services)
    {
        services.AddGoogleDrive(Path.Combine(Environment.CurrentDirectory, "config", "google-servicekey.json"));
        services.AddWorksheetLoader(Path.Combine(Environment.CurrentDirectory, "config", "google-servicekey.json"));
        services.AddUserLibrary();
        services.AddResourceLibrary();
    }

}

