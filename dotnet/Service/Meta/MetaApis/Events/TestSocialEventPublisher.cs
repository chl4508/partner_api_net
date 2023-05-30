using Colorverse.Meta.Config;
using CvFramework.Common;
using CvFramework.MongoDB.Extensions;
using CvFramework.Nats;
using CvFramework.Nats.EventPublisher;

namespace Colorverse.Meta.Events;

/// <summary>
/// 
/// </summary>
public interface ITestSocialEventPublisher : INatsEventPublisher
{
    bool ItemCommented(Uuid itemId);

    bool ItemLiked(Uuid itemId);
}

/// <summary>
/// 
/// </summary>
public class TestSocialEventPublisherImpl : NatsEventPublisherBase, ITestSocialEventPublisher
{
    private readonly ILogger _logger;
    private AppConfig _config;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="config"></param>
    /// <param name="connectionFactory"></param>
    /// <returns></returns>
    public TestSocialEventPublisherImpl(ILogger<TestSocialEventPublisherImpl> logger, AppConfig config, INatsConnectionFactory connectionFactory) : base(connectionFactory)
    {
        _logger = logger;
        _config = config;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected override void OnBoot()
    {
        // var social = _config.Social.NatsEvent;

        // var jsm = CreateJetStreamManagementContext();
        // UpdateStream(jsm, "social", $"{social}.>", StorageType.Memory);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="itemId"></param>
    public bool ItemCommented(Uuid itemId)
    {
        try
        {
            var request = new NatsSendRequest("1", "social/comment/item");
            request.AddParam("id", itemId);

            var socialNatsEventSubject = _config.Social.NatsEvent;

            using(var conn = _connectionFactory.CreateConnection())
            {
                conn.Publish($"{socialNatsEventSubject}.comment.item", request);
            }
        }
        catch(Exception e)
        {
            _logger.LogError(e, "Nats publish fail.");
            return false;
        }
        return true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="itemId"></param>
    /// <returns></returns>
    public bool ItemLiked(Uuid itemId)
    {
        try
        {
            var request = new NatsSendRequest("1", "social/reaction/item");
            request.AddParam("id", itemId.ToBsonBinaryData());

            var socialNatsEventSubject = _config.Social.NatsEvent;

            using(var conn = _connectionFactory.CreateConnection())
            {
                conn.Publish($"{socialNatsEventSubject}.reaction.item", request);
            }
        }
        catch(Exception e)
        {
            _logger.LogError(e, "Nats publish fail.");
            return false;
        }
        return true;
    }
}