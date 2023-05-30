using Colorverse.Apis.Controller;
using CvFramework.Common;
using Microsoft.AspNetCore.Mvc;
using Colorverse.Common;
using Colorverse.Meta.Events;

namespace Colorverse.Meta.Apis;

/// <summary>
/// Nats Test
/// </summary>
[CvRoute("/test/nats")]
public class TestNatsController : ApiControllerBase
{
    private IServiceProvider _serviceProvider;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="serviceProvider"></param>
    public TestNatsController(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [Obsolete("Nats Test")]
    [HttpPost("event/item/commented")]
    public IActionResult EventItemCommented(string? id)
    {
        Uuid targetId;
        if (id == null)
        {
            targetId = UuidGenerater.NewUuid(SvcDomainType.MetaItem);
        }
        else
        {
            targetId = Uuid.FromBase62(id);
        }

        var publisher = _serviceProvider.GetRequiredService<ITestSocialEventPublisher>();
        publisher.ItemCommented(targetId);
        return new JsonResult(null);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [Obsolete("Nats Test")]
    [HttpPost("event/item/liked")]
    public IActionResult EventItemLiked(string? id)
    {
        Uuid targetId;
        if (id == null)
        {
            targetId = UuidGenerater.NewUuid(SvcDomainType.MetaItem);
        }
        else
        {
            targetId = Uuid.FromBase62(id);
        }

        var publisher = _serviceProvider.GetRequiredService<ITestSocialEventPublisher>();
        publisher.ItemLiked(targetId);

        return new JsonResult(null);
    }

}

