using Colorverse.Apis.Controller;
using Colorverse.Apis.Controller.Models;
using Colorverse.Application.Session;
using Colorverse.Common.DataTypes;
using Colorverse.Meta.Apis.DataTypes.Asset;
using CvFramework.Apis.Extensions.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Colorverse.Meta.Apis;

/// <summary>
/// 애셋
/// </summary>
[CvRoute]
[Authorize]
public class AssetController : ApiControllerBase
{
    private readonly IMediator _mediator;
    private readonly IContextUserProfile _profile;

    /// <summary>
    /// 애셋 API 관련 설정
    /// </summary>
    /// <param name="mediator"></param>
    /// <param name="profile"></param>
    public AssetController(IMediator mediator, IContextUserProfile profile)
    {
        _mediator = mediator;
        _profile = profile;
    }

    /// <summary>
    /// 애셋 상세 정보
    /// </summary>
    /// <remarks>
    /// 판매중인  애셋만 조회된다 <br></br>
    /// jwt token 없어도 사용이가능하다. <br></br>
    /// </remarks>
    /// <param name="assetid"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <see cref="Mediators.AssetMediatorHandler.Handle(GetAssetRequest, CancellationToken)"/>
    [ResponseCachePublic(1)]
    [HttpGet("-/{assetid}")]
    [AllowAnonymous]
    public async Task<DataResponse<AssetResponse>> GetAsset([UuidBase62] string assetid, [FromQuery] GetAssetRequest request, CancellationToken cancellationToken)
    {
        request.Assetid = assetid;
        var response = await _mediator.Send(request, cancellationToken);
        return Success(response);
    }

    /// <summary>
    /// 나의 애셋 상세 정보
    /// </summary>
    /// <remarks>
    /// 나의 애셋가 조회된다 <br></br>
    /// </remarks>
    /// <param name="assetid"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <see cref="Mediators.AssetMediatorHandler.Handle(GetAssetRequest, CancellationToken)"/>
    [ResponseCachePublic(1)]
    [HttpGet("me/{assetid}")]
    public Task<DataResponse<AssetResponse>> GetMeAsset([UuidBase62] string assetid, [FromQuery] GetAssetRequest request, CancellationToken cancellationToken)
    {
        request.MeCheck = true;
        return GetAsset(assetid, request, cancellationToken);
    }

    /// <summary>
    /// 애셋 리스트
    /// </summary>
    /// <remarks>
    /// 프로필아이디를 통한 판매중인 애셋 목록조회
    /// </remarks>
    /// <param name="profileid"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <see cref="Mediators.AssetMediatorHandler.Handle(GetAssetListRequest, CancellationToken)"/>
    [HttpGet("{profileid}")]
    public async Task<CvListResponse<AssetResponse>> GetAssetList([UuidBase62] string profileid, [FromQuery] GetAssetListRequest request, CancellationToken cancellationToken)
    {
        request.Profileid = profileid;
        var response = await _mediator.Send(request, cancellationToken);
        return response;
    }

    /// <summary>
    /// 나의 애셋 리스트
    /// </summary>
    /// <remarks>
    /// 나의 프로필아이디를 통한 애셋 목록조회
    /// </remarks>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <see cref="Mediators.AssetMediatorHandler.Handle(GetAssetListRequest, CancellationToken)"/>
    [HttpGet("me")]
    public Task<CvListResponse<AssetResponse>> GetAssetList([FromQuery] GetAssetListRequest request, CancellationToken cancellationToken)
    {
        request.MeCheck = true;
        return GetAssetList(_profile.ProfileId, request, cancellationToken);
    }

    /// <summary>
    /// 애셋 생성
    /// </summary>
    /// <remarks>
    /// 애셋을 생성한다.
    /// </remarks>
    /// <param name="assetid"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <see cref="Mediators.AssetMediatorHandler.Handle(CreateAssetRequest, CancellationToken)"/>
    [HttpPost("-/{assetid}")]
    public async Task<DataResponse<AssetResponse>> CreateAsset([UuidBase62] string assetid, [FromBody] CreateAssetRequest request, CancellationToken cancellationToken)
    {
        request.Assetid = assetid;
        var response = await _mediator.Send(request, cancellationToken);
        return Success(response);
    }

    /// <summary>
    /// 애셋 판매 심사 요청
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="assetid"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <see cref="Mediators.AssetMediatorHandler.Handle(SaleInspectionAssetRequest, CancellationToken)"/>
    [HttpPost("-/{assetid}/sale-inspection")]
    public async Task<DataResponse<AssetResponse>> SaleInspectionAsset([UuidBase62] string assetid, [FromBody] SaleInspectionAssetRequest request, CancellationToken cancellationToken)
    {
        request.Assetid = assetid;
        var response = await _mediator.Send(request, cancellationToken);
        return Success(response);
    }

    /// <summary>
    /// 애셋 판매
    /// </summary>
    /// <remarks>
    /// 애셋 상태를 판매상태로 변경한다. <br></br>
    /// </remarks>
    /// <param name="assetid"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <see cref="Mediators.AssetMediatorHandler.Handle(SaleStartAssetRequest, CancellationToken)"/>
    [HttpPost("-/{assetid}/sale-start")]
    public async Task<DataResponse<AssetResponse>> SaleStartAsset([UuidBase62] string assetid, [FromBody] SaleStartAssetRequest request, CancellationToken cancellationToken)
    {
        request.Assetid = assetid;
        var response = await _mediator.Send(request, cancellationToken);
        return Success(response);
    }

    /// <summary>
    /// 애셋 판매 중단
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="assetid"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <see cref="Mediators.AssetMediatorHandler.Handle(SaleStopAssetRequest, CancellationToken)"/>
    [HttpPost("-/{assetid}/sale-stop")]
    public async Task<DataResponse<AssetResponse>> SaleStopAsset([UuidBase62] string assetid, CancellationToken cancellationToken)
    {
        var request = new SaleStopAssetRequest { Assetid = assetid };
        var response = await _mediator.Send(request, cancellationToken);
        return Success(response);
    }
}
