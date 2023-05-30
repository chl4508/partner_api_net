using Colorverse.Common.Exceptions;
using Colorverse.Meta.Events;
using CvFramework.MongoDB;
using CvFramework.Nats;

namespace Colorverse.Meta;

public static class AppBoot
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="app"></param>
    public static void Before(WebApplication app)
    {
        ////TestMongoDb(app.Services);
        ////TestNats(app.Services);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="serviceProvider"></param>
    private static void TestMongoDb(IServiceProvider serviceProvider)
    {
        var mongoDb = serviceProvider.GetRequiredService<IMongoDb>();
        try
        {
            mongoDb.GetDatabase().ListCollectionNames();
        }
        catch(Exception e)
        {
            throw new ErrorInternal(e, $"{nameof(TestMongoDb)}(): fail.");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <exception cref="ErrorInternal"></exception>
    private static void TestNats(IServiceProvider serviceProvider)
    {
        try
        {
            var natsConnection = serviceProvider.GetRequiredService<INatsConnectionFactory>();
            using var connection = natsConnection.CreateConnection();
            connection.Publish("test", $"ServerApis {AppDomain.CurrentDomain.FriendlyName} boot.");

            serviceProvider.GetRequiredService<ITestSocialEventPublisher>().Boot();
        }
        catch (Exception e)
        {
            throw new ErrorInternal(e, $"{nameof(TestNats)}(): fail.");
        }
    }

}
