using Colorverse.ApisTests;
using Colorverse.Common.Enums;
using Colorverse.Meta.DataTypes.Resource;
using CvFramework.Apis.Utility;
using CvFramework.Aws.S3;
using Moq;
using Xunit;

namespace Colorverse.Meta.Mediators.Tests;

/// <summary>
/// 
/// </summary>
public class ResourceMediatorHandlerTests : BaseTests
{
    [Fact]
    public async Task Handle_UploadResourceParam()
    {
        var awsS3Mock = new Mock<IAwsS3>();
        awsS3Mock.Setup(x=>x.UploadAsync(It.IsAny<IEnumerable<AwsS3Object>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var awsS3FactoryMock = new Mock<IAwsS3Factory>();
        awsS3FactoryMock.Setup(x=>x.GetS3(It.IsAny<string>())).Returns(awsS3Mock.Object);

        var file = new FileInfo("../../../../../../../sample/meta/asset/resource.zip");
        using var resourceFileStream = File.OpenRead(file.FullName);

        var formFile = FormFileUtility.Create(file.OpenRead(), "Resource", file.Name);        
        formFile.ContentType = "application/zip";

        var dbContextMock = GetTestDbContext();
        dbContextMock.MongoDocSetupUpdateAll();

        var param = new UploadResourceParam
        {
            File = formFile,
            Type = EResourceType.Asset,
            TargetUuid = null
        };

        // WHEN
        var handler = new ResourceMediatorHandler(dbContextMock, awsS3FactoryMock.Object);
        var result = await handler.Handle(param, CancellationToken.None);

        // THEN
        Assert.NotNull(result.Id);
        Assert.Equal((byte)param.Type, result.Type);
        Assert.Equal(1, result.Version);

        Assert.Equal(formFile.FileName, result.Option.OriginFileName);
        Assert.Equal(formFile.Length, result.Option.FileSize);
        Assert.Equal(formFile.ContentType, result.Option.FileContentType);

        // GIVEN
        param.TargetUuid = result.Id;

        dbContextMock.MongoDocSetupGetAll();

        // WHEN
        result = await handler.Handle(param, CancellationToken.None);

        //
        Assert.NotNull(result.Id);
        Assert.Equal((byte)param.Type, result.Type);
        Assert.Equal(1, result.Version);

        Assert.Equal(formFile.FileName, result.Option.OriginFileName);
        Assert.Equal(formFile.Length, result.Option.FileSize);
        Assert.Equal(formFile.ContentType, result.Option.FileContentType);
        
    }

}

