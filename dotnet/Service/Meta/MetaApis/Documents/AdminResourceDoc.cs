using Colorverse.Meta.Helper;
using CvFramework.Common;
using CvFramework.MongoDB.Documents;
using MongoDB.Bson;

namespace Colorverse.Meta.Documents;

public class AdminResourceDoc : MongoDoc
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="targetid"></param>
    /// <returns></returns>
    public static AdminResourceDoc Create(Uuid targetid) => new(targetid);

    /// <summary>
    /// 
    /// </summary>
    public AdminResourceDoc() : base() { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="doc"></param>
    public AdminResourceDoc(BsonDocument doc) : base(doc) { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="uuid"></param>
    /// <returns></returns>
    public AdminResourceDoc(Uuid uuid) : base(uuid) { }

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
}
