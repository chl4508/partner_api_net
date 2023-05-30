//using Colorverse.ApisTests;
//using Colorverse.Application.Session;
//using Colorverse.Common;
//using Colorverse.Meta.DataTypes.Item;
//using Colorverse.Meta.Documents;
//using CvFramework.Common;
//using CvFramework.MongoDB.Doc;
//using CvFramework.MongoDB.Documents;
//using CvFramework.MongoDB.Extensions;
//using MediatR;
//using MongoDB.Bson;
//using Moq;
//using Xunit;

//namespace Colorverse.Meta.Mediators.Tests;

///// <summary>
///// 
///// </summary>
//public class ItemMediatorHandlerTests : BaseTests
//{
//    private readonly IContextUserProfile _userProfile;
//    private readonly TestDbContextImpl _dbContextMock;

//    public ItemMediatorHandlerTests()
//    {
//        _userProfile = GetTestUserProfile();
//        _dbContextMock = GetTestDbContext();
//    }


//    [Fact]
//    public async Task Handle_GetItemParam()
//    {
//        // GIVEN
//        var param = new GetItemParam
//        {
//            Itemid = UuidGenerater.NewUuid(SvcDomainType.MetaItem)
//        };

        
//        _dbContextMock.MockDocRepository.Setup(x => x.GetAsync<MongoReadOnlyDocBase>(It.IsAny<ItemDoc>(), It.IsAny<CancellationToken>()))
//            .Returns<MongoReadOnlyDocBase, CancellationToken>((doc, cancellationToken)=>{
//                ((ItemDoc)doc).TestData(_userProfile.ProfileId, _userProfile.WorldId, _userProfile.UserId);
//                return Task.FromResult(new MongoDocUpdated(doc.BsonDocument));
//            });

//        // WHEN
//        var handler = new ItemMediatorHandler(_dbContextMock);
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
//    public async Task Handle_GetItemStatusListParam()
//    {
//        // GIVEN
//        var successIds = new List<Uuid>();
//        for (int i= 0; i < 3; i++){
//            successIds.Add(UuidGenerater.NewUuid(SvcDomainType.MetaItem));
//        }
//        var failIds = new List<Uuid>();
//        for (int i = 0; i < 3; i++)
//        {
//            failIds.Add(UuidGenerater.NewUuid(SvcDomainType.MetaItem));
//        }

//        var param = new GetItemStatusListParam()
//        {
//            Itemids = successIds
//        };
        
//        _dbContextMock.MockDocRepository.Setup(x => x.GetAsync<MongoReadOnlyDocBase>(It.IsAny<ItemDoc>(), It.IsAny<CancellationToken>()))
//            .Returns<MongoReadOnlyDocBase, CancellationToken>((doc, cancellationToken) => {
//                var itemDoc = ((ItemDoc)doc);
//                if (successIds.Contains(itemDoc.Id))
//                {
//                    ////itemDoc.TestData(_userProfile.ProfileId, _userProfile.WorldId, _userProfile.UserId);
//                    return Task.FromResult(new MongoDocUpdated(new BsonDocument(){
//                        {"_id" , itemDoc.Id.Guid.ToBsonBinaryData()},
//                        {"ownerid" , _userProfile.ProfileId.Guid.ToBsonBinaryData()},
//                        {"owner_type" , 1},
//                        {"status" , 5},
//                        {"option" , new BsonDocument(){ { "sale_status", 4 }, { "publish_version", 1 } } },
//                    }));
//                }
//                else
//                {
//                    return Task.FromResult(new MongoDocUpdated(null));
//                }
                
//            });
//        _dbContextMock.MockDocRepository.Setup(x => x.GetAsync<MongoReadOnlyDocBase>(It.IsAny<AssetDoc>(), It.IsAny<CancellationToken>()))
//           .Returns<MongoReadOnlyDocBase, CancellationToken>((doc, cancellationToken) => {
//               var assetDoc = ((AssetDoc)doc);
//               if (successIds.Contains(assetDoc.Id))
//               {
//                   ////itemDoc.TestData(_userProfile.ProfileId, _userProfile.WorldId, _userProfile.UserId);
//                   return Task.FromResult(new MongoDocUpdated(new BsonDocument(){
//                        {"_id" , assetDoc.Id.Guid.ToBsonBinaryData()},
//                        {"ownerid" , _userProfile.ProfileId.Guid.ToBsonBinaryData()},
//                        {"owner_type" , 1},
//                        {"status" , 5},
//                        {"type", 2 },
//                        {"option" , new BsonDocument(){ { "sale_status", 4 }, { "publish_version", 1 } } },
//                    }));
//               }
//               else
//               {
//                   return Task.FromResult(new MongoDocUpdated(null));
//               }
//           });


//        // WHEN
//        var handler = new ItemMediatorHandler(_dbContextMock);
//        var result = await handler.Handle(param, CancellationToken.None);

//        // THEN
//        Assert.Equal(3, result.List.Count());
//    }

//}

