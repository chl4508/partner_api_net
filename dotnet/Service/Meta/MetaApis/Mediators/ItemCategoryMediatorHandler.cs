using Colorverse.Apis.Controller.Models;
using Colorverse.Application.Mediator;
using Colorverse.Application.Session;
using Colorverse.Database;
using Colorverse.Meta.Apis.DataTypes.ItemCategory;
using Colorverse.Meta.Documents;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Colorverse.Meta.Mediators;

/// <summary>
/// 카테고리 Mediator
/// </summary>
[MediatorHandler]
public class ItemCategoryMediatorHandler : MediatorHandlerBase,
    IMediatorHandler<GetItemCategoryListRequest, ListResponse<ItemCategoryResponse>>
{
    private readonly IDbContext _dbContext;
    private readonly IContextUserProfile _profile;

    public ItemCategoryMediatorHandler(IDbContext dbContext, IContextUserProfile profile)
    {
        _dbContext = dbContext;
        _profile = profile;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<ListResponse<ItemCategoryResponse>> Handle(GetItemCategoryListRequest request, CancellationToken cancellationToken)
    {
        var db = _dbContext.GetMongoDb();

        var filter = new BsonDocument();
        if(request.Depth > 0)
        {
            filter.Add("option.depth", request.Depth);
        }
        if(!string.IsNullOrWhiteSpace(request.ParentId))
        {
            filter.Add("_srch.parentid", request.ParentId);
        }

        var collection = _dbContext.GetCollection<ItemCategoryResponse>(ItemCategoryDoc.COLLECTION_NAME);
        var result = await collection.Find(filter).ToListAsync(cancellationToken);
        
        var response = new ListResponse<ItemCategoryResponse>(result);
        return response;
    }
}


