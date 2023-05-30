using Cys.Apis.Controller;
using Cys.Apis.Controller.Models;
using Cys.Common.DataTypes;
using Cys.MetaAdmin.Apis.DataTypes.Asset;
using CvFramework.Apis.Extensions.Attributes;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Cys.MetaAdmin.Apis;

/// <summary>
/// 애셋
/// </summary>
[CvRoute]
public class AssetController : ApiControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="mediator"></param>
    public AssetController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// 애셋 조회
    /// </summary>
    /// <param name="assetid"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <see cref="Mediators.AssetMediatorHandler.Handle(GetAssetRequest, CancellationToken)"/>
    [HttpGet("{assetid}")]
    public async Task<DataResponse<AssetResponse>> GetAsset([UuidBase62] string assetid, CancellationToken cancellationToken)
    {
        var request = new GetAssetRequest() { Assetid = assetid };
        var response = await _mediator.Send(request, cancellationToken);
        return Success(response);
    }

    /// <summary>
    /// 애셋 리스트
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <see cref="Mediators.AssetMediatorHandler.Handle(GetAssetListRequest, CancellationToken)"/>
    [HttpGet("")]
    public async Task<CvAdminListResponse<AssetResponse>> GetAssetList([FromQuery]GetAssetListRequest request,CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);
        return response;
    }

    /// <summary>
    /// 애셋 강제 생성
    /// </summary>
    /// <remarks>
    /// asset worksheet의 데이터를 생성또는 version을 강제 업데이트한다.
    /// </remarks>
    /// <param name="assetid"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <see cref="Mediators.WorksheetMediatorHandler.Handle(CreateAssetForceRequest, CancellationToken)"/>
    [HttpPost("{assetid}/force")]
    public async Task<DataResponse<AssetResponse>> CreateAssetForce(string assetid, [FromBody]CreateAssetForceRequest request, CancellationToken cancellationToken)
    {
        request.Assetid = assetid;
        var response = await _mediator.Send(request, cancellationToken);
        return Success(response);
    }

    /// <summary>
    /// 애셋 수정
    /// </summary>
    /// <param name="assetid"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <see cref="Mediators.AssetMediatorHandler.Handle(UpdateAssetRequest, CancellationToken)"/>
    [HttpPatch("{assetid}")]
    public async Task<DataResponse<AssetResponse>> UpdateAsset([UuidBase62] string assetid, [FromBody] UpdateAssetRequest request, CancellationToken cancellationToken)
    {
        request.Assetid = assetid;
        var response = await _mediator.Send(request, cancellationToken);
        return Success(response);
    }

    /// <summary>
    /// 애셋 판매/사후 심사 승인
    /// </summary>
    /// <param name="assetid"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <see cref="Mediators.AssetMediatorHandler.Handle(SaleApprovalAssetRequest, CancellationToken)"/>
    [HttpPost("{assetid}/sale-approval")]
    public async Task<DataResponse<AssetResponse>> SaleApprovalAsset([UuidBase62] string assetid, SaleApprovalAssetRequest request,  CancellationToken cancellationToken)
    {
        request.Assetid = assetid;
        var response = await _mediator.Send(request, cancellationToken);
        return Success(response);
    }

    /// <summary>
    /// 애셋 판매/사후 심사 반려
    /// </summary>
    /// <remarks>
    /// 애셋은 판매심사가 반려되면 애셋이 즉시삭제된다.
    /// </remarks>
    /// <param name="assetid"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <see cref="Mediators.AssetMediatorHandler.Handle(SaleRejectionAssetRequest, CancellationToken)"/>
    [HttpPost("{assetid}/sale-rejection")]
    public async Task<DataResponse<AssetResponse>?> SaleRejectionAsset([UuidBase62] string assetid, SaleRejectionAssetRequest request, CancellationToken cancellationToken)
    {
        request.Assetid = assetid;
        var response = await _mediator.Send(request, cancellationToken);
        if (response == null) return null;
        return Success(response);
    }

    /// <summary>
    /// 애셋 삭제 요청
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="assetid"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <see cref="Mediators.AssetMediatorHandler.Handle(DeleteAssetRequest, CancellationToken)"/>
    [HttpPost("{assetid}/delete-request")]
    public async Task<DataResponse<AssetResponse>> DeleteAsset([UuidBase62] string assetid, CancellationToken cancellationToken)
    {
        var request = new DeleteAssetRequest { Assetid = assetid };
        var response = await _mediator.Send(request, cancellationToken);
        return Success(response);
    }

    /// <summary>
    /// 애셋 삭제 요청 취소
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="assetid"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <see cref="Mediators.AssetMediatorHandler.Handle(DeleteCancelAssetRequest, CancellationToken)"/>
    [HttpPost("{assetid}/cancel-delete-request")]
    public async Task<DataResponse<AssetResponse>> DeleteCancelAsset([UuidBase62] string assetid, CancellationToken cancellationToken)
    {
        var request = new DeleteCancelAssetRequest { Assetid = assetid };
        var response = await _mediator.Send(request, cancellationToken);
        return Success(response);
    }

}
