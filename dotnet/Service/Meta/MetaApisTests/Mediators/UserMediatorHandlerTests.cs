//using Colorverse.ApisTests;
//using Colorverse.Application.Session;
//using Colorverse.Common;
//using Colorverse.Database;
//using Colorverse.Meta.DataTypes.Asset;
//using Colorverse.Meta.DataTypes.Item;
//using Colorverse.Meta.Domains;
//using MediatR;
//using MongoDB.Driver;
//using Moq;
//using Xunit;

//namespace Colorverse.Meta.Mediators.Tests;

///// <summary>
///// 
///// </summary>
//public class UserMediatorHandlerTests : BaseTests
//{
//    private readonly IContextUserProfile _userProfile;


//    public UserMediatorHandlerTests()
//    {
//        _userProfile = GetTestUserProfile();
//    }

//    [Fact()]
//    public async Task Handle_GetUserItemListParam()
//    {
//        //GIVEN
//        var param = new GetUserItemListParam()
//        {
//            Page = 1,
//            Limit = 3,
//        };

//        var dbContextMock = new Mock<IDbContext>();
//        dbContextMock.Setup(x => x.GetListAsync("user_item", It.IsAny<FilterDefinition<UserItem>>(), It.IsAny<FindOptions<UserItem>>(), It.IsAny<CancellationToken>()))
//            .ReturnsAsync(()=>{
//                var userItem = new UserItem()
//                {
//                    Id = UuidGenerater.NewUuid(SvcDomainType.MetaUserItem),
//                    Userid = _userProfile.UserId,
//                    Profileid = _userProfile.ProfileId,
//                    Worldid = _userProfile.WorldId,
//                    Itemid = UuidGenerater.NewUuid(SvcDomainType.MetaItem),
//                    Option = new UserItemOption() { Quantity = 1 },
//                    Category = new int[3] { 1, 10000, 10001 },
//                    Stat = new UserItemStat() { Created = DateTime.UtcNow, Updated = DateTime.UtcNow }
//                };
//                ICollection<UserItem> list = new List<UserItem> { userItem };
//                return list;
//            });
            
//        //WHEN
//        var handler = new UserMediatorHandler(_userProfile, dbContextMock.Object);
//        var result = await handler.Handle(param, CancellationToken.None);

//        //THEN
//        Assert.Single(result.List);
//    }

//    [Fact()]
//    public async Task Handle_GetUserAssetListParam()
//    {
//        //GIVEN
//        var param = new GetUserAssetListParam()
//        {
//            Page = 1,
//            Limit = 3,
//        };

//        var dbContextMock = new Mock<IDbContext>();
//        dbContextMock.Setup(x => x.GetListAsync("user_asset", It.IsAny<FilterDefinition<UserAsset>>(), It.IsAny<FindOptions<UserAsset>>(), It.IsAny<CancellationToken>()))
//            .ReturnsAsync(() => {
//                var userAsset = new UserAsset()
//                {
//                    Id = UuidGenerater.NewUuid(SvcDomainType.MetaUserAsset),
//                    Userid = _userProfile.UserId,
//                    Profileid = _userProfile.ProfileId,
//                    Worldid = _userProfile.WorldId,
//                    Assetid = UuidGenerater.NewUuid(SvcDomainType.MetaAsset),
//                    Option = new UserAssetOption() { Quantity = 1 },
//                    Category = new int[3] { 1, 10000, 10001 },
//                    Stat = new UserAssetStat() { Created = DateTime.UtcNow, Updated = DateTime.UtcNow }
//                };
//                ICollection<UserAsset> list = new List<UserAsset> { userAsset };
//                return list;
//            });

//        //WHEN
//        var handler = new UserMediatorHandler(_userProfile, dbContextMock.Object);
//        var result = await handler.Handle(param, CancellationToken.None);

//        //THEN
//        Assert.Single(result.List);
//    }
//}