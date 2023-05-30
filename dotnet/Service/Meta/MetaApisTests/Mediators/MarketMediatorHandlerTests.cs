//using Colorverse.ApisTests;
//using Colorverse.Application.Session;
//using Colorverse.Common;
//using Colorverse.Common.Helper;
//using Colorverse.Database;
//using Colorverse.Meta.DataTypes.Market;
//using Colorverse.Meta.Documents;
//using Colorverse.Meta.Domains;
//using CvFramework.MongoDB.Doc;
//using CvFramework.MongoDB.Documents;
//using CvFramework.MongoDB.Extensions;
//using MediatR;
//using MongoDB.Bson;
//using MongoDB.Driver;
//using Moq;
//using Xunit;

//namespace Colorverse.Meta.Mediators.Tests;

//public class MarketMediatorHandlerTests : BaseTests
//{
//    private readonly IContextUserProfile _userProfile;
//    private readonly TestDbContextImpl _dbContextMock;


//    public MarketMediatorHandlerTests()
//    {
//        _userProfile = GetTestUserProfile();
//        _dbContextMock = GetTestDbContext();
//    }


//    [Fact()]
//    public async Task Handle_GetMarketParam()
//    {
//        //GIVEN
//        var param = new GetMarketParam
//        {
//            Productid = UuidGenerater.NewUuid(SvcDomainType.MetaItem)
//        };
        

//        _dbContextMock.MockDocRepository.Setup(x => x.GetAsync<MongoReadOnlyDocBase>(It.IsAny<MarketProductDoc>(), It.IsAny<CancellationToken>()))
//           .Returns<MongoReadOnlyDocBase, CancellationToken>((doc, cancellationToken) =>
//           {
//               return Task.FromResult(new MongoDocUpdated(new BsonDocument()
//               {
//                   {"_id",  param.Productid.Guid.ToBsonBinaryData()},
//                   {"type",  1},
//                   {"ownerid",  _userProfile.ProfileId.Guid.ToBsonBinaryData()},
//                   {"userid",  _userProfile.UserId.Guid.ToBsonBinaryData()},
//                   {"worldid",  _userProfile.WorldId.Guid.ToBsonBinaryData()},
//                   {"owner_type",  1},
//                   {"txt", new BsonDocument(){
//                        {"title",  new BsonDocument{ { "ko", "title"} } },
//                        {"desc",  new BsonDocument{ { "ko", "desc"} } },
//                        {"hashtag",  new BsonArray(){ {"hashtag"} } }
//                   } },
//                   {"resource", new BsonDocument(){
//                       {"manifest",  PublicUrlHelper.GetItemManifestUri(param.Productid, 1) },
//                        {"thumbnail",  PublicUrlHelper.GetResourceImageUri("thumbnailbase62") },
//                        {"image",  new BsonArray(){ PublicUrlHelper.GetResourceImageUri("imagebase62") } }
//                    } },
//                   {"option", new BsonDocument(){
//                        {"publish_version",  1 },
//                        {"del_status",  1 },
//                        {"asset_type",  1 },
//                        {"category",  new BsonArray(){ {1},{10000 },{10001 } } },
//                        {"price",  new BsonArray(){ new BsonDocument { {"type", 3}, {"amount", 10000 } } } },
//                        {"items",  new BsonArray(){ new BsonDocument { {"type", 1}, {"id", param.Productid.Guid.ToBsonBinaryData() }, { "quantity", 1 } } } },
//                    } }
//               }));
//           });

//        var mediatorMock = new Mock<IMediator>();

//        //WHEN
//        var handler = new MarketMediatorHandler(_userProfile, _dbContextMock, mediatorMock.Object);
//        var result = await handler.Handle(param, CancellationToken.None);
//        //THEN
//        Assert.Equal(param.Productid, result.Id);
//    }

//    [Fact()]
//    public async Task Handle_GetMarketListParam()
//    {
//        //GIVEN
//        var param = new GetMarketListParam
//        {
//            Worldid = _userProfile.WorldId,
//            Page = 1,
//            Limit = 50,
//            Category = new int[] { 1, 10000, 10001 }
//        };

//        var dbContextMock = new Mock<IDbContext>();
//        dbContextMock.Setup(x => x.GetListAsync("market_product", It.IsAny<FilterDefinition<Market>>(), It.IsAny<FindOptions<Market>>(), It.IsAny<CancellationToken>()))
//            .ReturnsAsync(() =>
//            {

//                var itemid = UuidGenerater.NewUuid(SvcDomainType.MetaItem);
//                var market = new Market()
//                {
//                    Id = itemid,
//                    Ownerid = _userProfile.ProfileId,
//                    Userid = _userProfile.UserId,
//                    Worldid = _userProfile.WorldId,
//                    Type = 1,
//                    OwnerType = 1,
//                    Status = 5,
//                    Txt = new MarketTxt
//                    {
//                        Title = new Common.DataTypes.CvLocaleText { Ko = "title" },
//                        Desc = new Common.DataTypes.CvLocaleText { Ko = "desc" },
//                        Hashtag = new string[] { "hashtag" }
//                    },
//                    Option = new MarketOption()
//                    {
//                        PublishVersion = 1,
//                        DelStatus = 1,
//                        Category = new int[] { 1, 10000, 10001 },
//                        Price = new MarketPrice[] { new MarketPrice { Type = 1, Amount = 0 } },
//                        Items = new MarketItems[] { new MarketItems { Type = 1, Id = itemid, Quantity = 1 } }
//                    }
//                };
//                market.Resource = new MarketResource(market);
//                ICollection<Market> list = new List<Market> { market };
//                return list;
//            });
        
//        var mediatorMock = new Mock<IMediator>();

//        //WHEN
//        var handler = new MarketMediatorHandler(_userProfile, dbContextMock.Object, mediatorMock.Object);
//        var result = await handler.Handle(param, CancellationToken.None);

//        //THEN
//        Assert.Single(result.List);
//        Assert.Equal(_userProfile.ProfileId, result.List.First().Ownerid);
//        Assert.Equal(1, result.List.First().Option.Price!.First().Type);
//    }

//    [Fact()]
//    public async Task Handle_PurchaseMarketListParam()
//    {
//        var itemid = UuidGenerater.NewUuid(SvcDomainType.MetaItem);
//        //GIVEN
//        var param = new PurchaseMarketListParam
//        {
//            Products = new PurchaseMarketListProductsParam[]
//            {
//               new PurchaseMarketListProductsParam
//               {
//                   Productid = itemid,
//                   PriceType = 3,
//                   PriceAmount = 10000,
//               }
//            }
//        };

        
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
//        var handler = new MarketMediatorHandler(_userProfile, _dbContextMock, mediatorMock.Object);
//        var result = await handler.Handle(param, CancellationToken.None);

       
//        Assert.NotNull(result.Id);
//        Assert.Equal(itemid, result.Product[0].Id);
//    }
//}