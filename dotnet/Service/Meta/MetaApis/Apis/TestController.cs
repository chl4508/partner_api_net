using Colorverse.Apis.Controller;
using Colorverse.Common;
using Colorverse.Common.Exceptions;
using Colorverse.Common.Helper;
using Colorverse.Meta.Apis.DataTypes.Item;
using CvFramework.Apis.Extensions.Attributes;
using CvFramework.Bson;
using CvFramework.Common;
using MetaLibrary.Domains;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Text.Json;

namespace Colorverse.Meta.Apis;
/// <summary>
/// Test 관련 API
/// </summary>
[CvRoute]
public class TestController : ApiControllerBase
{
    /// <summary>
    /// 
    /// </summary>
    public TestController()
    {
    }

    /// <summary>
    /// throw test
    /// </summary>
    /// <returns></returns>
    [HttpGet("throw")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public IActionResult ThrowTest()
    {
        throw new TestException();
    }

    /// <summary>
    /// uuid 생성
    /// </summary>
    /// <returns></returns>
    [HttpGet("uuid")]
    public IActionResult CreateUuid(SvcDomainType domainType)
    {
        var uuid = UuidGenerater.NewUuid(domainType);
        return new JsonResult(new
        {
            domainType = uuid.DomainType,
            uuid = uuid.ToString(),
            base62 = uuid.Base62,
            guid = uuid.Guid
        });
    }

    /// <summary>
    /// guid to uuid 변환
    /// </summary>
    /// <returns></returns>
    [HttpGet("uuid/guid/{guid}")]
    public IActionResult GetUuidFromGuid(string guid)
    {
        if(!Guid.TryParse(guid, out var _))
        {
            return BadRequest("");
        }
        var uuid = Uuid.FromGuid(guid);
        return new JsonResult(new
        {
            domainType = (SvcDomainType)uuid.DomainType,
            uuid = uuid.ToString(),
            base62 = uuid.Base62,
            guid = uuid.Guid
        });
    }

    /// <summary>
    /// base62 to uuid 변환
    /// </summary>
    /// <returns></returns>
    [HttpGet("uuid/base62/{base62}")]
    public IActionResult GetUuidFromBase62([UuidBase62]string base62)
    {
        if (!Uuid.TryParseByBase62(base62, out var uuid))
        {
            return BadRequest("");
        }
        return new JsonResult(new
        {
            domainType = (SvcDomainType)uuid.DomainType,
            uuid = uuid.ToString(),
            base62 = uuid.Base62,
            guid = uuid.Guid
        });
    }

    /// <summary>
    /// web 별 request url 결과값 및 길이
    /// </summary>
    /// <remarks>
    /// 웹 브라우저(chrome, safri... )별로 request url 길이가 다르므로 평균적인 길이체크를위한 API
    /// <br></br>
    /// itemIds 의 itemid 및 assetid 는 콤마 (,) 로 구분지어 쿼리를 날린다. ( 인코딩된 url 콤마 : %2C , 길이 : 3 )
    /// 
    /// ex) itemids=DCwlmaPVktDw4EIZoLX7o,1szYQa4WxrKwzRc3WT0DY
    /// <br></br>
    /// public: false cache: no
    /// </remarks>
    /// <param name="itemids"></param>
    /// <returns></returns>
    [HttpGet("url/return")]
    public IActionResult GetUrlReturn([FromQuery] string itemids)
    {
        var query = HttpContext.Request.QueryString;
#pragma warning disable S1075
        string devUrl = "https://dev.colorverseapis.com/v1/meta/version" + query;
        string irUrl = "https://ir.colorverseapis.com/v1/meta/version" + query;
        string realUrl = "https://colorverseapis.com/v1/meta/version" + query;
#pragma warning restore S1075
        var count = 0;
        if (itemids != null)
        {
            count = itemids.Split(",").Length;
        }

        return new JsonResult(new[] {
            new { url = devUrl , length = devUrl.Length , idsCount = count },
            new { url = irUrl , length = irUrl.Length , idsCount = count },
            new { url = realUrl , length = realUrl.Length , idsCount = count }
        });
    }

    /// <summary>
    /// GetUuidList
    /// </summary>
    /// <param name="domainType"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    [HttpGet("uuid/list")]
    public IEnumerable<JsonResult> GetUuidList(SvcDomainType domainType, int count)
    {
        var list = new List<JsonResult>();
        for (int i = 0; i < count; i++)
        {
            var uuid = UuidGenerater.NewUuid(domainType);
            var result = new JsonResult(new
            {
                domainType = uuid.DomainType,
                uuid = uuid.ToString(),
                base62 = uuid.Base62,
                guid = uuid.Guid
            });
            list.Add(result);
        }
        return list;
    }

    /// <summary>
    /// ublnQMdHCVhikVXMsURfs
    /// </summary>
    /// <param name="targetid"></param>
    /// <param name="version"></param>
    /// <param name="domainType"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("manifest")]
    public async Task<JsonDocument> GetManifest([FromQuery] string targetid, [FromQuery] int version, [FromQuery] SvcDomainType domainType, CancellationToken cancellationToken)
    {
        HttpClient client;
        string? url;
        if (domainType.Equals(SvcDomainType.MetaItem))
        {
            url = PublicUrlHelper.GetItemManifestUri(targetid, version);
            HttpClientHandler handler = new()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
            client = new(handler);
            HttpResponseMessage response = await client.GetAsync(url, cancellationToken);
            if(response.StatusCode != HttpStatusCode.OK)
            {
                throw new ErrorBadRequest("Not Found Manifest Url");
            }
            
            var responseBodyStream = await response.Content.ReadAsStreamAsync(cancellationToken);

            using var zipArchive = new ZipArchive(responseBodyStream, ZipArchiveMode.Read);

            var entry = zipArchive.Entries[0];
            using var entryStream = entry.Open();
            var stream = new MemoryStream();
            await entryStream.CopyToAsync(stream, cancellationToken);
            stream.Seek(0, SeekOrigin.Begin);
            if (entry.FullName.EndsWith("manifest.json"))
            {
                return JsonDocument.Parse(Encoding.ASCII.GetString(stream.ToArray()));
            }
        }
        else if (domainType.Equals(SvcDomainType.MetaAsset))
        {
            url = PublicUrlHelper.GetAssetManifestUri("ublnQMdHCVhikVXMsURfs", version);
            client = new();
            HttpResponseMessage response = await client.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
            return JsonDocument.Parse(responseBody);
        }
        else
        {
            throw new ErrorBadRequest("Not Used SvcType : " + domainType.ToString());
        }

        return JsonDocument.Parse("");
    }
}

