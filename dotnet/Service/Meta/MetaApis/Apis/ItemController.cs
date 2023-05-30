using Colorverse.Apis.Controller;
using Colorverse.Apis.Controller.Models;
using Colorverse.Application.Session;
using Colorverse.Common.DataTypes;
using Colorverse.Common.Errors;
using Colorverse.Common.Exceptions;
using Colorverse.Common.Helper;
using Colorverse.Meta.Apis.DataTypes.Item;
using CvFramework.Apis.Extensions.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Text.Json;

namespace Colorverse.Meta.Apis;

/// <summary>
/// 아이템
/// </summary>
[CvRoute]
[Authorize]
public class ItemController : ApiControllerBase
{
    private readonly IMediator _mediator;
    private readonly IContextUserProfile _profile;

    /// <summary>
    /// 아이템 API 관련 설정
    /// </summary>
    /// <param name="mediator"></param>
    /// <param name="profile"></param>
    public ItemController(IMediator mediator, IContextUserProfile profile)
    {
        _mediator = mediator;
        _profile = profile;
    }

    /// <summary>
    /// 아이템 상세 정보 
    /// </summary>
    /// <remarks>
    /// 판매중인 아이템정보만 조회된다. <br></br>
    /// jwt token 없어도 사용이가능하다.  <br></br>
    /// </remarks>
    /// <param name="itemid"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <see cref="Mediators.ItemMediatorHandler.Handle(GetItemRequest, CancellationToken)"/>
    [ResponseCachePublic(1)]
    [HttpGet("-/{itemid}")]
    [AllowAnonymous]
    public async Task<DataResponse<ItemResponse>> GetItem([UuidBase62] string itemid, [FromQuery] GetItemRequest request, CancellationToken cancellationToken)
    {
        request.Itemid = itemid;
        var response = await _mediator.Send(request, cancellationToken);
        return Success(response);
    }

    /// <summary>
    /// 나의 아이템 상세 정보 
    /// </summary>
    /// <remarks>
    /// 나의 아이템정보가 조회된다. <br></br>
    /// </remarks>
    /// <param name="itemid"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <see cref="Mediators.ItemMediatorHandler.Handle(GetItemRequest, CancellationToken)"/>
    [ResponseCachePublic(1)]
    [HttpGet("me/{itemid}")]
    public Task<DataResponse<ItemResponse>> GetMeItem([UuidBase62] string itemid, [FromQuery] GetItemRequest request, CancellationToken cancellationToken)
    {
        request.MeCheck = true;
        return GetItem(itemid, request, cancellationToken);
    }

    /// <summary>
    /// 아이템 리스트
    /// </summary>
    /// <remarks>
    /// 프로필아이디를통한 판매중인 아이템 목록조회
    /// </remarks>
    /// <param name="profileid"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <see cref="Mediators.ItemMediatorHandler.Handle(GetItemListRequest, CancellationToken)"/>
    [HttpGet("{profileid}")]
    public async Task<CvListResponse<ItemResponse>> GetItemList([UuidBase62] string profileid, [FromQuery] GetItemListRequest request, CancellationToken cancellationToken)
    {
        request.Profileid = profileid;
        var response = await _mediator.Send(request, cancellationToken);
        return response;
    }

    /// <summary>
    /// 나의 아이템 리스트
    /// </summary>
    /// <remarks>
    /// 나의 프로필아이디를 통한 아이템 목록조회 <br></br>
    /// </remarks>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <see cref="Mediators.ItemMediatorHandler.Handle(GetItemListRequest, CancellationToken)"/>
    [HttpGet("me")]
    public Task<CvListResponse<ItemResponse>> GetItemList([FromQuery] GetItemListRequest request, CancellationToken cancellationToken)
    {
        request.MeCheck = true;
        return GetItemList(_profile.ProfileId, request, cancellationToken);
    }

    /// <summary>
    /// 아이템 생성
    /// </summary>
    /// <remarks>
    /// 아이템을 생성한다. 아이템은 asset_type이 1로 고정이다.
    /// </remarks>
    /// <param name="itemid"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <see cref="Mediators.ItemMediatorHandler.Handle(CreateItemRequest, CancellationToken)"/>
    [HttpPost("-/{itemid}")]
    public async Task<DataResponse<ItemResponse>> CreateItem([UuidBase62] string itemid, [FromBody] CreateItemRequest request, CancellationToken cancellationToken)
    {
        request.Itemid = itemid;
        var response = await _mediator.Send(request, cancellationToken);
        return Success(response);
    }

    /// <summary>
    /// 아이템 수정
    /// </summary>
    /// <remarks>
    /// 아이템을 수정한다. 아이템은 asset_type이 1로 고정이다.
    /// </remarks>
    /// <param name="itemid"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <see cref="Mediators.ItemMediatorHandler.Handle(UpdateItemRequest, CancellationToken)"/>
    [HttpPatch("-/{itemid}")]
    public async Task<DataResponse<ItemResponse>> UpdateItem([UuidBase62] string itemid, [FromBody] UpdateItemRequest request, CancellationToken cancellationToken)
    {
        request.Itemid = itemid;
        var response = await _mediator.Send(request, cancellationToken);
        return Success(response);
    }

    /// <summary>
    /// 아이템 판매심사 요청
    /// </summary>
    /// <remarks>
    /// 나의 아이템 판매심사를 요청한다.
    /// </remarks>
    /// <param name="itemid"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <see cref="Mediators.ItemMediatorHandler.Handle(SaleInspectionItemRequest, CancellationToken)"/>
    [HttpPost("-/{itemid}/sale-inspection")]
    public async Task<DataResponse<ItemResponse>> SaleInspectionItem([UuidBase62] string itemid, [FromBody] SaleInspectionItemRequest request, CancellationToken cancellationToken)
    {
        request.Itemid = itemid;
        var response = await _mediator.Send(request, cancellationToken);
        return Success(response);
    }

    /// <summary>
    /// 아이템 판매
    /// </summary>
    /// <remarks>
    /// 아이템 판매심사가 승인된 나의 아이템 판매를 요청한다.
    /// </remarks>
    /// <param name="itemid"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <see cref="Mediators.ItemMediatorHandler.Handle(SaleStartItemRequest, CancellationToken)"/>
    [HttpPost("-/{itemid}/sale-start")]
    public async Task<DataResponse<ItemResponse>> SaleStartItem([UuidBase62] string itemid, [FromBody] SaleStartItemRequest request, CancellationToken cancellationToken)
    {
        request.Itemid = itemid;
        var response = await _mediator.Send(request, cancellationToken);
        return Success(response);
    }

    /// <summary>
    /// 아이템 판매 중단
    /// </summary>
    /// <remarks>
    /// 아이템 판매심사가 승인된 나의 아이템 판매를 중단한다.
    /// </remarks>
    /// <param name="itemid"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <see cref="Mediators.ItemMediatorHandler.Handle(SaleStopItemRequest, CancellationToken)"/>
    [HttpPost("-/{itemid}/sale-stop")]
    public async Task<DataResponse<ItemResponse>> SaleStopItem([UuidBase62] string itemid, CancellationToken cancellationToken)
    {
        var request = new SaleStopItemRequest { Itemid = itemid };
        var response = await _mediator.Send(request, cancellationToken);
        return Success(response);
    }

    /// <summary>
    /// 아이템 템플릿 생성
    /// </summary>
    /// <remarks>
    /// 아이템 템플릿을 생성한다.<br></br>
    /// 일반 유저는 사용할수없다.
    /// </remarks>
    /// <param name="itemid"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <see cref="Mediators.ItemMediatorHandler.Handle(CreateItemTemplateRequest, CancellationToken)"/>
    [HttpPost("-/{itemid}/template")]
    public async Task<DataResponse<ItemTemplateResponse>> CreateItemTemplate([UuidBase62] string itemid, CancellationToken cancellationToken)
    {
        var request = new CreateItemTemplateRequest { Itemid = itemid };
        var response = await _mediator.Send(request, cancellationToken);
        return Success(response);
    }

    /// <summary>
    /// 아이템 템플릿 리스트
    /// </summary>
    /// <remarks>
    /// 판매중인 템플릿 리스트를 보여준다.
    /// </remarks>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <see cref="Mediators.ItemMediatorHandler.Handle(GetItemTemplateListRequest, CancellationToken)"/>
    [HttpGet("template")]
    public async Task<CvListResponse<ItemTemplateResponse>> GetItemTemplateList([FromQuery] GetItemTemplateListRequest request, CancellationToken cancellationToken)
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
    [HttpDelete("-/{itemid}/template")]
    public async Task DeleteItemTemplate([UuidBase62] string itemid, CancellationToken cancellationToken)
    {
        var request = new DeleteItemTemplateRequest { Itemid = itemid };
        await _mediator.Send(request, cancellationToken);
    }

    /// <summary>
    /// manifest 조회
    /// </summary>
    /// <param name="itemid"></param>
    /// <param name="version"></param>
    /// <param name="format"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ErrorNotfoundId"></exception>
    //[ResponseCachePublic(60)]
    [HttpGet("-/{itemid}/{version}/manifest.{format}")]
    [AllowAnonymous]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetItemManifest([UuidBase62] string itemid, int version, string format, CancellationToken cancellationToken)
    {
        var url = PublicUrlHelper.GetItemManifestUri(itemid, version);
        HttpClientHandler handler = new()
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        };
        HttpClient client = new(handler);
        HttpResponseMessage response = await client.GetAsync(url, cancellationToken);
        if (response.StatusCode != HttpStatusCode.OK)
        {
            throw new ErrorNotfoundId(itemid);
        }

        try
        {
            var responseBodyStream = await response.Content.ReadAsStreamAsync(cancellationToken);
            if (format.Contains("zip"))
            {
                return File(responseBodyStream, "application/zip", "manifest.json.zip");
            }

            using var zipArchive = new ZipArchive(responseBodyStream, ZipArchiveMode.Read);
            var archiveFileCount = zipArchive.Entries.Count;
            if (archiveFileCount == 0)
            {
                throw new ErrorBadRequest("Invaild resource file. (zip empty)");
            }
            var entry = zipArchive.Entries[0];
            using var entryStream = entry.Open();
            var stream = new MemoryStream();
            await entryStream.CopyToAsync(stream, cancellationToken);
            stream.Seek(0, SeekOrigin.Begin);
            if (entry.FullName.EndsWith("manifest.json") && format.Contains("json"))
            {
                var json = JsonDocument.Parse(Encoding.ASCII.GetString(stream.ToArray()));
                return new JsonResult(json);
            }
        }
        catch (Exception e)
        {
            throw new ErrorBadRequest(e + "Invaild resource file.");
        }
        return new JsonResult(null);
    }
}

