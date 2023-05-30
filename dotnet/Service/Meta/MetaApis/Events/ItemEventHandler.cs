using Colorverse.Common;
using Colorverse.Application.Mediator;
using Colorverse.Database;
using Colorverse.Meta.Documents;
using Colorverse.Meta.Events.DataTypes;
using CvFramework.Common;

namespace Colorverse.Meta.Events;

/// <summary>
/// 
/// </summary>
public class ItemEventHandler :
    IEventrHandler<EventItemLiked>,
    IEventrHandler<EventItemCommented>
{ 
    private ILogger _logger;
    private readonly IDbContext _dbContext;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="dbContext"></param>
    public ItemEventHandler(ILogger<ItemEventHandler> logger, IDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="notification"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task Handle(EventItemCommented notification, CancellationToken cancellationToken)
    {
        var targetId = notification.Id;
        if(!targetId.IsDomainType(SvcDomainType.MetaAsset) 
            && !targetId.IsDomainType(SvcDomainType.MetaItem))
        {
             _logger.LogWarning("Invalid target. id={0}, domainType={1}", targetId, notification.Id.DomainType);
            return;
        }

        if(targetId.IsDomainType(SvcDomainType.MetaAsset))
        {
            var asset = await _dbContext.GetDocAsync<AssetDoc>(targetId, cancellationToken);
            if(asset.Exists)
            {
                asset.UpdateCommented();
                await asset.SaveAsync();
            }
            else
            {
                _logger.LogWarning("Asset notfound. id={0}", targetId);
            }
        }
        else
        {
            var item = await _dbContext.GetDocAsync<ItemDoc>(targetId, cancellationToken);
            if(item.Exists)
            {
                item.UpdateCommented();
                await item.SaveAsync();
            }
            else
            {
                _logger.LogWarning("Item notfound. id={0}", targetId);
            }
        }

        var product = await _dbContext.GetDocAsync<MarketProductDoc>(targetId, cancellationToken);
        if(product.Exists)
        {
            product.UpdateCommented();
            await product.SaveAsync();
        }
        
        await notification.Finish();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="notification"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task Handle(EventItemLiked notification, CancellationToken cancellationToken)
    {
        var targetId = notification.Id;
        if(!targetId.IsDomainType(SvcDomainType.MetaAsset) 
            && !targetId.IsDomainType(SvcDomainType.MetaItem))
        {
            return;
        }

        if(targetId.IsDomainType(SvcDomainType.MetaAsset))
        {
            var asset = await _dbContext.GetDocAsync<AssetDoc>(targetId, cancellationToken);
            if(asset.Exists)
            {
                asset.UpdateLiked();
                await asset.SaveAsync();
            }
        }
        else
        {
            var item = await _dbContext.GetDocAsync<ItemDoc>(targetId, cancellationToken);
            if(item.Exists)
            {
                item.UpdateLiked();
                await item.SaveAsync();
            }
        }

        var product = await _dbContext.GetDocAsync<MarketProductDoc>(targetId, cancellationToken);
        if(product.Exists)
        {
            product.UpdateLiked();
            await product.SaveAsync();
        }
        
        await notification.Finish();
    }
}
