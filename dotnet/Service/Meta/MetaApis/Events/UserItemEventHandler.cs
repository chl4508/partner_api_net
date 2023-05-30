using Colorverse.Application.Mediator;
using Colorverse.Database;
using Colorverse.Meta.Events.DataTypes;
using Colorverse.UserLibrary;

namespace Colorverse.Meta.Events;

/// <summary>
/// 
/// </summary>
public class UserItemEventHandler :
    IEventrHandler<EventUserItemPurchased>,
    IEventrHandler<EventUserItemDisPurchased>
{ 
    private  ILogger _logger;
    private readonly IDbContext _dbContext;
    private  IUserLibrary _userLibrary;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="dbContext"></param>
    /// <param name="userLibrary"></param>
    public UserItemEventHandler(
        ILogger<ItemEventHandler> logger,
        IDbContext dbContext,
        IUserLibrary userLibrary)
    {
        _logger = logger;
        _dbContext = dbContext;
        _userLibrary = userLibrary;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="notification"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task Handle(EventUserItemPurchased notification, CancellationToken cancellationToken)
    {
        // User 서비스 데이터 연동
        _ = _userLibrary.UpdateItemCount(notification.Profileid, notification.Itemids.Count)
            .ContinueWith(t => {                
                if(t.Exception != null)
                {
                    _logger.LogError($"{nameof(_userLibrary.UpdateItemCount)} fail. profileid={notification.Profileid}");
                }
            });
        return Task.CompletedTask;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="notification"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task Handle(EventUserItemDisPurchased notification, CancellationToken cancellationToken)
    {
        // User 서비스 데이터 연동
        _ = _userLibrary.UpdateItemCount(notification.Profileid, notification.Count)
            .ContinueWith(t => {
                if (t.Exception != null)
                {
                    _logger.LogError($"{nameof(_userLibrary.UpdateItemCount)} fail. profileid={notification.Profileid}");
                }
            });
        return Task.CompletedTask;
    }

}
