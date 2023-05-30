using Colorverse.Apis.Controller;
using Colorverse.Apis.Controller.Models;
using Colorverse.Meta.Apis.DataTypes.Cart;
using Colorverse.Meta.Apis.DataTypes.Market;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Colorverse.Meta.Apis;

/// <summary>
/// 장바구니
/// </summary>
[CvRoute]
[Authorize]
public class CartController : ApiControllerBase
{
    private readonly IMediator _mediator;


    /// <summary>
    /// CartController API 생성자
    /// </summary>
    /// <param name="mediator"></param>
    public CartController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// 장바구니 리스트
    /// </summary>
    /// <remarks>
    /// 프로필아이디 기반의 장바구니 목록조회 <br></br>
    /// </remarks>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <see cref="Mediators.CartMediatorHandler.Handle(GetCartListRequest, CancellationToken)"/>
    [HttpGet()]
    public async Task<DataResponse<CartResponse>> GetCartList(CancellationToken cancellationToken)
    {
        var request = new GetCartListRequest();
        var response = await _mediator.Send(request, cancellationToken);
        return Success(response);
    }

    /// <summary>
    /// 장바구니 추가
    /// </summary>
    /// <remarks>
    /// 단일추가일시 배열값 데이터 하나만 넣으면된다.
    /// </remarks>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <see cref="Mediators.CartMediatorHandler.Handle(CreateCartListRequest, CancellationToken)"/>
    [HttpPost()]
    public async Task<DataResponse<CartResponse>> CreateCartList([FromBody] CreateCartListRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);
        return Success(response);
    }

    /// <summary>
    /// 장바구니 리스트 삭제
    /// </summary>
    /// <remarks>
    /// productids 의 값은 콤마 (,) 로 구분지어 쿼리를 날린다. <br></br>
    /// ex) productids=DCwlmaPVktDw4EIZoLX7o,1szYQa4WxrKwzRc3WT0DY 
    /// </remarks>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <see cref="Mediators.CartMediatorHandler.Handle(DeleteCartListRequest, CancellationToken)"/>
    [HttpDelete("")]
    public async Task<DataResponse<CartResponse>> DeleteCartList([FromQuery] DeleteCartListRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);
        return Success(response);
    }

    /// <summary>
    /// 장바구니 리스트 구매
    /// </summary>
    /// <remarks>
    /// 단일구매일시 배열값 데이터 하나만 넣으면된다. 
    /// </remarks>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <see cref="Mediators.CartMediatorHandler.Handle(PurchaseCartListRequest, CancellationToken)"/>
    [HttpPost("purchase")]
    public async Task<DataResponse<MarketOrderResponse>> PurchaseCartList([FromBody] PurchaseCartListRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);
        return Success(response);
    }

}
