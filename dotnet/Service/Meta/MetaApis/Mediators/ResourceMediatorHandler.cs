using Colorverse.Common;
using Colorverse.Common.Enums;
using Colorverse.Common.Exceptions;
using Colorverse.Application.Mediator;
using Colorverse.Database;
using Colorverse.Meta.DataTypes.Resource;
using Colorverse.Meta.Documents;
using CvFramework.Aws.S3;
using Colorverse.Meta.Helper;

namespace Colorverse.Meta.Mediators;

/// <summary>
/// 리소스 Mediator
/// </summary>
[MediatorHandler]
public class ResourceMediatorHandler : MediatorHandlerBase,
     IMediatorHandler<UploadResourceParam, ResourceDto>,
     IMediatorHandler<GetResourceParam, ResourceDto?>,
     IMediatorHandler<UseResourceParam, ResourceDto>,
     IMediatorHandler<DeleteResourceParam, bool>
{
    private readonly IDbContext _dbContext;
    private readonly IAwsS3 _s3Asset;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="awsS3Factory"></param>
    public ResourceMediatorHandler(IDbContext dbContext, IAwsS3Factory awsS3Factory)
    {
        _dbContext = dbContext;
        _s3Asset = awsS3Factory.GetS3("asset");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ErrorInvalidParam"></exception>
    /// <exception cref="ErrorInternal"></exception>
    public async Task<ResourceDto> Handle(UploadResourceParam request, CancellationToken cancellationToken)
    {
        if (request.Type != EResourceType.Asset
            && request.Type != EResourceType.Item
            && request.Type != EResourceType.Land)
        {
            throw new ErrorInvalidParam(nameof(request.Type));
        }


        var domainType = request.Type switch
        {
            EResourceType.Asset => SvcDomainType.MetaAsset,
            EResourceType.Item => SvcDomainType.MetaItem,
            EResourceType.Land => SvcDomainType.SpaceLand,
            _ => throw new ErrorInvalidParam(nameof(request.Type)),
        };

        // Manifest용 리소스 생성하여 불러오기
        var manifestResource = new ManifestResource(request.Type);
        await manifestResource.LoadAsync(request.File, cancellationToken);

        ResourceDoc resourceDoc;
        if (request.TargetUuid == null)
        {
            resourceDoc = ResourceDoc.Create(domainType);

            resourceDoc.SetManifest(manifestResource);
            await _dbContext.SaveAsync(resourceDoc);
        }
        else
        {
            resourceDoc = await _dbContext.GetDocAsync<ResourceDoc>(request.TargetUuid, cancellationToken);
            
            resourceDoc.SetManifest(manifestResource);

            await resourceDoc.SaveAsync(cancellationToken);
        }

        if(manifestResource.S3Objects.Any())
        {
            var prefixPath = $"{manifestResource.BaseUploadPath}/{resourceDoc.Id}/{resourceDoc.GetValue("version")}";
            
            foreach (var s3Object in manifestResource.S3Objects)
            {
                if (s3Object.Key.Equals("manifest.json") && request.Type.Equals(EResourceType.Asset) && manifestResource.ManifestDocument!.TryGetValue("unused", out var unused))
                {
                    if (!unused.AsBoolean)
                    {
                        s3Object.SetKey(string.Concat(prefixPath, '/', s3Object.Key));
                    }
                }
                else
                {
                    s3Object.SetKey(string.Concat(prefixPath, '/', s3Object.Key));
                }
            }

            if(!await _s3Asset.UploadAsync(manifestResource.S3Objects))
            {
                throw new ErrorInternal("Object upload fail.");
            }
        }

        var resourceVersion = resourceDoc.GetVersion();
        
        var revsionDoc = ResourceRevisionDoc.Create(domainType, resourceVersion);
        await _dbContext.SaveAsync(revsionDoc);
        
        var resource = resourceDoc.To<ResourceDto>();
        return resource;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<ResourceDto?> Handle(GetResourceParam request, CancellationToken cancellationToken)
    {
        var resourceDoc = await _dbContext.GetDocAsync<ResourceDoc>(request.Id, cancellationToken);
        if(!resourceDoc.Exists)
        {
            return default;
        }

        var resource = resourceDoc.To<ResourceDto>();
        return resource;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<ResourceDto> Handle(UseResourceParam request, CancellationToken cancellationToken)
    {
        var resourceDoc = await _dbContext.GetDocAsync<ResourceDoc>(request.Id, cancellationToken);
        if(!resourceDoc.Exists)
        {
            throw new ErrorInternal();
        }

        resourceDoc.UseReference();
        await resourceDoc.SaveAsync();

        var resource = resourceDoc.To<ResourceDto>();
        return resource;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> Handle(DeleteResourceParam request, CancellationToken cancellationToken)
    {
        var resourceDoc = await _dbContext.GetDocAsync<ResourceDoc>(request.Id, cancellationToken);
        if(!resourceDoc.Exists)
        {
            throw new ErrorInternal();
        }

        resourceDoc.UnUseReference();
        await resourceDoc.SaveAsync();       

        await resourceDoc.DeleteAsync();
        return true;
    }
}
