using Colorverse.MetaAdmin.Apis.DataTypes.Asset;
using Colorverse.MetaAdmin.DataTypes.Asset;
using Colorverse.MetaAdmin.DataTypes.Excel;
using Colorverse.MetaAdmin.Excel;
using CvFramework.Common;
using CvFramework.MongoDB.Documents;
using CvFramework.MongoDB.Extensions;
using MongoDB.Bson;

namespace Colorverse.MetaAdmin.Documents;

public class AssetDoc : MongoDoc
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="assetid"></param>
    /// <returns></returns>
    public static AssetDoc Create(Uuid assetid) => new(assetid);

    /// <summary>
    /// 
    /// </summary>
    public AssetDoc() : base() { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="doc"></param>
    public AssetDoc(BsonDocument doc) : base(doc) { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="uuid"></param>
    public AssetDoc(Uuid uuid) : base(uuid) { }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override string GetDatabaseName() => "meta";

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override string GetCollectionName() => "asset";

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
    /// 애셋 수정
    /// </summary>
    /// <param name="request"></param>
    /// <param name="manifest"></param>
    public void UpdateAsset(UpdateAssetRequest request, BsonDocument? manifest)
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
        if (request.Resource != null && request.Resource.Thumbnail != null)
        {
            Set("option.resoruce.thumbnail", request.Resource.Thumbnail);
        }
    }

    /// <summary>
    /// 사후심사 승인
    /// </summary>
    public void SaleApprovalAsset()
    {
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
    /// <param name="assetRevision"></param>
    public void SaleApprovalAsset(SaleApprovalAssetRequest request, DateTime saleAccept, int saleStatus, int version, AssetRevisionResult? assetRevision)
    {
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
        if (assetRevision != null && assetRevision.Manifest != null)
        {
            Set("manifest", assetRevision.Manifest);
        }
    }

    /// <summary>
    /// 심사 반려
    /// </summary>
    public void SaleRejectionAsset()
    {
        Set("option.delete", true);
        Set("option.blind", true);
        Set("option.sale_status", 0); // 0 으로 초기화
        Set("option.judge_status", 3);
    }

    /// <summary>
    /// 애셋 삭제요청
    /// </summary>
    public void DeleteAsset()
    {
        Set("option.blind", true);
        Set("option.delete", true);
    }

    /// <summary>
    /// 애셋 삭제요청 취소
    /// </summary>
    public void DeleteCancelAsset()
    {
        Set("option.blind", false);
        Set("option.delete", false);
    }

    /// <summary>
    /// 워크시트 애셋 생성
    /// </summary>
    /// <param name="param"></param>
    public void SetWorksheetAsset(CreateExcelRequest param)
    {
        Set("creatorid", param.Profileid.ToBsonBinaryData());
        Set("profileid", param.Profileid.ToBsonBinaryData());
        Set("userid", param.Userid.ToBsonBinaryData());
        Set("worldid", param.Worldid.ToBsonBinaryData());
        if (param.Manifest != null)
        {
            Set("manifest", param.Manifest);
        }
        Set("type", param.AssetType);
        Set("txt.title.ko", param.Title);
        Set("txt.desc.ko", param.Desc);
        Set("txt.hashtag", new BsonArray(param.Hashtag));

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
    /// 워크시트 애셋 생성
    /// </summary>
    /// <param name="param"></param>
    public void SetWorksheetAsset(AssetRow param)
    {
        Set("creatorid", Uuid.FromBase62(param.Profileid).ToBsonBinaryData());
        Set("profileid", Uuid.FromBase62(param.Profileid).ToBsonBinaryData());
        Set("userid", Uuid.FromBase62(param.Userid).ToBsonBinaryData());
        Set("worldid", Uuid.FromBase62(param.Worldid).ToBsonBinaryData());
        if (param.Manifest != null)
        {
            Set("manifest", param.Manifest);
        }
        Set("type", Convert.ToInt32(param.AssetType));
        Set("txt.title.ko", param.Title);
        if (param.Desc != null)
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
    public void DeleteWorksheetAsset()
    {
        Set("option.sale_status", 2); // 판매허용으로 변경
    }
}
