using Colorverse.Apis.Controller;
using CvFramework.Common;
using Microsoft.AspNetCore.Mvc;
using Colorverse.MetaLibrary;

namespace Colorverse.Meta.Apis;

/// <summary>
/// Nats Test
/// </summary>
[CvRoute("/test/metalib")]
public class TestMetaLibController : ApiControllerBase
{
    private IMetaLibrary _metaLib;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="metaLib"></param>
    public TestMetaLibController(IMetaLibrary metaLib)
    {
        _metaLib = metaLib;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="version"></param>
    /// <returns></returns>
    [Obsolete("Nats Test")]
    [HttpGet("resource/get/{id}/{version}")]
    public async Task<JsonResult> GetResouce(string id, int version = 1)
    {
        var uuid = Uuid.FromBase62(id);
        var data = await _metaLib.GetResource(uuid, version);
        return new JsonResult(Success(data));
    }


}

