using System.IO.Compression;
using System.Text.Json;
using Colorverse.Common.Enums;
using Colorverse.Common.Exceptions;
using CvFramework.Apis.Utility;
using CvFramework.Aws.S3;
using MongoDB.Bson;

namespace Colorverse.Meta.Helper;

/// <summary>
/// 
/// </summary>
public class ManifestResource
{
    /// <summary>
    /// 
    /// </summary>
    /// <value></value>
    public IList<AwsS3Object>? s3Objects;

    /// <summary>
    /// 
    /// </summary>
    public string BaseUploadPath { get; set;}

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public IList<AwsS3Object> S3Objects { get => s3Objects ?? Array.Empty<AwsS3Object>();}

    /// <summary>
    /// 
    /// </summary>
    /// <value></value>
    public EResourceType Type { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <value></value>
    public string FileName { get; private set; } = string.Empty;

    /// <summary>
    /// 
    /// </summary>
    /// <value></value>
    public long FileSize { get; private set;} = 0;

    /// <summary>
    /// 
    /// </summary>
    /// <value></value>
    public string FileContentType{ get; private set; } = string.Empty;

    /// <summary>
    /// 
    /// </summary>
    /// <value></value>
    public BsonDocument? ManifestDocument { get; private set; }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    public ManifestResource(EResourceType type)
    {
        Type = type;
        BaseUploadPath = Type switch
        {
            EResourceType.Asset => "meta/item",
            EResourceType.Item => "meta/item",
            EResourceType.Land => "space/land",
            _ => throw new ErrorInternal(nameof(Type)),
        };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="file"></param>
    /// <param name="cancellationToken"></param>
    public async Task LoadAsync(IFormFile file, CancellationToken cancellationToken)
    {
        FileSize = file.Length;
        FileName = file.FileName;
        FileContentType = file.ContentType;
        
        if(file.FileName.EndsWith(".zip"))
        {
            using var fileStream = file.OpenReadStream();

            using var zipArchive = new ZipArchive(fileStream, ZipArchiveMode.Read);

            var archiveFileCount = zipArchive.Entries.Count;
            if(archiveFileCount == 0)
            {
                throw new ErrorBadRequest("Invaild resource file. (zip empty)"); 
            }
            if(archiveFileCount == 1)
            {
                var entry = zipArchive.Entries[0];
                using var entryStream = entry.Open();

                var stream = new MemoryStream();
                await entryStream.CopyToAsync(stream);
                stream.Seek(0, SeekOrigin.Begin);

                if(entry.FullName.EndsWith("manifest.json"))
                {
                    LoadManifest(stream);
                    stream.Seek(0, SeekOrigin.Begin);
                } 
                else if(entry.FullName.EndsWith("manifest.json.zip"))
                {
                    LoadManifest(stream, true);
                    stream.Seek(0, SeekOrigin.Begin);
                }
                else
                {
                    throw new ErrorBadRequest("Invaild resource file. (manifest.json)");
                }

                if("manifest.json.zip".Equals(file.FileName))
                {
                    fileStream.Seek(0, SeekOrigin.Begin);

                    stream.SetLength(0);
                    await fileStream.CopyToAsync(stream);
                    stream.Seek(0, SeekOrigin.Begin);

                    var s3Object = new AwsS3Object(file.FileName, stream);
                    s3Object.ContentType = MimeTypeUtility.ToContentType(file.FileName);
                    s3Object.AddMeta("origin_file_name", file.FileName);
                    ////s3Object.AddMeta("cv-uuid", resourceDoc.Id);

                    s3Objects = new List<AwsS3Object>(1);
                    s3Objects.Add(s3Object);
                }
                else
                {
                    var s3Object = new AwsS3Object(entry.Name, stream);
                    s3Object.ContentType = MimeTypeUtility.ToContentType(entry.Name);
                    s3Object.AddMeta("origin_file_name", file.FileName);
                    ////s3Object.AddMeta("cv-uuid", resourceDoc.Id);

                    s3Objects = new List<AwsS3Object>(1);
                    s3Objects.Add(s3Object);
                }
            }
            else
            {
                bool manifestFinded = false;

                s3Objects = new List<AwsS3Object>(archiveFileCount);
                foreach (var entry in zipArchive.Entries)
                {
                    using var entryStream = entry.Open();

                    var stream = new MemoryStream();
                    await entryStream.CopyToAsync(stream);
                    stream.Seek(0, SeekOrigin.Begin);

                    if(!manifestFinded && "manifest.json".Equals(entry.FullName))
                    {
                        LoadManifest(stream);
                        stream.Seek(0, SeekOrigin.Begin);
                        manifestFinded = true;
                    }
                    else if(!manifestFinded && "manifest.json.zip".Equals(entry.FullName))
                    {
                        LoadManifest(stream, true);
                        stream.Seek(0, SeekOrigin.Begin);
                        manifestFinded = true;
                    }

                    var s3Object = new AwsS3Object(entry.FullName, stream);
                    s3Object.ContentType = MimeTypeUtility.ToContentType(entry.Name);
                    ////s3Object.AddMeta("cv-uuid", resourceDoc.Id);

                    s3Objects.Add(s3Object);
                }

                    
                if(!manifestFinded)
                {
                    throw new ErrorBadRequest("Invaild resource file. (manifest.json)");
                }
            }
        }
        else if(file.FileName.EndsWith(".json"))
        {
            using var fileStream = file.OpenReadStream();

            var stream = new MemoryStream();
            await fileStream.CopyToAsync(stream);
            stream.Seek(0, SeekOrigin.Begin);
            
            LoadManifest(stream);
            stream.Seek(0, SeekOrigin.Begin);

            var s3Object = new AwsS3Object("manifest.json", stream);
            s3Object.ContentType = MimeTypeUtility.ToContentType(file.FileName);

            s3Object.AddMeta("origin_file_name", file.FileName);
            ////s3Object.AddMeta("cv-uuid", resourceDoc.Id);

            s3Objects = new List<AwsS3Object>(1);
            s3Objects.Add(s3Object);
        }
        else
        {
            throw new ErrorBadRequest("Invalid resource.");
        }
        
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="archive"></param>
    private void LoadManifest(Stream stream, bool archive)
    {
        if(!archive)
        {
            LoadManifest(stream);
            return;
        }

        using var zipArchive = new ZipArchive(stream, ZipArchiveMode.Read, true);
        if(zipArchive.Entries.Count != 1)
        {
            throw new ErrorBadRequest("Invalid resource. (manifest.json.zip)"); 
        }

        var entryManifest = zipArchive.Entries[0];
        using var manifestStream = entryManifest.Open();
        LoadManifest(manifestStream);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="stream"></param>
    private void LoadManifest(Stream stream)
    {
        var jsonDoc = JsonDocument.Parse(stream);
        var rootElement = jsonDoc.RootElement;
        

        if (!rootElement.TryGetProperty("main", out var mainElement))
        {
            throw new ErrorBadRequest("Invalid manifest formating. (main)");
        }
        
        if(!mainElement.TryGetProperty("type", out var mainTypeElement))
        {
            throw new ErrorBadRequest("Invalid manifest formating. (main.type)");
        }

        string mainType = mainTypeElement.GetString()!;
        if(string.IsNullOrWhiteSpace(mainType))
        {
            throw new ErrorBadRequest("Empty manifest data. (main.type)");
        }

        switch(Type)
        {
            //--------------------
            //- Asset
            //--------------------
            case EResourceType.Asset:
            {
                if(string.Equals("Item", mainType) || string.Equals("Land", mainType))
                {
                    throw new ErrorBadRequest("Invalid manifest data. (main.type)");
                }
            }
            break;

            //--------------------
            //- Item
            //--------------------
            case EResourceType.Item:
            {
                if(!string.Equals("Item", mainType))
                {
                    throw new ErrorBadRequest("Invalid manifest data. (main.type)");
                }
            }
            break;

            //--------------------
            //- Land
            //--------------------
            case EResourceType.Land:
            {
                mainType = "Land"; // 현재 json 데이터에 왜 Item이라고 되어있지?
                if(!string.Equals("Land", mainType))
                {
                    throw new ErrorBadRequest("Invalid manifest data. (main.type)");
                }
            }
            break;
        }

        ManifestDocument = new BsonDocument();

        if (rootElement.TryGetProperty("unused", out var unUsedElement))
        {
            ManifestDocument.Set("unused", unUsedElement.GetBoolean());
        }

        ManifestDocument.Set("main.type", mainType);
        
        if(rootElement.TryGetProperty("subItemIds", out var elements))
        {
            if(elements.ValueKind != JsonValueKind.Array)
            {
                throw new ErrorBadRequest("Invalid manifest formating. (subItemIds)");
            }

            var arrayElements = elements.EnumerateArray();
            if(arrayElements.Any())
            {
                var itemids = new BsonArray();
                foreach(var element in arrayElements)
                {
                    var itemid = element.GetString();
                    if(!string.IsNullOrWhiteSpace(itemid))
                    {
                        itemids.Add(itemid);
                    }
                }
                ManifestDocument.Set("subItemIds", itemids);
            }
        }
    }
}