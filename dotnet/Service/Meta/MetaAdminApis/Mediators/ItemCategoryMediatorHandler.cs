using Colorverse.Apis.Controller.Models;
using Colorverse.Application.Mediator;
using Colorverse.Common.Exceptions;
using Colorverse.Database;
using Colorverse.MetaAdmin.Apis.DataTypes.ItemCategory;
using Colorverse.MetaAdmin.Documents;
using Colorverse.MetaAdmin.Excel;
using CvFramework.Excel;
using CvFramework.GoogleDrive;
using CvFramework.MongoDB.Documents;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Colorverse.MetaAdmin.Mediators;

/// <summary>
/// 
/// </summary>
public class ItemCategoryMediatorHandler :
    IMediatorHandler<GetItemCategoryListRequest, ListResponse<ItemCategoryResponse>>,
    IMediatorHandler<SyncItemCategory, ListResponse<ItemCategoryResponse>>
{
    private readonly IConfiguration _configuration;
    private readonly IGoogleDrive _googleDrive;
    private readonly IDbContext _dbContext;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="googleDrive"></param>
    /// <param name="dbContext"></param>
    public ItemCategoryMediatorHandler(IConfiguration configuration, IGoogleDrive googleDrive, IDbContext dbContext)
    {
        _configuration = configuration;
        _googleDrive = googleDrive;
        _dbContext = dbContext;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<ListResponse<ItemCategoryResponse>> Handle(GetItemCategoryListRequest request, CancellationToken cancellationToken)
    {
        var db = _dbContext.GetMongoDb();

        var filter = new BsonDocument();
        if(request.Depth > 0)
        {
            filter.Add("option.depth", request.Depth);
        }
        if(!string.IsNullOrWhiteSpace(request.ParentId))
        {
            filter.Add("_srch.parentid", request.ParentId);
        }

        var collection = _dbContext.GetCollection<ItemCategoryResponse>(ItemCategoryDoc.COLLECTION_NAME);
        var result = await collection.Find(filter).ToListAsync(cancellationToken);
        
        var response = new ListResponse<ItemCategoryResponse>(result);
        return response;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<ListResponse<ItemCategoryResponse>> Handle(SyncItemCategory request, CancellationToken cancellationToken)
    {
        var sheetId = request.WorksheetId;
        var driveId = request.DriveId;
        if(string.IsNullOrWhiteSpace(sheetId))
        {
            sheetId = _configuration.GetValue<string>("meta:google_sheets_category:path");
        }
        if(string.IsNullOrWhiteSpace(sheetId))
        {
            throw new ErrorBadRequest(nameof(request.WorksheetId));
        }

        using var fileStream = await _googleDrive.GetFileStreamAsync(sheetId, driveId);

        var excelFile = new ExcelFile(fileStream);
        var document = await excelFile.DeserializeAsync<ItemCategoryExcel>();
        if(document == null)
        {
            throw new ErrorInternal("excel file deserializeAsync fail.");
        }
        
        var totalCount = document.Category1.Count() + document.Category2.Count() + document.Category3.Count();
        var docList = new List<MongoDocBase>(totalCount);

        foreach(var category1 in document.Category1.Values)
        {
            if(string.IsNullOrWhiteSpace(category1.Name))
            {
                continue;
            }

            var filter = ItemCategoryDoc.CreateDefaultFilter(category1.Id);
            var doc = await _dbContext.GetDocAsync<ItemCategoryDoc>(filter, false);
            doc.SetData(category1);
           
            docList.Add(doc);
        }

        var category1NameMap = document.Category1.Where(x=>!string.IsNullOrWhiteSpace(x.Value.Name))
            .ToDictionary(x=>x.Value.Name, x=>x.Value);

        var category2NameMap = document.Category2.Where(x=>!string.IsNullOrWhiteSpace(x.Value.Name))
            .ToDictionary(x=>x.Value.Name, x=>x.Value);


        foreach(var category2 in document.Category2.Values)
        {
            if(string.IsNullOrWhiteSpace(category2.Name)
               || string.IsNullOrWhiteSpace(category2.Parent))
            {
                continue;
            }

            _ = category1NameMap.TryGetValue(category2.Parent, out var category1) ? true : throw new ErrorInvalidParam("category1");

            var filter = ItemCategoryDoc.CreateDefaultFilter(category1.Id, category2.Id);
            var doc = await _dbContext.GetDocAsync<ItemCategoryDoc>(filter, false);
            doc.SetData(category2, new string[]{category1.Id});
            
            docList.Add(doc);
        }

        foreach(var category3 in document.Category3.Values)
        {
            if(string.IsNullOrWhiteSpace(category3.Name) 
               || string.IsNullOrWhiteSpace(category3.Parent))
            {
                continue;
            }

            _ = category2NameMap.TryGetValue(category3.Parent, out var category2) ? true : throw new ErrorInvalidParam("category2");
            _ = category1NameMap.TryGetValue(category2.Parent, out var category1) ? true : throw new ErrorInvalidParam("category1");

            var originIds = new int[]{};
            var filter = ItemCategoryDoc.CreateDefaultFilter(category1.Id, category2.Id, category3.Id);
            var doc = await _dbContext.GetDocAsync<ItemCategoryDoc>(filter, false);
            doc.SetData(category3, new string[]{category1.Id, category2.Id});
          
            docList.Add(doc);
        }

        foreach(var doc in docList)
        {
            await doc.SaveAsync();
        }

        var list = docList.Select(x=>x.To<ItemCategoryResponse>()).ToList();
        var result = new ListResponse<ItemCategoryResponse>(list);
        return result;
    }
}