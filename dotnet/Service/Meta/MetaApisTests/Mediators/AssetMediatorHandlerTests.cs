//using Colorverse.ApisTests;
//using Colorverse.Application.Session;
//using Colorverse.Common;
//using Colorverse.Common.Helper;
//using Colorverse.Meta.DataTypes.Asset;
//using Colorverse.Meta.Documents;
//using CvFramework.MongoDB.Doc;
//using CvFramework.MongoDB.Documents;
//using CvFramework.MongoDB.Extensions;
//using MediatR;
//using MongoDB.Bson;
//using Moq;
//using Xunit;

//namespace Colorverse.Meta.Mediators.Tests;

//public class AssetMediatorHandlerTests : BaseTests
//{
//    private readonly IContextUserProfile _userProfile;
//    private readonly TestDbContextImpl _dbContextMock;

//    public AssetMediatorHandlerTests()
//    {
//        _userProfile = GetTestUserProfile();
//        _dbContextMock = GetTestDbContext();
//    }

//    [Fact()]
//    public async Task Handle_GetAssetParam()
//    {
//        // GIVEN
//        var param = new GetAssetParam
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
//        var handler = new AssetMediatorHandler(_dbContextMock);
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
//}