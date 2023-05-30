using Colorverse.Apis.Controller;
using Colorverse.Common;
using Microsoft.AspNetCore.Mvc;

namespace Colorverse.Meta.Apis;

/// <summary>
/// Builder Controller
/// </summary>
[CvRoute]
public class DocsController : ApiControllerBase
{
    /// <summary>
    /// 오류 코드 목록
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpGet("error-codes")]
    public IActionResult GetErrorCodes(CancellationToken cancellationToken)
    {
        return new JsonResult(new
        {
            error_codes = ErrorFactory.GetList(typeof(ErrorCodeMeta))
        });
    }
}