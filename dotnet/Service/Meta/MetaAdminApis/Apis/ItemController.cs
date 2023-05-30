using Colorverse.Apis.Controller;
using Colorverse.Apis.Controller.Models;
using Colorverse.Common.DataTypes;
using Colorverse.MetaAdmin.Apis.DataTypes.Item;
using Colorverse.MetaAdmin.DataTypes.Excel;
using CvFramework.Apis.Extensions.Attributes;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Colorverse.MetaAdmin.Apis;

/// <summary>
/// 아이템 
/// </summary>
[CvRoute]
public class ItemController : ApiControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="mediator"></param>
    public ItemController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// 아이템 조회
    /// </summary>
    /// <param name="itemid"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <see cref="Mediators.ItemMediatorHandler.Handle(GetItemRequest, CancellationToken)"/>
    [HttpGet("{itemid}")]
    public async Task<DataResponse<ItemResponse>> GetItem([UuidBase62] string itemid, CancellationToken cancellationToken)
    {
        var request = new GetItemRequest { Itemid = itemid };
        var response = await _mediator.Send(request, cancellationToken);
        return Success(response);
    }

    /// <summary>
    /// 아이템 리스트
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <see cref="Mediators.ItemMediatorHandler.Handle(GetItemListRequest, CancellationToken)"/>
    [HttpGet("")]
    public async Task<CvAdminListResponse<ItemResponse>> GetItemList([FromQuery]GetItemListRequest request, CancellationToken cancellationToken)
    {
        var resopnse = await _mediator.Send(request, cancellationToken);
        return resopnse;
    }

    /// <summary>
    /// 아이템/애셋 excel Data 등록
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <see cref="Mediators.WorksheetMediatorHandler.Handle(CreateExcelRequest, CancellationToken)"/>
    [HttpPost("batch")]
    public async Task<string> CreateExcel(CancellationToken cancellationToken)
    {
        var param = new CreateExcelRequest();
        return await _mediator.Send(param, cancellationToken);
    }

    /// <summary>
    /// 아이템 강제 생성
    /// </summary>
    /// <remarks>
    /// item worksheet의 데이터를 생성또는 version을 강제 업데이트한다.
    /// </remarks>
    /// <param name="itemid"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <see cref="Mediators.WorksheetMediatorHandler.Handle(CreateItemForceRequest, CancellationToken)"/>
    [HttpPost("{itemid}/force")]
    public async Task<DataResponse<ItemResponse>> CreateItemForce(string itemid, [FromBody]CreateItemForceRequest request, CancellationToken cancellationToken)
    {
        request.Itemid = itemid;
        var response = await _mediator.Send(request, cancellationToken);
        return Success(response);
    }

    /// <summary>
    /// 아이템 수정
    /// </summary>
    /// <param name="itemid"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <see cref="Mediators.ItemMediatorHandler.Handle(UpdateItemRequest, CancellationToken)"/>
    [HttpPatch("{itemid}")]
    public async Task<DataResponse<ItemResponse>> UpdateIitem([UuidBase62] string itemid, UpdateItemRequest request, CancellationToken cancellationToken)
    {
        request.Itemid = itemid;
        var response = await _mediator.Send(request, cancellationToken);
        return Success(response);
    }

    /// <summary>
    /// 아이템 판매/사후 심사 승인
    /// </summary>
    /// <param name="itemid"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <see cref="Mediators.ItemMediatorHandler.Handle(SaleApprovalItemRequest, CancellationToken)"/>
    [HttpPost("{itemid}/sale-approval")]
    public async Task<DataResponse<ItemResponse>> SaleApprovalItem([UuidBase62] string itemid, SaleApprovalItemRequest request, CancellationToken cancellationToken)
    {
        request.Itemid = itemid;
        var response = await _mediator.Send(request, cancellationToken);
        return Success(response);
    }

    /// <summary>
    /// 아이템 판매/사후 심사 반려
    /// </summary>
    /// <param name="itemid"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <see cref="Mediators.ItemMediatorHandler.Handle(SaleRejectionItemRequest, CancellationToken)"/>
    [HttpPost("{itemid}/sale-rejection")]
    public async Task<DataResponse<ItemResponse>> SaleRejectionItem([UuidBase62] string itemid, SaleRejectionItemRequest request, CancellationToken cancellationToken)
    {
        request.Itemid = itemid;
        var response = await _mediator.Send(request, cancellationToken);
        return Success(response);
    }

    /// <summary>
    /// 아이템 삭제 요청
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="itemid"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <see cref="Mediators.ItemMediatorHandler.Handle(DeleteItemRequest, CancellationToken)"/>
    [HttpPost("{itemid}/delete-request")]
    public async Task<DataResponse<ItemResponse>> DeleteItem([UuidBase62] string itemid, CancellationToken cancellationToken)
    {
        var request = new DeleteItemRequest { Itemid = itemid };
        var response = await _mediator.Send(request, cancellationToken);
        return Success(response);
    }

    /// <summary>
    /// 아이템 삭제 요청 취소
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="itemid"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <see cref="Mediators.ItemMediatorHandler.Handle(DeleteCancelItemRequest, CancellationToken)"/>
    [HttpPost("{itemid}/cancel-delete-request")]
    public async Task<DataResponse<ItemResponse>> DeleteCancelItem([UuidBase62] string itemid, CancellationToken cancellationToken)
    {
        var request = new DeleteCancelItemRequest { Itemid = itemid };
        var response = await _mediator.Send(request, cancellationToken);
        return Success(response);
    }

    /// <summary>
    /// 아이템 템플릿 생성
    /// </summary>
    /// <param name="itmeid"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <see cref="Mediators.ItemMediatorHandler.Handle(CreateItemTemplateRequest, CancellationToken)"/>
    [HttpPost("{itemid}/template")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<DataResponse<ItemTemplateResponse>> CreateItemTemplate([UuidBase62] string itmeid, CancellationToken cancellationToken)
    {
        var request = new CreateItemTemplateRequest { Itemid = itmeid };
        var response = await _mediator.Send(request, cancellationToken);
        return Success(response);
    }

    /// <summary>
    /// 아이템 템플릿 리스트
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <see cref="Mediators.ItemMediatorHandler.Handle(GetItemTemplateListRequest, CancellationToken)"/>
    [HttpGet("template")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<CvAdminListResponse<ItemTemplateResponse>> GetItemTemplateList([FromQuery] GetItemTemplateListRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);
        return response;
    }

    /// <summary>
    /// 아이템 템플릿 삭제
    /// </summary>
    /// <param name="itemid"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <see cref="Mediators.ItemMediatorHandler.Handle(DeleteItemTemplateRequest, CancellationToken)"/>
    [HttpDelete("{itemid}/template")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task DeleteItemTemplate([UuidBase62] string itemid, CancellationToken cancellationToken)
    {
        var request = new DeleteItemTemplateRequest { Itemid = itemid };
        await _mediator.Send(request, cancellationToken);
    }
}
