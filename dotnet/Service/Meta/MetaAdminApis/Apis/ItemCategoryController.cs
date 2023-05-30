using Colorverse.Apis.Controller;
using Colorverse.Apis.Controller.Models;
using Colorverse.MetaAdmin.Apis.DataTypes.ItemCategory;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Colorverse.MetaAdmin.Apis;

/// <summary>
/// 아이템 카테고리 
/// </summary>
[CvRoute("item/category")]
public class ItemCategoryController : ApiControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// 
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

    /// <summary>
    /// 아이템 카테고리 데이터 동기화
    /// </summary>
    /// <param name="worksheetId">worksheet id</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <see cref="Mediators.ItemCategoryMediatorHandler.Handle(SyncItemCategory, CancellationToken)"/>
    [HttpPost("sync")]
    public async Task<ListResponse<ItemCategoryResponse>> Sync( 
        [FromQuery(Name ="worksheet_id")] string? worksheetId,
        CancellationToken cancellationToken)
    {        
        var request = new SyncItemCategory(worksheetId);
        var response = await _mediator.Send(request, cancellationToken);
        return response;
    }
}
