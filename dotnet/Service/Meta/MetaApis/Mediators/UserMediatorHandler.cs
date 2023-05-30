using Colorverse.Application.Session;
using Colorverse.Application.Mediator;
using Colorverse.Database;
using CvFramework.MongoDB.Extensions;
using MongoDB.Driver;
using MongoDB.Bson;
using Colorverse.Meta.Apis.DataTypes.Item;
using Colorverse.Common.DataTypes;

namespace Colorverse.Meta.Mediators;

/// <summary>
/// 유저아이템 Mediator
/// </summary>
[MediatorHandler]
public class UserMediatorHandler : MediatorHandlerBase,
    IMediatorHandler<GetUserItemListRequest, CvListResponse<UserItemResponse>>
{
    private readonly IContextUserProfile _profile;
    private readonly IDbContext _dbContext;

    /// <summary>
    /// 유저 아이템 Mediator 설정
    /// </summary>
    /// <param name="profile"></param>
    /// <param name="dbContext"></param>
    public UserMediatorHandler(IContextUserProfile profile, IDbContext dbContext)
    {
        _profile = profile;
        _dbContext = dbContext;
    }

    /// <summary>
    /// 유저아이템 목록 조회
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<CvListResponse<UserItemResponse>> Handle(GetUserItemListRequest request, CancellationToken cancellationToken)
    {
        var filter = Builders<UserItemResponse>.Filter.Eq("profileid", _profile.ProfileId.Guid.ToBsonBinaryData());
        if (request.Category != null)
        {
            filter &= Builders<UserItemResponse>.Filter.In("category", new BsonArray(request.Category));
        }

        var options = new FindOptions<UserItemResponse>
        {
            Skip = request.Page - 1,
            Limit = request.Limit
        };

        var userItemList = await _dbContext.GetListAsync("user_item", filter, options, cancellationToken);
        var Count = new CvListCountResponse
        {
            Current = request.Page,
            Page = request.Page,
            Limit = request.Limit,
        };
        var response = new CvListResponse<UserItemResponse>(Count, userItemList);
        return response;
    }
}