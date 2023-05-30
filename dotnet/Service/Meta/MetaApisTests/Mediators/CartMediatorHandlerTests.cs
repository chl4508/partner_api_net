//using Colorverse.ApisTests;
//using Colorverse.Application.Session;
//using Colorverse.Common;
//using Colorverse.Meta.DataTypes.Cart;
//using Colorverse.Meta.Documents;
//using CvFramework.MongoDB.Doc;
//using CvFramework.MongoDB.Documents;
//using CvFramework.MongoDB.Extensions;
//using MediatR;
//using MongoDB.Bson;
//using Moq;
//using Xunit;

//namespace Colorverse.Meta.Mediators.Tests;

//public class CartMediatorHandlerTests : BaseTests
//{
//    private readonly IContextUserProfile _userProfile;
//    private readonly TestDbContextImpl _dbContextMock;

//    public CartMediatorHandlerTests()
//    {
//        _userProfile = GetTestUserProfile();
//        _dbContextMock = GetTestDbContext();
//    }


//    [Fact()]
//    public async Task Handle_GetCartListParam()
//    {
//        //GIVEN
        
//        _dbContextMock.MockDocRepository.Setup(x => x.GetAsync<MongoReadOnlyDocBase>(It.IsAny<CartDoc>(), It.IsAny<CancellationToken>()))
//           .Returns<MongoReadOnlyDocBase, CancellationToken>((doc, cancellationToken) =>
//           {
//               return Task.FromResult(new MongoDocUpdated(new BsonDocument()
//               {
//                   {"_id",  _userProfile.ProfileId.Guid.ToBsonBinaryData()}
//               }));
//           });

//        var mediatorMock = new Mock<IMediator>();

//        //WHEN
//        var handler = new CartMediatorHandler(_userProfile, _dbContextMock, mediatorMock.Object);
//        var result = await handler.Handle(new GetCartListParam(), CancellationToken.None);
//        //THEN
//        Assert.Equal(_userProfile.ProfileId, result.Id);
//    }

//    [Fact()]
//    public async Task Handle_CreateCartListParam()
//    {
        
//        //GIVEN
//        var itemid = UuidGenerater.NewUuid(SvcDomainType.MetaItem);
//        var param = new CreateCartListParam
//        {
//            Items = new CreateCartItemsParam[] { new CreateCartItemsParam { Productid = itemid, PriceType = 3, PriceAmount = 10000, Category = new int[] { 1, 10000, 10001 } } }
//        };

        
//        _dbContextMock.MongoDocSetupGetAll();
//        _dbContextMock.MongoDocSetupUpdateAll();

//        var mediatorMock = new Mock<IMediator>();

//        //WHEN
//        var handler = new CartMediatorHandler(_userProfile, _dbContextMock, mediatorMock.Object);
//        var result = await handler.Handle(param, CancellationToken.None);
//        //THEN
//        Assert.Equal(_userProfile.ProfileId, result.Id);
//        Assert.Equal(itemid, result.Items![0].Productid);
//    }

//    [Fact()]
//    public async Task Handle_DeleteCartListParam()
//    {
//        //GIVEN
//        var itemid = UuidGenerater.NewUuid(SvcDomainType.MetaItem);
//        var param = new DeleteCartListParam
//        {
//            Productids = new[] { itemid }
//        };

        
//        _dbContextMock.MongoDocSetupGetAll();
//        _dbContextMock.MongoDocSetupUpdateAll();

//        var mediatorMock = new Mock<IMediator>();

//        //WHEN
//        var handler = new CartMediatorHandler(_userProfile, _dbContextMock, mediatorMock.Object);
//        var result = await handler.Handle(param, CancellationToken.None);
//        //THEN
//        Assert.Equal(_userProfile.ProfileId, result.Id);
//        Assert.Null(result.Items);
//    }

//    [Fact()]
//    public async Task Handle_PurchaseCartListParam()
//    {
//        //GIVEN
//        var itemid = UuidGenerater.NewUuid(SvcDomainType.MetaItem);
//        var param = new PurchaseCartListParam
//        {
//            Items = new PurchaseCartListItemsParam[] {new PurchaseCartListItemsParam { Productid = itemid ,PriceType = 3, PriceAmount =10000} }
//        };

        
//        _dbContextMock.MockDocRepository.Setup(x => x.GetAsync<MongoReadOnlyDocBase>(It.IsAny<CartDoc>(), It.IsAny<CancellationToken>()))
//           .Returns<MongoReadOnlyDocBase, CancellationToken>((doc, cancellationToken) =>
//           {
//               return Task.FromResult(new MongoDocUpdated(new BsonDocument()
//               {
//                   {"_id",  _userProfile.ProfileId.Guid.ToBsonBinaryData()}
//               }));
//           });
//        _dbContextMock.MockDocRepository.Setup(x => x.GetAsync<MongoReadOnlyDocBase>(It.IsAny<UserItemDoc>(), It.IsAny<CancellationToken>()))
//            .Returns<MongoReadOnlyDocBase, CancellationToken>((doc, cancellationToken) =>
//            {
//                return Task.FromResult(new MongoDocUpdated(null));
//            });
//        _dbContextMock.MockDocRepository.Setup(x => x.GetAsync<MongoReadOnlyDocBase>(It.IsAny<UserAssetDoc>(), It.IsAny<CancellationToken>()))
//           .Returns<MongoReadOnlyDocBase, CancellationToken>((doc, cancellationToken) =>
//           {
//               return Task.FromResult(new MongoDocUpdated(null));
//           });
//        _dbContextMock.MockDocRepository.Setup(x => x.GetAsync<MongoReadOnlyDocBase>(It.IsAny<MarketProductDoc>(), It.IsAny<CancellationToken>()))
//           .Returns<MongoReadOnlyDocBase, CancellationToken>((doc, cancellationToken) =>
//           {
//               return Task.FromResult(new MongoDocUpdated(new BsonDocument()
//               {
//                   {"_id",  itemid.Guid.ToBsonBinaryData()},
//                   {"ownerid",  _userProfile.ProfileId.Guid.ToBsonBinaryData()},
//                   {"type",  1},
//                   {"txt", new BsonDocument()
//                   {
//                        { "title", new BsonDocument() {
//                            { "ko", "title" } }
//                        },
//                        { "desc", new BsonDocument() {
//                            { "ko", "desc" } }
//                        },
//                        { "hashtag", new BsonArray(){
//                            {"hashtag"}
//                        }}
//                   }},
//                   {"option", new BsonDocument(){
//                        {"category",  new BsonArray(){ {1},{10000 },{10001 } } },
//                        {"price", new BsonArray  { new BsonDocument { {"type", 3 }, {"amount", 10000 } } } },
//                        {"items", new BsonArray  { new BsonDocument { {"type", 1 }, {"id", itemid.Guid.ToBsonBinaryData() } , { "quantity", 1} } } },
//                    } }
//               }));
//           });

//        _dbContextMock.MongoDocSetupUpdateAll();

//        var mediatorMock = new Mock<IMediator>();

//        //WHEN
//        var handler = new CartMediatorHandler(_userProfile, _dbContextMock, mediatorMock.Object);
//        var result = await handler.Handle(param, CancellationToken.None);

//        //THEN
//        Assert.Equal(_userProfile.ProfileId, result.Profileid);
//        Assert.Equal(itemid, result.Product[0].Id);
//    }
//}