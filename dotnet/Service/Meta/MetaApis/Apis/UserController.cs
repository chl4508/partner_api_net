using Colorverse.Apis.Controller;
using Colorverse.Common.DataTypes;
using Colorverse.Meta.Apis.DataTypes.Item;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Colorverse.Meta.Apis;

/// <summary>
/// 유저 아이템/애셋
/// </summary>
[CvRoute]
[Authorize]
public class UserController : ApiControllerBase
{
    private readonly IMediator _mediator;


    /// <summary>
    /// 아이템 API 관련 설정
    /// </summary>
    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// 나의 아이템 리스트
    /// </summary>
    /// <remarks>
    /// 나의 보유 아이템 목록을 조회한다. ( 구매아이템, 생성후 게시된아이템)
    /// </remarks>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <see cref="Mediators.UserMediatorHandler.Handle(GetUserItemListRequest, CancellationToken)"/>
    [HttpGet("item")]
    public async Task<CvListResponse<UserItemResponse>> GetUserItemList([FromQuery] GetUserItemListRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);
        return response;
    }
}