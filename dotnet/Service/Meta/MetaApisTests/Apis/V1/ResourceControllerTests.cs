using Colorverse.ApisTests;
using Colorverse.Common;
using Colorverse.Common.Enums;
using Colorverse.Database;
using Colorverse.Meta.Apis.DataTypes.Resource;
using Colorverse.Meta.DataTypes.Resource;
using Colorverse.Meta.Documents;
using Colorverse.Meta.Mediators;
using CvFramework.Apis.Utility;
using CvFramework.Aws.S3;
using CvFramework.MongoDB;
using CvFramework.MongoDB.Doc;
using CvFramework.MongoDB.Documents;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using Moq;
using Xunit;

namespace Colorverse.Meta.Apis.V1.Tests;

/// <summary>
/// 
/// </summary>
public class ResourceControllerTests : ControllerTests
{
    private readonly ResourceController _controller;
    private readonly Mock<IMongoDocRepository> _docRepositoryMock;

    /// <summary>
    /// 
    /// </summary>
    public ResourceControllerTests() : base(true)
    {
        var mongoDbMock = _services.AddMock<IMongoDb>();

        _docRepositoryMock = new Mock<IMongoDocRepository>();
        mongoDbMock.Setup(x=>x.DocRepository).Returns(_docRepositoryMock.Object);

        var awsS3Mock = new Mock<IAwsS3>();
        awsS3Mock.Setup(x=>x.UploadAsync(It.IsAny<IEnumerable<AwsS3Object>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var awsS3FactoryMock = _services.AddMock<IAwsS3Factory>();
        awsS3FactoryMock.Setup(x=>x.GetS3(It.IsAny<string>())).Returns(awsS3Mock.Object);

        _services.AddScoped<IDbContext, DbContextImpl>();

        _services.AddMediaorHandler<UploadResourceParam,ResourceDto>(typeof(ResourceMediatorHandler));

        _controller = _services.BuildWithAddGet<ResourceController>();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task Upload_Asset_OK()
    {
        // GIVEN
        var file = new FileInfo("../../../../../../../sample/meta/asset/resource.zip");
        using var resourceFileStream = File.OpenRead(file.FullName);

        var formFile = FormFileUtility.Create(file.OpenRead(), nameof(UploadResourceRequest.File), file.Name);        
        formFile.ContentType = "application/zip";

        var request = new UploadResourceRequest
        {
            File = formFile,
            Type = EResourceType.Asset,
            //TargetUuid = ""
        };
        
        _docRepositoryMock.Setup(x => x.UpdateAsync<MongoDocBase>(It.IsAny<ResourceDoc>(), It.IsAny<CancellationToken>()))
            .Returns<MongoDocBase, CancellationToken>((doc, cancellationToken)=>{
                doc.TestForceUpdate();
                return Task.FromResult(new MongoDocUpdated(doc.BsonDocument));
            });
        _docRepositoryMock.Setup(x => x.UpdateAsync<MongoDocBase>(It.IsAny<ResourceRevisionDoc>(), It.IsAny<CancellationToken>()))
            .Returns<MongoDocBase, CancellationToken>((doc, cancellationToken)=>{
                doc.TestForceUpdate();
                return Task.FromResult(new MongoDocUpdated(doc.BsonDocument));
            });

        // WHEN
        var response = await _controller.Upload(request);

        // THEN
        Assert.NotNull(response.Id);
        Assert.Equal((byte)request.Type, response.Type);
        Assert.Equal(1, response.Version);

        Assert.Equal(formFile.FileName, response.Option.OriginFileName);
        Assert.Equal(formFile.Length, response.Option.FileSize);
        Assert.Equal(formFile.ContentType, response.Option.FileContentType);
    }

    [Fact]
    public async Task Upload_Asset_Target_OK()
    {
        // GIVEN
        var file = new FileInfo("../../../../../../../sample/meta/asset/resource.zip");
        using var resourceFileStream = File.OpenRead(file.FullName);

        var formFile = FormFileUtility.Create(file.OpenRead(), nameof(UploadResourceRequest.File), file.Name);        
        formFile.ContentType = "application/zip";

        var targetId = UuidGenerater.NewUuid(SvcDomainType.AssetEtc);
        var request = new UploadResourceRequest
        {
            File = formFile,
            Type = EResourceType.Asset,
            TargetId = targetId
        };
        
        _docRepositoryMock.Setup(x => x.GetAsync<MongoReadOnlyDocBase>(It.IsAny<ResourceDoc>(), It.IsAny<CancellationToken>()))
            .Returns<MongoReadOnlyDocBase, CancellationToken>((doc, cancellationToken)=>{
                doc.TestForceGet();
                return Task.FromResult(new MongoDocUpdated(doc.BsonDocument));
            });
            
        _docRepositoryMock.Setup(x => x.UpdateAsync<MongoDocBase>(It.IsAny<ResourceDoc>(), It.IsAny<CancellationToken>()))
            .Returns<MongoDocBase, CancellationToken>((doc, cancellationToken)=>{
                doc.TestForceUpdate();
                return Task.FromResult(new MongoDocUpdated(doc.BsonDocument));
            });

        _docRepositoryMock.Setup(x => x.UpdateAsync<MongoDocBase>(It.IsAny<ResourceRevisionDoc>(), It.IsAny<CancellationToken>()))
            .Returns<MongoDocBase, CancellationToken>((doc, cancellationToken)=>{
                doc.TestForceUpdate();
                return Task.FromResult(new MongoDocUpdated(doc.BsonDocument));
            });
            
        // WHEN
        var response = await _controller.Upload(request);

        // THEN
        Assert.NotNull(response.Id);
        Assert.Equal((byte)request.Type, response.Type);
        Assert.Equal(1, response.Version);

        Assert.Equal(formFile.FileName, response.Option.OriginFileName);
        Assert.Equal(formFile.Length, response.Option.FileSize);
        Assert.Equal(formFile.ContentType, response.Option.FileContentType);
    }

}
