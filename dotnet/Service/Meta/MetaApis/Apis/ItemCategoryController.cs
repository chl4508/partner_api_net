using Colorverse.Apis.Controller;
using Colorverse.Apis.Controller.Models;
using Colorverse.Meta.Apis.DataTypes.ItemCategory;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Colorverse.Meta.Apis;

/// <summary>
/// 아이템 카테고리
/// </summary>
[CvRoute("item/category")]
[Authorize]
public class ItemCategoryController : ApiControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// 생성자
    /// </summary>
    /// <param name="mediator"></param>
    public ItemCategoryController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    /// <summary>
    /// 아이템 카테고리 조회
    /// </summary>
    /// <param name="depth">depth</param>
    /// <param name="parentid">카테고리 ids (,로 구분 가능)</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <see cref="Mediators.ItemCategoryMediatorHandler.Handle(GetItemCategoryListRequest, CancellationToken)"/>
    [HttpGet()]
    public async Task<ListResponse<ItemCategoryResponse>> GetList([FromQuery] int? depth, [FromQuery] string? parentid, CancellationToken cancellationToken = default)
    {
        var request = new GetItemCategoryListRequest(depth, parentid); 
        var response = await _mediator.Send(request, cancellationToken);
        return response;
    }
}

