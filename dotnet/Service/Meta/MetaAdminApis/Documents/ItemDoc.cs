using Colorverse.MetaAdmin.Apis.DataTypes.Item;
using Colorverse.MetaAdmin.DataTypes.Excel;
using Colorverse.MetaAdmin.DataTypes.Item;
using Colorverse.MetaAdmin.Excel;
using CvFramework.Common;
using CvFramework.MongoDB.Documents;
using CvFramework.MongoDB.Extensions;
using MongoDB.Bson;

namespace Colorverse.MetaAdmin.Documents;

/// <summary>
/// 
/// </summary>
public class ItemDoc : MongoDoc
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="itemid"></param>
    /// <returns></returns>
    public static ItemDoc Create(Uuid itemid) => new(itemid);

    /// <summary>
    /// 
    /// </summary>
    public ItemDoc() : base() { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="doc"></param>
    public ItemDoc(BsonDocument doc) : base(doc) { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="uuid"></param>
    public ItemDoc(Uuid uuid) : base(uuid) { }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override string GetDatabaseName() => "meta";

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override string GetCollectionName() => "item";

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override string? GetCacheKey() => string.Join(':', GetDatabaseName(), GetCollectionName(), Id);

    /// <summary>
    /// 
    /// </summary>
    public override void OnCreate()
    {
        CurrentDate("stat.created");
        CurrentDate("stat.updated");
    }

    /// <summary>
    /// 
    /// </summary>
    public override void OnUpdate()
    {
        CurrentDate("stat.updated");
    }

    /// <summary>
    /// 아이템 수정
    /// </summary>
    /// <param name="manifest"></param>
    /// <param name="request"></param>
    public void UpdateItem(UpdateItemRequest request, BsonDocument? manifest)
    {
        if (request.Txt != null)
        {
            if (request.Txt.Title != null && request.Txt.Title.Ko != null)
            {
                Set("txt.title.ko", request.Txt.Title.Ko);
            }
            if (request.Txt.Desc != null && request.Txt.Desc.Ko != null)
            {
                Set("txt.desc.ko", request.Txt.Desc.Ko);
            }
            if (request.Txt.Hashtag != null)
            {
                Set("txt.hashtag", new BsonArray(request.Txt.Hashtag));
            }
        }
        if (manifest != null)
        {
            Set("manifest", manifest);
        }
        if (request.Option != null)
        {
            if (request.Option.Version > 0)
            {
                Set("option.version", request.Option.Version);
            }
            if (request.Option.Price != null)
            {
                if (request.Option.Price.Type != null)
                {
                    Set("option.price.type", request.Option.Price.Type);
                }
                if (request.Option.Price.Amount != null)
                {
                    Set("option.price.amount", request.Option.Price.Amount);
                }
            }
        }
        if (request.Resource != null)
        {
            if (request.Resource.Thumbnail != null)
            {
                Set("option.resoruce.thumbnail", request.Resource.Thumbnail);
            }
            if (request.Resource.Preview != null)
            {
                Set("option.resoruce.preview", request.Resource.Preview);
            }
        }
    }

    /// <summary>
    /// 사후심사 승인
    /// </summary>
    public void SaleApprovalItem()
    {
        Unset("option.edit_date"); // 수정일 초기화
        Set("option.blind", false);
        Set("option.judge_status", 4); // 사후심사 승인
    }

    /// <summary>
    /// 심사 승인
    /// </summary>
    /// <param name="request"></param>
    /// <param name="saleAccept"></param>
    /// <param name="saleStatus"></param>
    /// <param name="version"></param>
    /// <param name="itemRevision"></param>
    public void SaleApprovalItem(SaleApprovalItemRequest request, DateTime saleAccept, int saleStatus, int version, ItemRevisionResult? itemRevision)
    {
        Unset("option.edit_date"); // 수정일 초기화
        Set("option.sale_accept", saleAccept);
        Set("option.blind", false);
        Set("option.sale_status", saleStatus);
        if (request.SaleStart != null)
        {
            Set("option.sale_start", request.SaleStart);
        }
        if (request.SaleEnd != null)
        {
            Set("option.sale_end", request.SaleEnd);
        }
        Set("option.sale_review_status", 4);
        Set("option.sale_version", version);
        if (itemRevision != null && itemRevision.Manifest != null)
        {
            Set("manifest", itemRevision.Manifest);
        }
    }

    /// <summary>
    /// 사후심사 반려
    /// </summary>
    public void SaleRejectionItem()
    {
        Set("option.blind", true);
        Set("option.sale_status", 0); // 0 으로 초기화
        Set("option.judge_status", 3);
    }

    /// <summary>
    /// 심사 반려
    /// </summary>
    /// <param name="request"></param>
    public void SaleRejectionItem(SaleRejectionItemRequest request)
    {
        Set("option.blind", true);
        Set("option.sale_status", 0); // 0 으로 초기화
        Set("option.sale_review_status", 3);
    }

    /// <summary>
    /// 아이템 삭제요청
    /// </summary>
    /// <param name="request"></param>
    public void DeleteItem(DeleteItemRequest request)
    {
        Set("option.blind", true);
        Set("option.delete", true);
    }

    /// <summary>
    /// 아이템 삭제요청 취소
    /// </summary>
    public void DeleteCancelItem()
    {
        Set("option.blind", false);
        Set("option.delete", false);
    }

    /// <summary>
    /// 워크 시트 아이템 생성
    /// </summary>
    /// <param name="param"></param>
    public void SetWorksheetItem(CreateExcelRequest param)
    {
        Set("creatorid", param.Profileid.ToBsonBinaryData());
        Set("profileid", param.Profileid.ToBsonBinaryData());
        Set("userid", param.Userid.ToBsonBinaryData());
        Set("worldid", param.Worldid.ToBsonBinaryData());
        if (param.Manifest != null)
        {
            Set("manifest", param.Manifest);
        }
        Set("option.template", false);
        Set("asset_type", 1);
        Set("txt.title.ko", param.Title);
        Set("txt.desc.ko", param.Desc);
        Set("txt.hashtag", new BsonArray(param.Hashtag));

        CurrentDate("option.sale_accept");


        Set("option.version", param.Version);
        Set("option.sale_version", param.Version);
        Set("option.instant_sale", true);
        Set("option.ready_status", 1); // 준비상태 (0 : 제작중, 1 : 저장됨)
        Set("option.sale_review_status", 4); // 판매심사상태 (0 : 없음, 1 : 판매심사요청, 2 : 판매심사중, 3 : 판매심사반려, 4 : 판매심사승인)
        Set("option.judge_status", 0); // 사후심사상태(0 : 없음, 1: 사후심사요청, 2 : 사후심사중, 3: 사후심사반려, 4 : 사후심사승인)
        Set("option.sale_status", 3); //아이템 판매상태( 0 : none , 1 : 판매심사중, 2: 판매허용 , 3 : 판매중
        Set("option.blind", false); //블라인드
        Set("option.delete", false); //delete
        if(param.ClientItemid != null && param.ClientItemid != "")
        {
            Set("option.client_itemid", param.ClientItemid);
        }
        if (param.PriceType == 1)
        {
            Set("option.none_sale", true); //비매품여부
        }
        else
        {
            Set("option.none_sale", false); //비매품여부
        }

        Set("option.category", new BsonArray(param.Category));
        Set("option.price", new BsonDocument() { { "type", param.PriceType }, { "amount", param.PriceAmount } });
    }

    /// <summary>
    /// 워크 시트 아이템 생성
    /// </summary>
    /// <param name="param"></param>
    public void SetWorksheetItem(ItemRow param)
    {
        Set("creatorid", Uuid.FromBase62(param.Profileid).ToBsonBinaryData());
        Set("profileid", Uuid.FromBase62(param.Profileid).ToBsonBinaryData());
        Set("userid", Uuid.FromBase62(param.Userid).ToBsonBinaryData());
        Set("worldid", Uuid.FromBase62(param.Worldid).ToBsonBinaryData());
        Set("asset_type", 1);
        if (param.Manifest != null)
        {
            Set("manifest", param.Manifest);
        }
        Set("option.template", false);
        Set("txt.title.ko", param.Title);
        if(param.Desc != null)
        {
            Set("txt.desc.ko", param.Desc);
        }
        if (param.Hashtag != null)
        {
            var hashTagArray = param.Hashtag.Split(",");
            Set("txt.hashtag", new BsonArray(hashTagArray));
        }

        CurrentDate("option.sale_accept");
        Set("option.instant_sale", true);

        Set("option.version", param.Version);
        Set("option.sale_version", param.Version);

        Set("option.ready_status", 1); // 준비상태 (0 : 제작중, 1 : 저장됨)
        Set("option.sale_review_status", 4); // 판매심사상태 (0 : 없음, 1 : 판매심사요청, 2 : 판매심사중, 3 : 판매심사반려, 4 : 판매심사승인)
        Set("option.judge_status", 0); // 사후심사상태(0 : 없음, 1: 사후심사요청, 2 : 사후심사중, 3: 사후심사반려, 4 : 사후심사승인)
        Set("option.sale_status", 3); //아이템 판매상태( 0 : none , 1 : 판매심사중, 2: 판매허용 , 3 : 판매중
        Set("option.blind", false); //블라인드
        Set("option.delete", false); //delete
        if (param.ClientItemid != null && param.ClientItemid != "")
        {
            Set("option.client_itemid", param.ClientItemid);
        }
        if (param.PriceType!.Equals("1"))
        {
            Set("option.none_sale", true); //비매품여부
        }
        else
        {
            Set("option.none_sale", false); //비매품여부
        }
        string[] categoryArray = new string[3] { param.Category1!.ToString(),
                                                 param.Category1!.ToString() + "," + param.Category2!.ToString(),
                                                 param.Category1!.ToString() + "," + param.Category2!.ToString() + "," + param.Category3!.ToString() };
        Set("option.category", new BsonArray(categoryArray));
        Set("option.price", new BsonDocument() { { "type", Convert.ToInt32(param.PriceType) }, { "amount", Convert.ToInt32(param.PriceAmount) } });
    }

    /// <summary>
    /// 워크시트 아이템 판매허용 변경
    /// </summary>
    public void DeleteWorksheetItem()
    {
        Set("option.sale_status", 2); // 판매허용으로 변경
    }
}
