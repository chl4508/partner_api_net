//using Colorverse.ApisTests;
//using Colorverse.Application.Session;
//using Colorverse.Common;
//using Colorverse.Common.Helper;
//using Colorverse.Database;
//using Colorverse.Meta.DataTypes.Item;
//using Colorverse.Meta.DataTypes.Resource;
//using Colorverse.Meta.Documents;
//using Colorverse.Meta.Domains;
//using Colorverse.UserLibrary;
//using CvFramework.MongoDB.Doc;
//using CvFramework.MongoDB.Documents;
//using CvFramework.MongoDB.Extensions;
//using MediatR;
//using MongoDB.Bson;
//using MongoDB.Driver;
//using Moq;
//using Xunit;

//namespace Colorverse.Meta.Mediators.Tests;

///// <summary>
///// 
///// </summary>
//public class BuilderItemMediatorHandlerTests : BaseTests
//{
//    private readonly IContextUserProfile _userProfile;
//    private readonly TestDbContextImpl _dbContextMock;
//    private readonly Mock<IMediator> _handlerMock;
//    private readonly Mock<IUserLibrary> _userLibraryMock;


//    public BuilderItemMediatorHandlerTests()
//    {
//        _userProfile = GetTestUserProfile();
//        _dbContextMock = GetTestDbContext();
//        _handlerMock = new Mock<IMediator>();
//        _userLibraryMock = new Mock<IUserLibrary>();
//    }




//    [Fact()]
//    public async Task Handle_GetBuilderItemParam()
//    {
//        // GIVEN
//        var param = new GetBuilderItemParam
//        {
//            Itemid = UuidGenerater.NewUuid(SvcDomainType.MetaItem)
//        };

//        _dbContextMock.MockDocRepository.Setup(x => x.GetAsync<MongoReadOnlyDocBase>(It.IsAny<ItemDoc>(), It.IsAny<CancellationToken>()))
//            .Returns<MongoReadOnlyDocBase, CancellationToken>((doc, cancellationToken) => {
//                ((ItemDoc)doc).TestData(_userProfile.ProfileId, _userProfile.WorldId, _userProfile.UserId);
//                return Task.FromResult(new MongoDocUpdated(doc.BsonDocument));
//            });

//        // WHEN
//        var handler = new BuilderItemMediatorHandler(_userProfile, _dbContextMock, _handlerMock.Object, _userLibraryMock.Object);
//        var result = await handler.Handle(param, CancellationToken.None);

//        // THEN
//        Assert.Equal(param.Itemid, result.Id);
//        Assert.Equal(1, result.CreatorType);
//        Assert.Equal(_userProfile.ProfileId, result.Creatorid);
//        Assert.Equal(1, result.OwnerType);
//        Assert.Equal(_userProfile.ProfileId, result.Ownerid);
//        Assert.Equal(_userProfile.UserId, result.Userid);
//        Assert.Equal(_userProfile.WorldId, result.Worldid);
//        Assert.Equal(5, result.Status);
//        Assert.Equal(1, result.AssetType);
//        Assert.Equal($"Title_Ko_{result.Id}", result.Txt.Title.Ko);
//        Assert.Equal($"Desc_Ko_{result.Id}", result.Txt.Desc!.Ko);
//    }

//    [Fact()]
//    public async Task Handle_GetItemListParam()
//    {
//        //GIVEN
//        var param = new GetItemListParam()
//        {
//            Ownerid = _userProfile.ProfileId,
//            Page = 1,
//            Limit = 3,
//            PriceType = 1,
//        };

//        var dbContextMock = new Mock<IDbContext>();
//        dbContextMock.Setup(x => x.GetListAsync("item", It.IsAny<FilterDefinition<Item>>(), It.IsAny<FindOptions<Item>>(), It.IsAny<CancellationToken>()))
//            .ReturnsAsync(() =>
//            {

//                var itemid = UuidGenerater.NewUuid(SvcDomainType.MetaItem);
//                var item = new Item()
//                {
//                    Id = itemid,
//                    Creatorid = _userProfile.ProfileId,
//                    Ownerid = _userProfile.ProfileId,
//                    Userid = _userProfile.UserId,
//                    Worldid = _userProfile.WorldId,
//                    CreatorType = 1,
//                    Status = 5,
//                    AssetType = 1,
//                    Txt = new ItemTxt
//                    {
//                        Title = new Common.DataTypes.CvLocaleText { Ko = "title" },
//                        Desc = new Common.DataTypes.CvLocaleText { Ko = "desc" },
//                        Hashtag = new string[] { "hashtag" }
//                    },
//                    Option = new ItemOption()
//                    {
//                        Version = 1,
//                        PublishVersion = 1,
//                        PublishReviewStatus = 4,
//                        SaleStatus = 4,
//                        SaleReviewStatus = 4,
//                        DelStatus = 1,
//                        Category = new int[] { 1, 10000, 10001 },
//                        Price = new ItemPrice[] { new ItemPrice { Type = 1, Amount = 0 } }
//                    }
//                };
//                item.Resource = new ItemResource(item, 1);
//                ICollection<Item> list = new List<Item> { item };
//                return list;
//            });

//        //WHEN
//        var handler = new BuilderItemMediatorHandler(_userProfile, dbContextMock.Object, _handlerMock.Object, _userLibraryMock.Object);
//        var result = await handler.Handle(param, CancellationToken.None);

//        //THEN
//        Assert.Single(result.List);
//        Assert.Equal(_userProfile.ProfileId, result.List.First().Ownerid);
//        Assert.Equal(1, result.List.First().Option.Price!.First().Type);
//    }


//    [Fact()]
//    public async Task Handle_CreateItemParam()
//    {
//        //GIVEN
//        var param = new CreateItemParam
//        {
//            Itemid = UuidGenerater.NewUuid(SvcDomainType.MetaItem),
//            Txt = new CreateItemTxtParam
//            {
//                Title = new Common.DataTypes.CvLocaleText { Ko = "title" },
//                Desc = new Common.DataTypes.CvLocaleText { Ko = "desc" },
//                Hashtag = new string[] { "hashtag" }
//            },
//            Option = new CreateItemOptionParam
//            {
//                Version = 1,
//                Category = new int[] { 1, 10000, 10001 },
//                Price = new CreateItemPriceParam[] { new CreateItemPriceParam { Type = 3, Amount = 1000 } }
//            },
//            Resource = new CreateItemResourceParam
//            {
//                Thumbnail = UuidGenerater.NewUuid(SvcDomainType.AssetEtc),
//                Image = new string[] { UuidGenerater.NewUuid(SvcDomainType.AssetEtc) },
//            }
//        };



        
//        _dbContextMock.MongoDocSetupGetNotAll();
//        _dbContextMock.MongoDocSetupUpdateAll();
//        _dbContextMock.MockDocRepository.Setup(x => x.UpdateAsync<MongoDocBase>(It.IsAny<ItemRevisionDoc>(), It.IsAny<CancellationToken>()))
//            .Returns<MongoDocBase, CancellationToken>((doc, cancellationToken) =>
//            {
//                doc.TestForceUpdate();
//                return Task.FromResult(new MongoDocUpdated(doc.BsonDocument));
//            });

        
//        var manifest = new BsonDocument()
//        {
//            {"main" , new BsonDocument(){ {"type" , "Item" } } },
//            {"subItemIds", new BsonArray(){ "1szXFgtTCUJTbsvV7NNlg", "1szcqLqDwdWICxGMlrika" } }
//        };
//        _handlerMock.Setup(x => x.Send(It.IsAny<GetResourceParam>(), It.IsAny<CancellationToken>()))
//            .ReturnsAsync(new ResourceDto(null!).TestData(param.Itemid, manifest));
//        _handlerMock.Setup(x => x.Send(It.IsAny<UseResourceParam>(), It.IsAny<CancellationToken>()))
//            .ReturnsAsync(new ResourceDto(null!).TestData(param.Itemid, manifest));

        
        
//        // WHEN
//        var handler = new BuilderItemMediatorHandler(_userProfile, _dbContextMock, _handlerMock.Object, _userLibraryMock.Object);
//        var result = await handler.Handle(param, CancellationToken.None);

//        // THEN
//        Assert.NotNull(result.Id);
//        Assert.Equal(param.Itemid, result.Id);
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
//    public async Task Handle_UpdateItemParam()
//    {
//        //GIVEN
//        var param = new UpdateItemParam
//        {
//            Itemid = UuidGenerater.NewUuid(SvcDomainType.MetaItem),
//            Txt = new UpdateItemTxtParam
//            {
//                Desc = new Common.DataTypes.CvLocaleText { Ko = "desc" },
//                Hashtag = new string[] { "hashtag" }
//            },
//            Option = new UpdateItemOptionParam
//            {
//                Version = 1,
//                Price = new UpdateItemPriceParam[] { new UpdateItemPriceParam { Type = 3, Amount = 1000 } }
//            },
//            Resource = new UpdateItemResourceParam
//            {
//                Thumbnail = UuidGenerater.NewUuid(SvcDomainType.AssetEtc),
//                Image = new string[] { UuidGenerater.NewUuid(SvcDomainType.AssetEtc) },
//            }
//        };

        
//        _dbContextMock.MongoDocSetupGetAll();
//        _dbContextMock.MockDocRepository.Setup(x => x.GetAsync<MongoReadOnlyDocBase>(It.IsAny<ItemDoc>(), It.IsAny<CancellationToken>()))
//            .Returns<MongoReadOnlyDocBase, CancellationToken>((doc, cancellationToken) =>
//            {
//                return Task.FromResult(new MongoDocUpdated(new BsonDocument()
//                {
//                   {"_id",  param.Itemid.Guid.ToBsonBinaryData()},
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
//            .ReturnsAsync(new ResourceDto(null!).TestData(param.Itemid, manifest));
//        _handlerMock.Setup(x => x.Send(It.IsAny<UseResourceParam>(), It.IsAny<CancellationToken>()))
//            .ReturnsAsync(new ResourceDto(null!).TestData(param.Itemid, manifest));

        

//        //WHEN
//        var handler = new BuilderItemMediatorHandler(_userProfile, _dbContextMock,_handlerMock.Object, _userLibraryMock.Object);
//        var result = await handler.Handle(param, CancellationToken.None);

//        //THEN
//        Assert.Equal(param.Itemid, result.Id);
//        Assert.Equal(param.Txt.Desc.Ko, result.Txt.Desc!.Ko);
//        Assert.Equal(param.Txt.Hashtag.Length, result.Txt.Hashtag.Length);
//        Assert.Equal(param.Option.Version, result.Option.Version);
//        Assert.Equal(param.Option.Price[0].Type, result.Option.Price![0].Type);
//        Assert.Equal(param.Option.Price[0].Amount, result.Option.Price[0].Amount);
//    }

//    [Fact()]
//    public async Task Handle_PublishInspectionItemParam()
//    {
//        //GIVEN
//        var param = new PublishInspectionItemParam
//        {
//            Itemid = UuidGenerater.NewUuid(SvcDomainType.MetaItem),
//            Version = 1
//        };

        

//        _dbContextMock.MockDocRepository.Setup(x => x.GetAsync<MongoReadOnlyDocBase>(It.IsAny<ItemDoc>(), It.IsAny<CancellationToken>()))
//            .Returns<MongoReadOnlyDocBase, CancellationToken>((doc, cancellationToken) =>
//            {
//                return Task.FromResult(new MongoDocUpdated(new BsonDocument()
//                {
//                   {"_id",  param.Itemid.Guid.ToBsonBinaryData()},
//                   {"ownerid",  _userProfile.ProfileId.Guid.ToBsonBinaryData()},
//                   {"option", new BsonDocument(){
//                        {"version", param.Version },
//                        {"publish_version", param.Version },
//                        {"category",  new BsonArray(){ {1},{10000 },{10001 } } }
//                    } }
//                }));
//            });
//        _dbContextMock.MockDocRepository.Setup(x => x.GetAsync<MongoReadOnlyDocBase>(It.IsAny<UserItemDoc>(), It.IsAny<CancellationToken>()))
//            .Returns<MongoReadOnlyDocBase, CancellationToken>((doc, cancellationToken) =>
//            {
//                return Task.FromResult(new MongoDocUpdated(new BsonDocument()
//                {
//                   {"_id",  UuidGenerater.NewUuid(SvcDomainType.MetaUserItem).Guid.ToBsonBinaryData()},
//                   {"userid",  _userProfile.ProfileId.Guid.ToBsonBinaryData()},
//                   {"profileid",  _userProfile.ProfileId.Guid.ToBsonBinaryData()},
//                   {"worldid",  _userProfile.WorldId.Guid.ToBsonBinaryData()},
//                   {"itemid", param.Itemid.Guid.ToBsonBinaryData()}
//                }));
//            });
//        _dbContextMock.MongoDocSetupUpdateAll();

//        _dbContextMock.MockDocRepository.Setup(x => x.GetAsync<MongoReadOnlyDocBase>(It.IsAny<ItemRevisionDoc>(), It.IsAny<CancellationToken>()))
//            .Returns<MongoReadOnlyDocBase, CancellationToken>((doc, cancellationToken) =>
//            {
//                return Task.FromResult(new MongoDocUpdated(new BsonDocument()
//                {
//                   {"_id",  Guid.NewGuid().ToBsonBinaryData()},
//                   {"itemid",  param.Itemid.Guid.ToBsonBinaryData()},
//                   {"version", param.Version },
//                   {"status", 1 }
//                }));
//            });
//        _dbContextMock.MockDocRepository.Setup(x => x.UpdateAsync<MongoDocBase>(It.IsAny<ItemRevisionDoc>(), It.IsAny<CancellationToken>()))
//            .Returns<MongoDocBase, CancellationToken>((doc, cancellationToken) =>
//            {
//                doc.TestForceUpdate();
//                return Task.FromResult(new MongoDocUpdated(doc.BsonDocument));
//            });



        
//        var manifest = new BsonDocument()
//        {
//            {"main" , new BsonDocument(){ {"type" , "Item" } } },
//            {"subItemIds", new BsonArray(){ param.Itemid.Base62 } }
//        };
//        _handlerMock.Setup(x => x.Send(It.IsAny<GetResourceParam>(), It.IsAny<CancellationToken>()))
//            .ReturnsAsync(new ResourceDto(null!).TestData(param.Itemid, manifest));

        
        
//        //WHEN
//        var handler = new BuilderItemMediatorHandler(_userProfile, _dbContextMock, _handlerMock.Object, _userLibraryMock.Object);
//        var result = await handler.Handle(param, CancellationToken.None);

//        //THEN
//        Assert.Equal(param.Itemid, result.Id);
//        Assert.Equal(param.Version, result.Option.PublishVersion);
//    }

//    [Fact()]
//    public async Task Handle_DeleteItemParam()
//    {
//        //GIVEN
//        var param = new DeleteItemParam
//        {
//            Itemid = UuidGenerater.NewUuid(SvcDomainType.MetaItem)
//        };
        

//        _dbContextMock.MockDocRepository.Setup(x => x.GetAsync<MongoReadOnlyDocBase>(It.IsAny<ItemDoc>(), It.IsAny<CancellationToken>()))
//           .Returns<MongoReadOnlyDocBase, CancellationToken>((doc, cancellationToken) =>
//           {
//               return Task.FromResult(new MongoDocUpdated(new BsonDocument()
//               {
//                   {"_id",  param.Itemid.Guid.ToBsonBinaryData()},
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
//                   {"_id",  param.Itemid.Guid.ToBsonBinaryData()},
//                   {"ownerid",  _userProfile.ProfileId.Guid.ToBsonBinaryData()},
//                   {"option", new BsonDocument(){
//                        {"category",  new BsonArray(){ {1},{10000 },{10001 } } }
//                    } }
//               }));
//           });

//        _dbContextMock.MongoDocSetupUpdateAll();

        
        

//        //WHEN
//        var handler = new BuilderItemMediatorHandler(_userProfile, _dbContextMock, _handlerMock.Object, _userLibraryMock.Object);
//        var result = await handler.Handle(param, CancellationToken.None);
//        //THEN
//        Assert.Equal(param.Itemid, result.Id);
//    }

//    [Fact()]
//    public async Task Handle_DeleteCancelItemParam()
//    {
//        //GIVEN
//        var param = new DeleteCancelItemParam
//        {
//            Itemid = UuidGenerater.NewUuid(SvcDomainType.MetaItem)
//        };
        

//        _dbContextMock.MockDocRepository.Setup(x => x.GetAsync<MongoReadOnlyDocBase>(It.IsAny<ItemDoc>(), It.IsAny<CancellationToken>()))
//           .Returns<MongoReadOnlyDocBase, CancellationToken>((doc, cancellationToken) =>
//           {
//               return Task.FromResult(new MongoDocUpdated(new BsonDocument()
//               {
//                   {"_id",  param.Itemid.Guid.ToBsonBinaryData()},
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
//                   {"_id",  param.Itemid.Guid.ToBsonBinaryData()},
//                   {"ownerid",  _userProfile.ProfileId.Guid.ToBsonBinaryData()},
//                   {"option", new BsonDocument(){
//                        {"category",  new BsonArray(){ {1},{10000 },{10001 } } }
//                    } }
//               }));
//           });
//        _dbContextMock.MockDocRepository.Setup(x => x.GetAsync<MongoReadOnlyDocBase>(It.IsAny<ItemDeleteRequestDoc>(), It.IsAny<CancellationToken>()))
//          .Returns<MongoReadOnlyDocBase, CancellationToken>((doc, cancellationToken) =>
//          {
//              return Task.FromResult(new MongoDocUpdated(null));
//          });


//        _dbContextMock.MongoDocSetupUpdateAll();

        
        

//        //WHEN
//        var handler = new BuilderItemMediatorHandler(_userProfile, _dbContextMock, _handlerMock.Object, _userLibraryMock.Object);
//        var result = await handler.Handle(param, CancellationToken.None);
//        //THEN
//        Assert.Equal(param.Itemid, result.Id);
//    }

//    [Fact()]
//    public async Task Handle_SaleInspectionItemParam()
//    {
//        //GIVEN
//        var param = new SaleInspectionItemParam
//        {
//            Itemid = UuidGenerater.NewUuid(SvcDomainType.MetaItem),
//            Option = new SaleInspectionItemOptionParam { Price = new SaleInspectionItemPriceParam[] { new SaleInspectionItemPriceParam { Type = 3, Amount = 10000 } } }
//        };
        

//        _dbContextMock.MockDocRepository.Setup(x => x.GetAsync<MongoReadOnlyDocBase>(It.IsAny<ItemDoc>(), It.IsAny<CancellationToken>()))
//           .Returns<MongoReadOnlyDocBase, CancellationToken>((doc, cancellationToken) =>
//           {
//               return Task.FromResult(new MongoDocUpdated(new BsonDocument()
//               {
//                   {"_id",  param.Itemid.Guid.ToBsonBinaryData()},
//                   {"ownerid",  _userProfile.ProfileId.Guid.ToBsonBinaryData()},
//                   {"option", new BsonDocument(){
//                        {"category",  new BsonArray(){ {1},{10000 },{10001 } } },
//                        {"publish_review_status",  4}
//                    } }
//               }));
//           });

//        _dbContextMock.MockDocRepository.Setup(x => x.GetAsync<MongoReadOnlyDocBase>(It.IsAny<MarketProductDoc>(), It.IsAny<CancellationToken>()))
//           .Returns<MongoReadOnlyDocBase, CancellationToken>((doc, cancellationToken) =>
//           {
//               return Task.FromResult(new MongoDocUpdated(new BsonDocument()
//               {
//                   {"_id",  param.Itemid.Guid.ToBsonBinaryData()},
//                   {"ownerid",  _userProfile.ProfileId.Guid.ToBsonBinaryData()},
//                   {"option", new BsonDocument(){
//                        {"category",  new BsonArray(){ {1},{10000 },{10001 } } }
//                    } }
//               }));
//           });

//        _dbContextMock.MongoDocSetupUpdateAll();

        
        

//        //WHEN
//        var handler = new BuilderItemMediatorHandler(_userProfile, _dbContextMock, _handlerMock.Object, _userLibraryMock.Object);
//        var result = await handler.Handle(param, CancellationToken.None);
//        //THEN
//        Assert.Equal(param.Itemid, result.Id);
//    }

//    [Fact()]
//    public async Task Handle_SaleStopItemParam()
//    {
//        //GIVEN
//        var param = new SaleStopItemParam
//        {
//            Itemid = UuidGenerater.NewUuid(SvcDomainType.MetaItem)
//        };
        

//        _dbContextMock.MockDocRepository.Setup(x => x.GetAsync<MongoReadOnlyDocBase>(It.IsAny<ItemDoc>(), It.IsAny<CancellationToken>()))
//           .Returns<MongoReadOnlyDocBase, CancellationToken>((doc, cancellationToken) =>
//           {
//               return Task.FromResult(new MongoDocUpdated(new BsonDocument()
//               {
//                   {"_id",  param.Itemid.Guid.ToBsonBinaryData()},
//                   {"ownerid",  _userProfile.ProfileId.Guid.ToBsonBinaryData()},
//                   {"option", new BsonDocument(){
//                        {"category",  new BsonArray(){ {1},{10000 },{10001 } } },
//                        {"sale_status", 4 }
//                    } }
//               }));
//           });

//        _dbContextMock.MockDocRepository.Setup(x => x.GetAsync<MongoReadOnlyDocBase>(It.IsAny<MarketProductDoc>(), It.IsAny<CancellationToken>()))
//           .Returns<MongoReadOnlyDocBase, CancellationToken>((doc, cancellationToken) =>
//           {
//               return Task.FromResult(new MongoDocUpdated(new BsonDocument()
//               {
//                   {"_id",  param.Itemid.Guid.ToBsonBinaryData()},
//                   {"ownerid",  _userProfile.ProfileId.Guid.ToBsonBinaryData()},
//                   {"option", new BsonDocument(){
//                        {"category",  new BsonArray(){ {1},{10000 },{10001 } } }
//                    } }
//               }));
//           });

//        _dbContextMock.MongoDocSetupUpdateAll();

//        //WHEN
//        var handler = new BuilderItemMediatorHandler(_userProfile, _dbContextMock, _handlerMock.Object, _userLibraryMock.Object);
//        var result = await handler.Handle(param, CancellationToken.None);
//        //THEN
//        Assert.Equal(param.Itemid, result.Id);
//    }

//    [Fact()]
//    public async Task Handle_GetItemTemplateListParam()
//    {
//        //GIVEN
//        var param = new GetItemTemplateListParam
//        {
//            W = _userProfile.WorldId,
//            Page = 1,
//            Limit = 50,
//            Category = new int[] { 1, 10000, 10001 }
//        };

//        var dbContextMock = new Mock<IDbContext>();
//        dbContextMock.Setup(x => x.GetListAsync("item_template", It.IsAny<FilterDefinition<ItemTemplate>>(), It.IsAny<FindOptions<ItemTemplate>>(), It.IsAny<CancellationToken>()))
//            .ReturnsAsync(() =>
//            {

//                var itemid = UuidGenerater.NewUuid(SvcDomainType.MetaItem);
//                var itemTemplate = new ItemTemplate()
//                {
//                    Id = itemid,
//                    Creatorid = _userProfile.ProfileId,
//                    Ownerid = _userProfile.ProfileId,
//                    Userid = _userProfile.UserId,
//                    Worldid = _userProfile.WorldId,
//                    CreatorType = 1,
//                    Status = 5,
//                    AssetType = 1,
//                    Txt = new ItemTemplateTxt
//                    {
//                        Title = new Common.DataTypes.CvLocaleText { Ko = "title" },
//                        Desc = new Common.DataTypes.CvLocaleText { Ko = "desc" },
//                        Hashtag = new string[] { "hashtag" }
//                    },
//                    Option = new ItemTemplateOption()
//                    {
//                        Version = 1,
//                        PublishVersion = 1,
//                        PublishReviewStatus = 4,
//                        SaleStatus = 4,
//                        SaleReviewStatus = 4,
//                        DelStatus = 1,
//                        Category = new int[] { 1, 10000, 10001 },
//                        Price = new ItemTemplatePrice[] { new ItemTemplatePrice { Type = 1, Amount = 0 } }
//                    }
//                };
//                itemTemplate.Resource = new ItemTemplateResource(itemTemplate, 1);
//                ICollection<ItemTemplate> list = new List<ItemTemplate> { itemTemplate };
//                return list;
//            });


//        //WHEN
//        var handler = new BuilderItemMediatorHandler(_userProfile, dbContextMock.Object, _handlerMock.Object, _userLibraryMock.Object);
//        var result = await handler.Handle(param, CancellationToken.None);

//        //THEN
//        Assert.Single(result.List);
//        Assert.Equal(_userProfile.ProfileId, result.List.First().Ownerid);
//        Assert.Equal(1, result.List.First().Option.Price!.First().Type);
//    }
//}