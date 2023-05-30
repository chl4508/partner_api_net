using Colorverse.Common;
using Colorverse.MetaAdmin.DataTypes.Resource;
using CvFramework.Common;
using CvFramework.MongoDB.Documents;
using MongoDB.Bson;

namespace Colorverse.MetaAdmin.Documents;

/// <summary>
/// 
/// </summary>
public class ResourceDoc : MongoDoc
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="domainType"></param>
    /// <returns></returns>
    public static ResourceDoc Create(SvcDomainType domainType) => new(UuidGenerater.NewUuid(domainType));

    /// <summary>
    /// 
    /// </summary>
    public ResourceDoc() : base() { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="doc"></param>
    public ResourceDoc(BsonDocument doc) : base(doc) { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="uuid"></param>
    /// <returns></returns>
    public ResourceDoc(Uuid uuid) : base(uuid) { }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override string GetDatabaseName() => "meta";

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override string GetCollectionName() => "resource";

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override string? GetCacheKey() => string.Join(':', GetDatabaseName(), GetCollectionName(), Id);

    /// <summary>
    /// 
    /// </summary>
    public override void OnCreate()
    {
        CurrentDate("stat.created");
        CurrentDate("stat.updated");
    }

    /// <summary>
    /// 
    /// </summary>
    public override void OnUpdate()
    {
        CurrentDate("stat.updated");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="manifest"></param>
    public void SetManifest(ManifestResource manifest)
    {
        Set("type", manifest.Type);
        Inc("version", 1);
        Set("option.origin_file_name", manifest.FileName);
        Set("option.file_size", manifest.FileSize);
        Set("option.file_content_type", manifest.FileContentType);

        if (manifest.S3Objects.Any())
        {
            Set("resource.base", manifest.BaseUploadPath);
            var resourceFiles = BsonArray.Create(manifest.S3Objects.Select(x => x.Key).ToArray());
            Set("resource.files", resourceFiles);
        }

        if (manifest.ManifestDocument != null)
        {
            foreach (var target in manifest.ManifestDocument)
            {
                Set(string.Concat("manifest.", target.Name), target.Value);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void UseReference()
    {
        Inc("reference", 1);
    }

    /// <summary>
    /// 
    /// </summary>
    public void UnUseReference()
    {
        Inc("reference", -1);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public int GetVersion() => GetInt32("version");

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public string GetResourceBase() => GetString("resource", "base");

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public string[] GetResourceFiles()
    {
        var files = GetBsonValue("resource", "files")?.AsBsonArray;
        if (files == null || files.Count == 0)
        {
            return Array.Empty<string>();
        }
        return files.Select(x => x.AsString).ToArray();
    }
}
