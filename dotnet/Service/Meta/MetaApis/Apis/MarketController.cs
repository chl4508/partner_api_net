using Colorverse.Apis.Controller;
using Colorverse.Apis.Controller.Models;
using Colorverse.Common.DataTypes;
using Colorverse.Meta.Apis.DataTypes.Market;
using CvFramework.Apis.Extensions.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Colorverse.Meta.Apis;

/// <summary>
/// 마켓
/// </summary>
[CvRoute]
[Authorize]
public class MarketController : ApiControllerBase
{
    private readonly IMediator _mediator;


    /// <summary>
    /// Markets API 생성자
    /// </summary>
    /// <param name="mediator"></param>
    public MarketController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// 마켓 상세 정보
    /// </summary>
    /// <remarks>
    /// 마켓의 status 는 sale_status 와 동일하다. <br></br>
    /// </remarks>
    /// <param name="productid"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <see cref="Mediators.MarketMediatorHandler.Handle(GetMarketRequest, CancellationToken)"/>
    [ResponseCachePublic(1)]
    [HttpGet("{productid}")]
    public async Task<DataResponse<MarketResponse>> GetMarket([UuidBase62] string productid, CancellationToken cancellationToken)
    {
        var request = new GetMarketRequest { Productid = productid };
        var response = await _mediator.Send(request, cancellationToken);
        return Success(response);
    }

    /// <summary>
    /// 마켓 리스트
    /// </summary>
    /// <remarks>
    /// 기본적으로 비매품을 포함한 마켓리스트가 조회된다.<br></br>
    /// 비매품만 확인하고싶으면 nonesale = true , 비매품만제외하고싶으면 nonesale = false
    /// </remarks>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <see cref="Mediators.MarketMediatorHandler.Handle(GetMarketListRequest, CancellationToken)"/>
    [HttpGet("search")]
    public async Task<CvListResponse<MarketResponse>> GetMarketList([FromQuery] GetMarketListRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);
        return response;
    }

    /// <summary>
    /// 마켓 리스트 구매
    /// </summary>
    /// <remarks>
    /// 단일구매일시 배열값 데이터 하나만 넣으면된다.
    /// </remarks>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <see cref="Mediators.MarketMediatorHandler.Handle(PurchaseMarketListRequest, CancellationToken)"/>
    [HttpPost("purchase")]
    public async Task<DataResponse<MarketOrderResponse>> PurchaseMarketList([FromBody] PurchaseMarketListRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);
        return Success(response);
    }

    /// <summary>
    /// 선물 리스트 조회
    /// </summary>
    /// <remarks>
    /// MS03 일정 미정
    /// </remarks>
    /// <returns></returns>
    [HttpGet("gift")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task GetGifts()
    {
        await Task.Delay(1);
    }

    /// <summary>
    /// 선물 보내기
    /// </summary>
    /// <remarks>
    /// MS03 일정 미정
    /// </remarks>
    /// <returns></returns>
    [HttpPost("gift")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task SendGift()
    {
        await Task.Delay(1);
    }

    /// <summary>
    /// 선물 상세 정보
    /// </summary>
    /// <remarks>
    /// MS03 일정 미정
    /// </remarks>
    /// <returns></returns>
    [HttpGet("gift/{gift_id}")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task GetGift()
    {
        await Task.Delay(1);
    }

    /// <summary>
    /// 선물 수락
    /// </summary>
    /// <remarks>
    /// MS03 일정 미정
    /// </remarks>
    /// <returns></returns>
    [HttpPost("gift/{gift_id}/accept")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task AcceptGift()
    {
        await Task.Delay(1);
    }

    /// <summary>
    /// 선물 거절
    /// </summary>
    /// <remarks>
    /// MS03 일정 미정
    /// </remarks>
    /// <returns></returns>
    [HttpPost("gift/{gift_id}/refuse")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task RefuseGift()
    {
        await Task.Delay(1);
    }
}

