//using Colorverse.ApisTests;
//using Colorverse.Application.Session;
//using Colorverse.Common;
//using Colorverse.Common.Helper;
//using Colorverse.Database;
//using Colorverse.Meta.DataTypes.Asset;
//using Colorverse.Meta.DataTypes.Resource;
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

//public class BuilderAssetMediatorHandlerTests : BaseTests
//{
//    private readonly IContextUserProfile _userProfile;
//    private readonly TestDbContextImpl _dbContextMock;
//    private readonly Mock<IMediator> _handlerMock;

//    public BuilderAssetMediatorHandlerTests()
//    {
//        _userProfile = GetTestUserProfile();
//        _dbContextMock = GetTestDbContext();
//        _handlerMock = new Mock<IMediator>();
//    }

//    [Fact()]
//    public async Task Handle_GetBuilderAssetParam()
//    {
//        // GIVEN
//        var param = new GetBuilderAssetParam
//        {
//            Assetid = UuidGenerater.NewUuid(SvcDomainType.MetaAsset)
//        };

//        var testImage = UuidGenerater.NewUuid(SvcDomainType.MetaAsset);

        
//        _dbContextMock.MockDocRepository.Setup(x => x.GetAsync<MongoReadOnlyDocBase>(It.IsAny<AssetDoc>(), It.IsAny<CancellationToken>()))
//            .Returns<MongoReadOnlyDocBase, CancellationToken>((doc, cancellationToken) => {
//                var assetDoc = ((AssetDoc)doc);
//                return Task.FromResult(new MongoDocUpdated(new BsonDocument()
//                {
//                    {"_id",  assetDoc.Id.Guid.ToBsonBinaryData()},
//                    {"creatorid",  _userProfile.ProfileId.Guid.ToBsonBinaryData()},
//                    {"ownerid",  _userProfile.ProfileId.Guid.ToBsonBinaryData()},
//                    {"userid",  _userProfile.UserId.Guid.ToBsonBinaryData()},
//                    {"worldid",  _userProfile.WorldId.Guid.ToBsonBinaryData()},
//                    {"creator_type",  1},
//                    {"owner_type",  1},
//                    {"status",  5},
//                    {"type",  2},
//                    {"txt", new BsonDocument(){
//                        { "title", new BsonDocument() {
//                            {"ko", "test" } } },
//                        { "desc", new BsonDocument() {
//                            { "ko", "test" } }
//                        }
//                    } },
//                    {"resource", new BsonDocument(){
//                        {"manifest", PublicUrlHelper.GetAssetManifestUri(assetDoc.Id, 1, 2) },
//                        {"thumbnail", PublicUrlHelper.GetResourceImageUri(testImage) },
//                        {"image", new BsonArray(){ PublicUrlHelper.GetResourceImageUri(testImage) } },
//                    } },
//                    {"option",  new BsonDocument(){
//                        {"version", 1},
//                        {"publish_version", 1},
//                        {"publish_review_status", 4},
//                        {"sale_status",  4},
//                        {"sale_review_status",  4},
//                        {"del_status",  1},
//                        {"category",  new BsonArray(){ {1},{10000 },{10001 } } },
//                        {"price",  new BsonArray(){
//                            new BsonDocument() {
//                                {"type" , 3},
//                                {"amount", 10000 }
//                            }
//                        } },
//                    } },

//                }));
//            });

        
//        // WHEN
//        var handler = new BuilderAssetMediatorHandler(_userProfile, _dbContextMock, _handlerMock.Object);
//        var result = await handler.Handle(param, CancellationToken.None);

//        // THEN
//        Assert.Equal(param.Assetid, result.Id);
//        Assert.Equal(_userProfile.ProfileId, result.Creatorid);
//        Assert.Equal(_userProfile.ProfileId, result.Ownerid);
//        Assert.Equal(_userProfile.UserId, result.Userid);
//        Assert.Equal(_userProfile.WorldId, result.Worldid);
//        Assert.Equal(5, result.Status);
//        Assert.NotEqual(1, result.Type);
//        Assert.Equal("test", result.Txt.Title.Ko);
//        Assert.Equal("test", result.Txt.Desc!.Ko);
//        Assert.Equal(1, result.Option.Version);
//        Assert.Equal(4, result.Option.SaleStatus);
//        Assert.Equal(1, result.Option.DelStatus);
//        Assert.Equal(3, result.Option.Category.Length);
//        if (result.Option.Price!.Length > 0)
//        {
//            Assert.Equal(3, result.Option.Price[0].Type);
//        }
//    }

//    [Fact()]
//    public async Task Handle_GetAssetListParam()
//    {
//        //GIVEN
//        var param = new GetAssetListParam()
//        {
//            Ownerid = _userProfile.ProfileId,
//            Page = 1,
//            Limit = 3,
//            PriceType = 1,
//        };

//        var dbContextMock = new Mock<IDbContext>();
//        dbContextMock.Setup(x => x.GetListAsync("asset", It.IsAny<FilterDefinition<Asset>>(), It.IsAny<FindOptions<Asset>>(), It.IsAny<CancellationToken>()))
//            .ReturnsAsync(() =>
//            {

//                var assetid = UuidGenerater.NewUuid(SvcDomainType.MetaAsset);
//                var asset = new Asset()
//                {
//                    Id = assetid,
//                    Creatorid = _userProfile.ProfileId,
//                    Ownerid = _userProfile.ProfileId,
//                    Userid = _userProfile.UserId,
//                    Worldid = _userProfile.WorldId,
//                    CreatorType = 1,
//                    Status = 5,
//                    Type = 2,
//                    Txt = new AssetTxt
//                    {
//                        Title = new Common.DataTypes.CvLocaleText { Ko = "title" },
//                        Desc = new Common.DataTypes.CvLocaleText { Ko = "desc" },
//                        Hashtag = new string[] { "hashtag" }
//                    },
//                    Option = new AssetOption()
//                    {
//                        Version = 1,
//                        PublishVersion = 1,
//                        PublishReviewStatus = 4,
//                        SaleStatus = 4,
//                        SaleReviewStatus = 4,
//                        DelStatus = 1,
//                        Category = new int[] { 1, 10000, 10001 },
//                        Price = new AssetPrice[] { new AssetPrice { Type = 1, Amount = 0 } }
//                    }
//                };
//                asset.Resource = new AssetResource(asset, 1, asset.Type);
//                ICollection<Asset> list = new List<Asset> { asset };
//                return list;
//            });

        
//        //WHEN
//        var handler = new BuilderAssetMediatorHandler(_userProfile, dbContextMock.Object, _handlerMock.Object);
//        var result = await handler.Handle(param, CancellationToken.None);

//        //THEN
//        Assert.Single(result.List);
//        Assert.Equal(_userProfile.ProfileId, result.List.First().Ownerid);
//        Assert.Equal(1, result.List.First().Option.Price!.First().Type);
//    }

//    [Fact()]
//    public async Task Handle_CreateAssetParam()
//    {
//        //GIVEN
//        var param = new CreateAssetParam
//        {
//            Assetid = UuidGenerater.NewUuid(SvcDomainType.MetaAsset),
//            Txt = new CreateAssetTxtParam
//            {
//                Title = new Common.DataTypes.CvLocaleText { Ko = "title" },
//                Desc = new Common.DataTypes.CvLocaleText { Ko = "desc" },
//                Hashtag = new string[] { "hashtag" }
//            },
//            Option = new CreateAssetOptionParam
//            {
//                Version = 1,
//                Category = new int[] { 1, 10000, 10001 },
//                Price = new CreateAssetPriceParam[] { new CreateAssetPriceParam { Type = 3, Amount = 1000 } }
//            },
//            Resource = new CreateAssetResourceParam
//            {
//                Thumbnail = UuidGenerater.NewUuid(SvcDomainType.AssetEtc),
//                Image = new string[] { UuidGenerater.NewUuid(SvcDomainType.AssetEtc) },
//            }
//        };



        
//        _dbContextMock.MongoDocSetupGetNotAll();
//        _dbContextMock.MongoDocSetupUpdateAll();
//        _dbContextMock.MockDocRepository.Setup(x => x.UpdateAsync<MongoDocBase>(It.IsAny<AssetRevisionDoc>(), It.IsAny<CancellationToken>()))
//            .Returns<MongoDocBase, CancellationToken>((doc, cancellationToken) =>
//            {
//                doc.TestForceUpdate();
//                return Task.FromResult(new MongoDocUpdated(doc.BsonDocument));
//            });

        
//        var manifest = new BsonDocument()
//        {
//            {"main" , new BsonDocument(){ {"type" , "model" } } },
//            {"subItemIds", new BsonArray(){ "1szXFgtTCUJTbsvV7NNlg", "1szcqLqDwdWICxGMlrika" } }
//        };
//        _handlerMock.Setup(x => x.Send(It.IsAny<GetResourceParam>(), It.IsAny<CancellationToken>()))
//            .ReturnsAsync(new ResourceDto(null!).TestData(param.Assetid, manifest));
//        _handlerMock.Setup(x => x.Send(It.IsAny<UseResourceParam>(), It.IsAny<CancellationToken>()))
//            .ReturnsAsync(new ResourceDto(null!).TestData(param.Assetid, manifest));


//        // WHEN
//        var handler = new BuilderAssetMediatorHandler(GetTestUserProfile(), _dbContextMock, _handlerMock.Object);
//        var result = await handler.Handle(param, CancellationToken.None);

//        // THEN
//        Assert.NotNull(result.Id);
//        Assert.Equal(param.Assetid, result.Id);
//        Assert.Equal(param.Txt.Title.Ko, result.Txt.Title.Ko);
//        Assert.Equal(param.Txt.Desc.Ko, result.Txt.Desc!.Ko);
//        Assert.Equal(param.Txt.Hashtag, result.Txt.Hashtag);
//        Assert.Equal(param.Option.Version, result.Option.Version);
//        Assert.Equal(param.Option.Category, result.Option.Category);
//        Assert.Equal(param.Option.Price[0].Type, result.Option.Price![0].Type);
//        Assert.Equal(PublicUrlHelper.GetResourceImageUri(param.Resource.Thumbnail), result.Resource.Thumbnail);
//        Assert.Equal(param.Resource.Image.Select(x => PublicUrlHelper.GetResourceImageUri(x)), result.Resource.Image!);
//    }

//    [Fact()]
//    public async Task Handle_UpdateAssetParam()
//    {
//        //GIVEN
//        var param = new UpdateAssetParam
//        {
//            Assetid = UuidGenerater.NewUuid(SvcDomainType.MetaAsset),
//            Txt = new UpdateAssetTxtParam
//            {
//                Desc = new Common.DataTypes.CvLocaleText { Ko = "desc" },
//                Hashtag = new string[] { "hashtag" }
//            },
//            Option = new UpdateAssetOptionParam
//            {
//                Version = 1,
//                Price = new UpdateAssetPriceParam[] { new UpdateAssetPriceParam { Type = 3, Amount = 1000 } }
//            },
//            Resource = new UpdateAssetResourceParam
//            {
//                Thumbnail = UuidGenerater.NewUuid(SvcDomainType.AssetEtc),
//                Image = new string[] { UuidGenerater.NewUuid(SvcDomainType.AssetEtc) },
//            }
//        };

        
//        _dbContextMock.MongoDocSetupGetAll();
//        _dbContextMock.MockDocRepository.Setup(x => x.GetAsync<MongoReadOnlyDocBase>(It.IsAny<AssetDoc>(), It.IsAny<CancellationToken>()))
//            .Returns<MongoReadOnlyDocBase, CancellationToken>((doc, cancellationToken) =>
//            {
//                return Task.FromResult(new MongoDocUpdated(new BsonDocument()
//                {
//                   {"_id",  param.Assetid.Guid.ToBsonBinaryData()},
//                   {"ownerid",  _userProfile.ProfileId.Guid.ToBsonBinaryData()},
//                   {"txt", new BsonDocument(){
//                        { "desc", new BsonDocument() {
//                            { "ko", "desc" } }
//                        },
//                        { "hashtag", new BsonArray(){
//                            param.Txt.Hashtag[0]
//                        }}
//                    } },
//                    {"option", new BsonDocument(){
//                        {"version", 1 },
//                        {"price", new BsonArray()
//                            {
//                                new BsonDocument()
//                                {
//                                    {"type", 3 },
//                                    {"amount", 1000 }
//                                }
//                            }
//                        }
//                    } }
//                }));
//            });

//        _dbContextMock.MongoDocSetupUpdateAll();

        
//        var manifest = new BsonDocument()
//        {
//            {"main" , new BsonDocument(){ {"type" , "Item" } } },
//            {"subItemIds", new BsonArray(){ "1szXFgtTCUJTbsvV7NNlg", "1szcqLqDwdWICxGMlrika" } }
//        };
//        _handlerMock.Setup(x => x.Send(It.IsAny<GetResourceParam>(), It.IsAny<CancellationToken>()))
//            .ReturnsAsync(new ResourceDto(null!).TestData(param.Assetid, manifest));
//        _handlerMock.Setup(x => x.Send(It.IsAny<UseResourceParam>(), It.IsAny<CancellationToken>()))
//            .ReturnsAsync(new ResourceDto(null!).TestData(param.Assetid, manifest));
//        // WHEN
//        var handler = new BuilderAssetMediatorHandler(GetTestUserProfile(), _dbContextMock, _handlerMock.Object);
//        var result = await handler.Handle(param, CancellationToken.None);

//        //THEN
//        Assert.Equal(param.Assetid, result.Id);
//        Assert.Equal(param.Txt.Desc.Ko, result.Txt.Desc!.Ko);
//        Assert.Equal(param.Txt.Hashtag.Length, result.Txt.Hashtag.Length);
//        Assert.Equal(param.Option.Version, result.Option.Version);
//        Assert.Equal(param.Option.Price[0].Type, result.Option.Price![0].Type);
//        Assert.Equal(param.Option.Price[0].Amount, result.Option.Price[0].Amount);
//    }

//    [Fact()]
//    public async Task Handle_PublishInspectionAssetParam()
//    {
//        //GIVEN
//        var param = new PublishInspectionAssetParam
//        {
//            Assetid = UuidGenerater.NewUuid(SvcDomainType.MetaAsset),
//            Version = 1
//        };

        

//        _dbContextMock.MockDocRepository.Setup(x => x.GetAsync<MongoReadOnlyDocBase>(It.IsAny<AssetDoc>(), It.IsAny<CancellationToken>()))
//            .Returns<MongoReadOnlyDocBase, CancellationToken>((doc, cancellationToken) =>
//            {
//                return Task.FromResult(new MongoDocUpdated(new BsonDocument()
//                {
//                   {"_id",  param.Assetid.Guid.ToBsonBinaryData()},
//                   {"ownerid",  _userProfile.ProfileId.Guid.ToBsonBinaryData()},
//                   {"option", new BsonDocument(){
//                        {"version", param.Version },
//                        {"publish_version", param.Version },
//                        {"category",  new BsonArray(){ {1},{10000 },{10001 } } }
//                    } }
//                }));
//            });

//        _dbContextMock.MongoDocSetupUpdateAll();

//        _dbContextMock.MockDocRepository.Setup(x => x.GetAsync<MongoReadOnlyDocBase>(It.IsAny<AssetRevisionDoc>(), It.IsAny<CancellationToken>()))
//            .Returns<MongoReadOnlyDocBase, CancellationToken>((doc, cancellationToken) =>
//            {
//                return Task.FromResult(new MongoDocUpdated(new BsonDocument()
//                {
//                   {"_id",  Guid.NewGuid().ToBsonBinaryData()},
//                   {"assetid",  param.Assetid.Guid.ToBsonBinaryData()},
//                   {"version", param.Version },
//                   {"status", 1 }
//                }));
//            });
//        _dbContextMock.MockDocRepository.Setup(x => x.UpdateAsync<MongoDocBase>(It.IsAny<AssetRevisionDoc>(), It.IsAny<CancellationToken>()))
//            .Returns<MongoDocBase, CancellationToken>((doc, cancellationToken) =>
//            {
//                doc.TestForceUpdate();
//                return Task.FromResult(new MongoDocUpdated(doc.BsonDocument));
//            });


        
//        // WHEN
//        var handler = new BuilderAssetMediatorHandler(GetTestUserProfile(), _dbContextMock, _handlerMock.Object);
//        var result = await handler.Handle(param, CancellationToken.None);

//        //THEN
//        Assert.Equal(param.Assetid, result.Id);
//        Assert.Equal(param.Version, result.Option.PublishVersion);
//    }

//    [Fact()]
//    public async Task Handle_DeleteAssetParam()
//    {
//        //GIVEN
//        var param = new DeleteAssetParam
//        {
//            Assetid = UuidGenerater.NewUuid(SvcDomainType.MetaAsset)
//        };
        

//        _dbContextMock.MockDocRepository.Setup(x => x.GetAsync<MongoReadOnlyDocBase>(It.IsAny<AssetDoc>(), It.IsAny<CancellationToken>()))
//           .Returns<MongoReadOnlyDocBase, CancellationToken>((doc, cancellationToken) =>
//           {
//               return Task.FromResult(new MongoDocUpdated(new BsonDocument()
//               {
//                   {"_id",  param.Assetid.Guid.ToBsonBinaryData()},
//                   {"ownerid",  _userProfile.ProfileId.Guid.ToBsonBinaryData()},
//                   {"option", new BsonDocument(){
//                        {"category",  new BsonArray(){ {1},{10000 },{10001 } } }
//                    } }
//               }));
//           });

//        _dbContextMock.MockDocRepository.Setup(x => x.GetAsync<MongoReadOnlyDocBase>(It.IsAny<MarketProductDoc>(), It.IsAny<CancellationToken>()))
//           .Returns<MongoReadOnlyDocBase, CancellationToken>((doc, cancellationToken) =>
//           {
//               return Task.FromResult(new MongoDocUpdated(new BsonDocument()
//               {
//                   {"_id",  param.Assetid.Guid.ToBsonBinaryData()},
//                   {"ownerid",  _userProfile.ProfileId.Guid.ToBsonBinaryData()},
//                   {"option", new BsonDocument(){
//                        {"category",  new BsonArray(){ {1},{10000 },{10001 } } }
//                    } }
//               }));
//           });

//        _dbContextMock.MongoDocSetupUpdateAll();


        
//        // WHEN
//        var handler = new BuilderAssetMediatorHandler(GetTestUserProfile(), _dbContextMock, _handlerMock.Object);
//        var result = await handler.Handle(param, CancellationToken.None);
//        //THEN
//        Assert.Equal(param.Assetid, result.Id);
//    }

//    [Fact()]
//    public async Task Handle_DeleteCancelAssetParam()
//    {
//        //GIVEN
//        var param = new DeleteCancelAssetParam
//        {
//            Assetid = UuidGenerater.NewUuid(SvcDomainType.MetaAsset)
//        };
        

//        _dbContextMock.MockDocRepository.Setup(x => x.GetAsync<MongoReadOnlyDocBase>(It.IsAny<AssetDoc>(), It.IsAny<CancellationToken>()))
//           .Returns<MongoReadOnlyDocBase, CancellationToken>((doc, cancellationToken) =>
//           {
//               return Task.FromResult(new MongoDocUpdated(new BsonDocument()
//               {
//                   {"_id",  param.Assetid.Guid.ToBsonBinaryData()},
//                   {"ownerid",  _userProfile.ProfileId.Guid.ToBsonBinaryData()},
//                   {"option", new BsonDocument(){
//                        {"category",  new BsonArray(){ {1},{10000 },{10001 } } }
//                    } }
//               }));
//           });

//        _dbContextMock.MockDocRepository.Setup(x => x.GetAsync<MongoReadOnlyDocBase>(It.IsAny<MarketProductDoc>(), It.IsAny<CancellationToken>()))
//           .Returns<MongoReadOnlyDocBase, CancellationToken>((doc, cancellationToken) =>
//           {
//               return Task.FromResult(new MongoDocUpdated(new BsonDocument()
//               {
//                   {"_id",  param.Assetid.Guid.ToBsonBinaryData()},
//                   {"ownerid",  _userProfile.ProfileId.Guid.ToBsonBinaryData()},
//                   {"option", new BsonDocument(){
//                        {"category",  new BsonArray(){ {1},{10000 },{10001 } } }
//                    } }
//               }));
//           });
//        _dbContextMock.MockDocRepository.Setup(x => x.GetAsync<MongoReadOnlyDocBase>(It.IsAny<AssetDeleteRequestDoc>(), It.IsAny<CancellationToken>()))
//          .Returns<MongoReadOnlyDocBase, CancellationToken>((doc, cancellationToken) =>
//          {
//              return Task.FromResult(new MongoDocUpdated(null));
//          });


//        _dbContextMock.MongoDocSetupUpdateAll();


        
//        // WHEN
//        var handler = new BuilderAssetMediatorHandler(GetTestUserProfile(), _dbContextMock, _handlerMock.Object);
//        var result = await handler.Handle(param, CancellationToken.None);
//        //THEN
//        Assert.Equal(param.Assetid, result.Id);
//    }

//    [Fact()]
//    public async Task Handle_SaleInspectionAssetParam()
//    {
//        //GIVEN
//        var param = new SaleInspectionAssetParam
//        {
//            Assetid = UuidGenerater.NewUuid(SvcDomainType.MetaAsset),
//            Option = new SaleInspectionAssetOptionParam { Price = new SaleInspectionAssetPriceParam[] { new SaleInspectionAssetPriceParam { Type = 3, Amount = 10000 } }, IsSale = true }
//        };
        

//        _dbContextMock.MockDocRepository.Setup(x => x.GetAsync<MongoReadOnlyDocBase>(It.IsAny<AssetDoc>(), It.IsAny<CancellationToken>()))
//           .Returns<MongoReadOnlyDocBase, CancellationToken>((doc, cancellationToken) =>
//           {
//               return Task.FromResult(new MongoDocUpdated(new BsonDocument()
//               {
//                   {"_id",  param.Assetid.Guid.ToBsonBinaryData()},
//                   {"ownerid",  _userProfile.ProfileId.Guid.ToBsonBinaryData()},
//                   {"option", new BsonDocument(){
//                        {"category",  new BsonArray(){ {1},{10000 },{10001 } } },
//                        {"publish_review_status", 4 }
//                    } }
//               }));
//           });

//        _dbContextMock.MockDocRepository.Setup(x => x.GetAsync<MongoReadOnlyDocBase>(It.IsAny<MarketProductDoc>(), It.IsAny<CancellationToken>()))
//           .Returns<MongoReadOnlyDocBase, CancellationToken>((doc, cancellationToken) =>
//           {
//               return Task.FromResult(new MongoDocUpdated(new BsonDocument()
//               {
//                   {"_id",  param.Assetid.Guid.ToBsonBinaryData()},
//                   {"ownerid",  _userProfile.ProfileId.Guid.ToBsonBinaryData()},
//                   {"option", new BsonDocument(){
//                        {"category",  new BsonArray(){ {1},{10000 },{10001 } } }
//                    } }
//               }));
//           });

//        _dbContextMock.MongoDocSetupUpdateAll();

        
//        // WHEN
//        var handler = new BuilderAssetMediatorHandler(GetTestUserProfile(), _dbContextMock, _handlerMock.Object);
//        var result = await handler.Handle(param, CancellationToken.None);
//        //THEN
//        Assert.Equal(param.Assetid, result.Id);
//    }

//    [Fact()]
//    public async Task Handle_SaleStopAssetParam()
//    {
//        //GIVEN
//        var param = new SaleStopAssetParam
//        {
//            Assetid = UuidGenerater.NewUuid(SvcDomainType.MetaAsset)
//        };
        

//        _dbContextMock.MockDocRepository.Setup(x => x.GetAsync<MongoReadOnlyDocBase>(It.IsAny<AssetDoc>(), It.IsAny<CancellationToken>()))
//           .Returns<MongoReadOnlyDocBase, CancellationToken>((doc, cancellationToken) =>
//           {
//               return Task.FromResult(new MongoDocUpdated(new BsonDocument()
//               {
//                   {"_id",  param.Assetid.Guid.ToBsonBinaryData()},
//                   {"ownerid",  _userProfile.ProfileId.Guid.ToBsonBinaryData()},
//                   {"option", new BsonDocument(){
//                        {"category",  new BsonArray(){ {1},{10000 },{10001 } } },
//                        {"sale_status",  4 }
//                    } }
//               }));
//           });

//        _dbContextMock.MockDocRepository.Setup(x => x.GetAsync<MongoReadOnlyDocBase>(It.IsAny<MarketProductDoc>(), It.IsAny<CancellationToken>()))
//           .Returns<MongoReadOnlyDocBase, CancellationToken>((doc, cancellationToken) =>
//           {
//               return Task.FromResult(new MongoDocUpdated(new BsonDocument()
//               {
//                   {"_id",  param.Assetid.Guid.ToBsonBinaryData()},
//                   {"ownerid",  _userProfile.ProfileId.Guid.ToBsonBinaryData()},
//                   {"option", new BsonDocument(){
//                        {"category",  new BsonArray(){ {1},{10000 },{10001 } } }
//                    } }
//               }));
//           });

//        _dbContextMock.MongoDocSetupUpdateAll();

        
//        // WHEN
//        var handler = new BuilderAssetMediatorHandler(GetTestUserProfile(), _dbContextMock, _handlerMock.Object);
//        var result = await handler.Handle(param, CancellationToken.None);
//        //THEN
//        Assert.Equal(param.Assetid, result.Id);
//    }
//}