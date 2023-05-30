using Colorverse.Application.Mediator;
using Colorverse.Common.Errors;
using Colorverse.Common.Exceptions;
using Colorverse.Common.Helper;
using Colorverse.Database;
using Colorverse.MetaAdmin.Apis.DataTypes.Asset;
using Colorverse.MetaAdmin.Apis.DataTypes.Item;
using Colorverse.MetaAdmin.DataTypes.Excel;
using Colorverse.MetaAdmin.DataTypes.Resource;
using Colorverse.MetaAdmin.Documents;
using Colorverse.MetaAdmin.Excel;
using CvFramework.Common;
using CvFramework.Excel;
using CvFramework.MongoDB.Extensions;
using CvFramework.Worksheet;
using MongoDB.Bson;
using MongoDB.Driver;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Colorverse.MetaAdmin.Mediators;

/// <summary>
/// 
/// </summary>
public class WorksheetMediatorHandler : MediatorHandlerBase,
    IMediatorHandler<CreateExcelRequest, string>,
    IMediatorHandler<CreateItemForceRequest, ItemResponse>,
    IMediatorHandler<CreateAssetForceRequest, AssetResponse>

{
    private readonly ILogger _logger;
    private readonly IWorksheetLoader _worksheetLoader;
    private readonly IConfiguration _configuration;
    private readonly IDbContext _dbContext;
    private readonly string userid = "XxrVt9UaEiaKay9NWLVJo";
    private readonly string worldid = "1mkSuyvWsZAZKE2cuDzmzI";
    private readonly string profileid = "1OneDRIOUeEoc4RhklAGVU";

    /// <summary>
    /// 
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="worksheetLoader"></param>
    /// <param name="configuration"></param>
    /// <param name="dbContext"></param>
    public WorksheetMediatorHandler(ILogger<WorksheetMediatorHandler> logger, IWorksheetLoader worksheetLoader, IConfiguration configuration, IDbContext dbContext)
    {
        _logger = logger;
        _worksheetLoader = worksheetLoader;
        _dbContext = dbContext;
        _configuration = configuration;
    }

    /// <summary>
    /// 아이템 워크시트 업로드
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task UploadItemWorkSheetData(Stream stream, CancellationToken cancellationToken = default)
    {
        try
        {
            var itemExcelFile = new ExcelFile(stream);
            
            var itemDoc = await itemExcelFile.DeserializeAsync<ItemWorkSheet>();
            if (itemDoc == null)
            {
                throw new ErrorInternal("excel file deserializeAsync fail.");
            }

            foreach (var item in itemDoc.Item)
            {
                if (item.Itemid == null || item.Itemid == "" || item.Itemid.Equals("itemid")) continue;
                item.Profileid = profileid;
                item.Userid = userid;
                item.Worldid = worldid;
                if (item.UseStatus != null && item.UseStatus.Equals("Y"))
                {
                    var resourceDoc = await _dbContext.GetDocAsync<ResourceDoc>(Uuid.FromBase62(item.Itemid!), false);
                    if (resourceDoc.Exists)
                    {
                        var resourceDto = resourceDoc.To<ResourceDto>();
                        item.Manifest = resourceDto.Manifest;
                        item.Version = resourceDto.Version;
                    }
                    else
                    {
                        item.Version = 0;
                    }

                    // revision 생성
                    var revisionFilter = Builders<BsonDocument>.Filter.Eq("itemid", Uuid.FromBase62(item.Itemid!).ToBsonBinaryData());
                    revisionFilter &= Builders<BsonDocument>.Filter.Eq("version", Convert.ToInt32(item.Version));
                    var findRevisionDoc = await _dbContext.GetDocAsync<ItemRevisionDoc>(revisionFilter, false, cancellationToken);
                    if (findRevisionDoc.Exists)
                    {
                        findRevisionDoc.SetItemRevision(1, item.Manifest);
                        await findRevisionDoc.SaveAsync(cancellationToken);
                    }
                    else
                    {
                        var createItemRevision = ItemRevisionDoc.Create(Uuid.FromBase62(item.Itemid!), item.Version);
                        _dbContext.Add(createItemRevision);
                        createItemRevision.SetItemRevision(1, item.Manifest);
                        await createItemRevision.SaveAsync(cancellationToken);
                    }

                    // item 생성
                    var findItemDoc = await _dbContext.GetDocAsync<ItemDoc>(Uuid.FromBase62(item.Itemid!), false, cancellationToken);
                    if (findItemDoc.Exists)
                    {
                        findItemDoc.SetWorksheetItem(item);
                        await findItemDoc.SaveAsync(cancellationToken);
                    }
                    else
                    {
                        var createItem = ItemDoc.Create(Uuid.FromBase62(item.Itemid!));
                        _dbContext.Add(createItem);
                        createItem.SetWorksheetItem(item);
                        await createItem.SaveAsync(cancellationToken);
                        if (resourceDoc.Exists)
                        {
                            resourceDoc.UseReference();
                            _ = await resourceDoc.SaveAsync();
                        }
                    }

                    // user item 생성
                    var userItemFilter = Builders<BsonDocument>.Filter.Eq("itemid", Uuid.FromBase62(item.Itemid!).ToBsonBinaryData());
                    userItemFilter &= Builders<BsonDocument>.Filter.Eq("profileid", Uuid.FromBase62(item.Profileid).ToBsonBinaryData());
                    var findUserItemDoc = await _dbContext.GetDocAsync<UserItemDoc>(userItemFilter, false, cancellationToken);
                    if (findUserItemDoc.Exists)
                    {
                        findUserItemDoc.SetWorksheetUserItem(item);
                        await findUserItemDoc.SaveAsync(cancellationToken);
                    }
                    else
                    {
                        var createUserItem = UserItemDoc.Create(Uuid.FromBase62(item.Itemid!), Uuid.FromBase62(item.Profileid));
                        _dbContext.Add(createUserItem);
                        createUserItem.SetWorksheetUserItem(item);
                        await createUserItem.SaveAsync(cancellationToken);
                    }

                    // market product 생성
                    var findMarketDoc = await _dbContext.GetDocAsync<MarketProductDoc>(Uuid.FromBase62(item.Itemid!), false, cancellationToken);
                    if (findMarketDoc.Exists)
                    {
                        findMarketDoc.SetWorksheetMarket(item);
                        await findMarketDoc.SaveAsync(cancellationToken);
                    }
                    else
                    {
                        var createMarketDoc = MarketProductDoc.Create(Uuid.FromBase62(item.Itemid!));
                        _dbContext.Add(createMarketDoc);
                        createMarketDoc.SetWorksheetMarket(item);
                        await createMarketDoc.SaveAsync(cancellationToken);
                    }
                }
                else
                {
                    // 아이템
                    var findItemDoc = await _dbContext.GetDocAsync<ItemDoc>(Uuid.FromBase62(item.Itemid!), false, cancellationToken);
                    if (findItemDoc.Exists)
                    {
                        findItemDoc.DeleteWorksheetItem();
                        await findItemDoc.SaveAsync(cancellationToken);
                    }

                    // 마켓
                    var findMarketDoc = await _dbContext.GetDocAsync<MarketProductDoc>(Uuid.FromBase62(item.Itemid!), false, cancellationToken);
                    if (findMarketDoc.Exists)
                    {
                        findMarketDoc.DeleteWorksheetMarket();
                        await findMarketDoc.SaveAsync(cancellationToken);
                    }
                }
            }

        }catch(Exception e)
        {
            _logger.LogError(e.ToString());
        }
    }

    /// <summary>
    /// 애셋 워크시트 업로드
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task UploadAssetWorkSheetData(Stream stream, CancellationToken cancellationToken = default)
    {
        try
        {
            var assetExcelFile = new ExcelFile(stream);
            var assetDoc = await assetExcelFile.DeserializeAsync<AssetWorkSheet>();
            if (assetDoc == null)
            {
                throw new ErrorInternal("excel file deserializeAsync fail.");
            }

            foreach (var asset in assetDoc.Asset)
            {
                if (asset.Assetid == null || asset.Assetid == "" || asset.Assetid.Equals("assetid")) continue;
                asset.Profileid = profileid;
                asset.Userid = userid;
                asset.Worldid = worldid;
                if (asset.UseStatus != null && asset.UseStatus.Equals("Y"))
                {
                    var resourceDoc = await _dbContext.GetDocAsync<ResourceDoc>(Uuid.FromBase62(asset.Assetid!), false);
                    if (resourceDoc.Exists)
                    {
                        var resourceDto = resourceDoc.To<ResourceDto>();
                        asset.Manifest = resourceDto.Manifest;
                        asset.Version = resourceDto.Version;
                    }
                    else
                    {
                        asset.Version = 0;
                    }

                    // revision 생성
                    var revisionFilter = Builders<BsonDocument>.Filter.Eq("assetid", Uuid.FromBase62(asset.Assetid!).ToBsonBinaryData());
                    revisionFilter &= Builders<BsonDocument>.Filter.Eq("version", asset.Version);
                    var findRevisionDoc = await _dbContext.GetDocAsync<AssetRevisionDoc>(revisionFilter, false, cancellationToken);
                    if (findRevisionDoc.Exists)
                    {
                        findRevisionDoc.SetAssetRevision(1, asset.Manifest);
                        await findRevisionDoc.SaveAsync(cancellationToken);
                    }
                    else
                    {
                        var createItemRevision = AssetRevisionDoc.Create(Uuid.FromBase62(asset.Assetid!), asset.Version);
                        _dbContext.Add(createItemRevision);
                        createItemRevision.SetAssetRevision(1, asset.Manifest);
                        await createItemRevision.SaveAsync(cancellationToken);
                    }

                    // asset 생성
                    var findAssetDoc = await _dbContext.GetDocAsync<AssetDoc>(Uuid.FromBase62(asset.Assetid!), false, cancellationToken);
                    if (findAssetDoc.Exists)
                    {
                        findAssetDoc.SetWorksheetAsset(asset);
                        await findAssetDoc.SaveAsync(cancellationToken);
                    }
                    else
                    {
                        var createAsset = AssetDoc.Create(Uuid.FromBase62(asset.Assetid!));
                        _dbContext.Add(createAsset);
                        createAsset.SetWorksheetAsset(asset);
                        await createAsset.SaveAsync(cancellationToken);

                        if (resourceDoc.Exists)
                        {
                            resourceDoc.UseReference();
                            _ = await resourceDoc.SaveAsync();
                        }
                    }


                    // market product 생성
                    var findMarketDoc = await _dbContext.GetDocAsync<MarketProductDoc>(Uuid.FromBase62(asset.Assetid!), false, cancellationToken);
                    if (findMarketDoc.Exists)
                    {
                        findMarketDoc.SetWorksheetMarket(asset);
                        await findMarketDoc.SaveAsync(cancellationToken);
                    }
                    else
                    {
                        var createMarketDoc = MarketProductDoc.Create(Uuid.FromBase62(asset.Assetid!));
                        _dbContext.Add(createMarketDoc);
                        createMarketDoc.SetWorksheetMarket(asset);
                        await createMarketDoc.SaveAsync(cancellationToken);
                    }
                }
                else
                {
                    // 애셋
                    var findAssetDoc = await _dbContext.GetDocAsync<AssetDoc>(Uuid.FromBase62(asset.Assetid!), false, cancellationToken);
                    if (findAssetDoc.Exists)
                    {
                        findAssetDoc.DeleteWorksheetAsset();
                        await findAssetDoc.SaveAsync(cancellationToken);
                    }

                    // 마켓
                    var findMarketDoc = await _dbContext.GetDocAsync<MarketProductDoc>(Uuid.FromBase62(asset.Assetid!), false, cancellationToken);
                    if (findMarketDoc.Exists)
                    {
                        findMarketDoc.DeleteWorksheetMarket();
                        await findMarketDoc.SaveAsync(cancellationToken);
                    }
                }

            }
        }
        catch (Exception e)
        {
            _logger.LogError(e.ToString());
        }
    }

    /// <summary>
    /// 아이템/애셋 엑셀 데이터 생성
    /// </summary>
    /// <param name="param"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<string> Handle(CreateExcelRequest param, CancellationToken cancellationToken)
    {
        ////if (!_profile.UserId.ToString().Equals("QPtPSSIIVZvHfyk29gqNU")) // chl4508 ownerid 만 통과
        ////{
        ////    throw new ErrorUnauthorized("invalid auth id " + _profile.UserId);
        ////}

        var reqeustId = Guid.NewGuid().ToString();

        var spredsheetId = _configuration.GetValue<string>("meta:google_sheets_item");
        var itemStream = _worksheetLoader.GetFileStream(spredsheetId!, "item");
        _ = UploadItemWorkSheetData(itemStream);


        var assetStream = _worksheetLoader.GetFileStream(spredsheetId!, "asset");
        _ = UploadAssetWorkSheetData(assetStream);

        return Task.FromResult("reqeustId : " + reqeustId);
    }

    /// <summary>
    /// 아이템 강제생성
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ErrorNotfoundItem"></exception>
    /// <exception cref="ErrorNotFound"></exception>
    /// <exception cref="ErrorInvalidParam"></exception>
    /// <exception cref="ErrorInternal"></exception>
    public async Task<ItemResponse> Handle(CreateItemForceRequest request, CancellationToken cancellationToken)
    {
        var item = new ItemRow
        {
            Profileid = profileid,
            Userid = userid,
            Worldid = worldid
        };
        item.SetItem(request);
        if (item.UseStatus == null || item.UseStatus.Equals("N"))
        {
            // 아이템
            var findUseNItemDoc = await _dbContext.GetDocAsync<ItemDoc>(Uuid.FromBase62(item.Itemid!), false, cancellationToken);
            if (!findUseNItemDoc.Exists)
            {
                throw new ErrorNotfoundItem(item.Itemid!);
            }

            // 마켓
            var findUseNMarketDoc = await _dbContext.GetDocAsync<MarketProductDoc>(Uuid.FromBase62(item.Itemid!), false, cancellationToken);
            if (findUseNMarketDoc.Exists)
            {
                findUseNMarketDoc.DeleteWorksheetMarket();
                await findUseNMarketDoc.SaveAsync(cancellationToken);
            }

            findUseNItemDoc.DeleteWorksheetItem();
            await findUseNItemDoc.SaveAsync(cancellationToken);

            var responseUseN = findUseNItemDoc.To<ItemResponse>();
            responseUseN.Resource ??= new ItemResourceResponse();
            responseUseN.Resource.Manifest = PublicUrlHelper.GetItemManifestUri(responseUseN.Id, responseUseN.Option.Version);
            responseUseN.Resource.Preview = responseUseN.Resource.Preview != null ? PublicUrlHelper.GetResourceImageUri(responseUseN.Resource.Preview) : null;
            responseUseN.Resource.Thumbnail = responseUseN.Resource.Thumbnail != null || responseUseN.Resource.Thumbnail != "" ? PublicUrlHelper.GetResourceImageUri(responseUseN.Resource.Thumbnail!) : PublicUrlHelper.GetItemThumbnailtUri(responseUseN.Id, responseUseN.Option.Version);
            return responseUseN;
        }

        // resource
        var resourceDoc = await _dbContext.GetDocAsync<ResourceDoc>(Uuid.FromBase62(item.Itemid!), false);
        if (!resourceDoc.Exists)
        {
            _logger.LogInformation("resourceDoc.Exists : id={a}", item.Itemid!);
            throw new ErrorNotFound("Not Found Resource id=" + item.Itemid);
        }
        var resourceDto = resourceDoc.To<ResourceDto>();
        item.Manifest = resourceDto.Manifest;
        if (item.Version != resourceDto.Version)
        {
            _logger.LogInformation("itemSheet.Version != resourceDto.Version : id ={a}  requestVersion={b}, dtoVersion={c}", item.Itemid!, item.Version, resourceDto.Version);
            throw new ErrorInvalidParam("Not Matching Resource Version :" + resourceDto.Version);
        }

        // revision 생성
        var revisionFilter = Builders<BsonDocument>.Filter.Eq("itemid", Uuid.FromBase62(item.Itemid!).ToBsonBinaryData());
        revisionFilter &= Builders<BsonDocument>.Filter.Eq("version", Convert.ToInt32(item.Version));
        var revisionDoc = await _dbContext.GetDocAsync<ItemRevisionDoc>(revisionFilter, false, cancellationToken);
        if (revisionDoc.Exists)
        {
            revisionDoc.SetItemRevision(1, item.Manifest);
            await revisionDoc.SaveAsync(cancellationToken);
        }
        else
        {
            revisionDoc = ItemRevisionDoc.Create(Uuid.FromBase62(item.Itemid!), item.Version);
            _dbContext.Add(revisionDoc);
            revisionDoc.SetItemRevision(1, item.Manifest);
            await revisionDoc.SaveAsync(cancellationToken);
        }

        // item 생성
        var findItemDoc = await _dbContext.GetDocAsync<ItemDoc>(Uuid.FromBase62(item.Itemid!), false, cancellationToken);
        if (findItemDoc.Exists)
        {
            findItemDoc.SetWorksheetItem(item);
            await findItemDoc.SaveAsync(cancellationToken);
        }
        else
        {
            findItemDoc = ItemDoc.Create(Uuid.FromBase62(item.Itemid!));
            _dbContext.Add(findItemDoc);
            findItemDoc.SetWorksheetItem(item);
            await findItemDoc.SaveAsync(cancellationToken);

            resourceDoc.UseReference();
            _ = await resourceDoc.SaveAsync();
        }

        // user item 생성
        var userItemFilter = Builders<BsonDocument>.Filter.Eq("itemid", Uuid.FromBase62(item.Itemid!).ToBsonBinaryData());
        userItemFilter &= Builders<BsonDocument>.Filter.Eq("profileid", Uuid.FromBase62(item.Profileid).ToBsonBinaryData());
        var userItemDoc = await _dbContext.GetDocAsync<UserItemDoc>(userItemFilter, false, cancellationToken);
        if (userItemDoc.Exists)
        {
            userItemDoc.SetWorksheetUserItem(item);
            await userItemDoc.SaveAsync(cancellationToken);
        }
        else
        {
            userItemDoc = UserItemDoc.Create(Uuid.FromBase62(item.Itemid!), Uuid.FromBase62(item.Profileid));
            _dbContext.Add(userItemDoc);
            userItemDoc.SetWorksheetUserItem(item);
            await userItemDoc.SaveAsync(cancellationToken);
        }

        // market product 생성
        var marketDoc = await _dbContext.GetDocAsync<MarketProductDoc>(Uuid.FromBase62(item.Itemid!), false, cancellationToken);
        if (marketDoc.Exists)
        {
            marketDoc.SetWorksheetMarket(item);
            await marketDoc.SaveAsync(cancellationToken);
        }
        else
        {
            marketDoc = MarketProductDoc.Create(Uuid.FromBase62(item.Itemid!));
            _dbContext.Add(marketDoc);
            marketDoc.SetWorksheetMarket(item);
            await marketDoc.SaveAsync(cancellationToken);
        }

        var response = findItemDoc.To<ItemResponse>();
        response.Resource ??= new ItemResourceResponse();
        response.Resource.Manifest = PublicUrlHelper.GetItemManifestUri(response.Id, response.Option.Version);
        response.Resource.Preview = response.Resource.Preview != null ? PublicUrlHelper.GetResourceImageUri(response.Resource.Preview) : null;
        response.Resource.Thumbnail = response.Resource.Thumbnail != null || response.Resource.Thumbnail != "" ? PublicUrlHelper.GetResourceImageUri(response.Resource.Thumbnail!) : PublicUrlHelper.GetItemThumbnailtUri(response.Id, response.Option.Version);
        return response;
    }

    /// <summary>
    /// 애셋 강제생성
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ErrorNotfoundId"></exception>
    /// <exception cref="ErrorInternal"></exception>
    /// <exception cref="ErrorNotFound"></exception>
    /// <exception cref="ErrorInvalidParam"></exception>
    public async Task<AssetResponse> Handle(CreateAssetForceRequest request, CancellationToken cancellationToken)
    {
        var asset = new AssetRow()
        {
            Profileid = profileid,
            Userid = userid,
            Worldid = worldid
        };
        asset.SetAsset(request);
        if (asset.UseStatus == null || asset.UseStatus.Equals("N"))
        {
            // 애셋
            var findUseNAssetDoc = await _dbContext.GetDocAsync<AssetDoc>(Uuid.FromBase62(asset.Assetid!), false, cancellationToken);
            if (!findUseNAssetDoc.Exists)
            {
                throw new ErrorNotfoundId(asset.Assetid!);

            }

            // 마켓
            var findUseNMarketDoc = await _dbContext.GetDocAsync<MarketProductDoc>(Uuid.FromBase62(asset.Assetid!), false, cancellationToken);
            if (findUseNMarketDoc.Exists)
            {
                findUseNMarketDoc.DeleteWorksheetMarket();
                await findUseNMarketDoc.SaveAsync(cancellationToken);
            }

            findUseNAssetDoc.DeleteWorksheetAsset();
            await findUseNAssetDoc.SaveAsync(cancellationToken);


            var responseUseN = findUseNAssetDoc.To<AssetResponse>();
            responseUseN.Resource ??= new AssetResourceResponse();
            responseUseN.Resource.Manifest = PublicUrlHelper.GetAssetManifestUri(responseUseN.Id, responseUseN.Option.Version);
            responseUseN.Resource.Thumbnail = responseUseN.Resource.Thumbnail != null || responseUseN.Resource.Thumbnail != "" ? PublicUrlHelper.GetResourceImageUri(responseUseN.Resource.Thumbnail!) : PublicUrlHelper.GetAssetThumbnailtUri(responseUseN.Id, responseUseN.Option.Version);
            return responseUseN;
        }


        var resourceDoc = await _dbContext.GetDocAsync<ResourceDoc>(Uuid.FromBase62(asset.Assetid!), false);
        if (!resourceDoc.Exists)
        {
            _logger.LogInformation("resourceDoc.Exists : id={a}", asset.Assetid!);
            throw new ErrorNotFound("Not Found Resource id=" + asset.Assetid!);
        }
        var resourceDto = resourceDoc.To<ResourceDto>();
        asset.Manifest = resourceDto.Manifest;
        if (asset.Version != resourceDto.Version)
        {
            _logger.LogInformation("assetSheet.Version != resourceDto.Version : id ={a}  requestVersion={b}, dtoVersion={c}", asset.Assetid!, asset.Version, resourceDto.Version);
            throw new ErrorInvalidParam("Not Matching Resource Version :" + resourceDto.Version);
        }

        // revision 생성
        var revisionFilter = Builders<BsonDocument>.Filter.Eq("assetid", Uuid.FromBase62(asset.Assetid!).ToBsonBinaryData());
        revisionFilter &= Builders<BsonDocument>.Filter.Eq("version", asset.Version);
        var revisionDoc = await _dbContext.GetDocAsync<AssetRevisionDoc>(revisionFilter, false, cancellationToken);
        if (revisionDoc.Exists)
        {
            revisionDoc.SetAssetRevision(1, asset.Manifest);
            await revisionDoc.SaveAsync(cancellationToken);
        }
        else
        {
            revisionDoc = AssetRevisionDoc.Create(Uuid.FromBase62(asset.Assetid!), asset.Version);
            _dbContext.Add(revisionDoc);
            revisionDoc.SetAssetRevision(1, asset.Manifest);
            await revisionDoc.SaveAsync(cancellationToken);
        }

        // asset 생성
        var assetDoc = await _dbContext.GetDocAsync<AssetDoc>(Uuid.FromBase62(asset.Assetid!), false, cancellationToken);
        if (assetDoc.Exists)
        {
            assetDoc.SetWorksheetAsset(asset);
            await assetDoc.SaveAsync(cancellationToken);
        }
        else
        {
            assetDoc = AssetDoc.Create(Uuid.FromBase62(asset.Assetid!));
            _dbContext.Add(assetDoc);
            assetDoc.SetWorksheetAsset(asset);
            await assetDoc.SaveAsync(cancellationToken);

            resourceDoc.UseReference();
            _ = await resourceDoc.SaveAsync();
        }


        // market product 생성
        var marketDoc = await _dbContext.GetDocAsync<MarketProductDoc>(Uuid.FromBase62(asset.Assetid!), false, cancellationToken);
        if (marketDoc.Exists)
        {
            marketDoc.SetWorksheetMarket(asset);
            await marketDoc.SaveAsync(cancellationToken);
        }
        else
        {
            marketDoc = MarketProductDoc.Create(Uuid.FromBase62(asset.Assetid!));
            _dbContext.Add(marketDoc);
            marketDoc.SetWorksheetMarket(asset);
            await marketDoc.SaveAsync(cancellationToken);
        }

        var response = assetDoc.To<AssetResponse>();
        response.Resource ??= new AssetResourceResponse();
        response.Resource.Manifest = PublicUrlHelper.GetAssetManifestUri(response.Id, response.Option.Version);
        response.Resource.Thumbnail = response.Resource.Thumbnail != null || response.Resource.Thumbnail != "" ? PublicUrlHelper.GetResourceImageUri(response.Resource.Thumbnail!) : PublicUrlHelper.GetAssetThumbnailtUri(response.Id, response.Option.Version);
        return response;
    }
}
