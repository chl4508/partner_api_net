using Colorverse.Apis.Controller;
using Colorverse.Meta.Apis.DataTypes.Resource;
using Colorverse.Meta.Apis.Mapper;
using Colorverse.Meta.DataTypes.Resource;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Colorverse.Meta.Apis;

/// <summary>
/// 업로드 관련 API
/// </summary>
[CvRoute]
[Authorize]
public class ResourceController : ApiControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="mediator"></param>
    public ResourceController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// 테스트
    /// </summary>
    /// <returns></returns>
    [HttpGet("test")]
    public async Task<IDictionary<string, string>> Test()
    {
        await Task.Delay(1);
        var map = new Dictionary<string, string>();
        return map;
    }

    /// <summary>
    /// 임시 업로드 ( 추후 통합 업로드서버로 대체 됩니다.)
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <see cref="Mediators.ResourceMediatorHandler.Handle(UploadResourceParam, CancellationToken)"/>
    [Consumes("multipart/form-data", IsOptional = false)]
    [HttpPost()]
    public async Task<ResourceResponse> Upload([FromForm] UploadResourceRequest request, CancellationToken cancellationToken = default)
    {
        var param = ResourceMapper.To(request);
        var result = await _mediator.Send(param, cancellationToken);
        var response = result.To<ResourceResponse>();
        return response;
    }

}

