using Colorverse.Common.Exceptions;
using Colorverse.Meta.Apis.DataTypes.Asset;
using CvFramework.Common;
using CvFramework.MongoDB.Documents;
using CvFramework.MongoDB.Extensions;
using MongoDB.Bson;

namespace Colorverse.Meta.Documents;

/// <summary>
/// AssetDoc
/// </summary>
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
    /// SetItem
    /// </summary>
    /// <param name="request"></param>
    /// <param name="profileid"></param>
    /// <param name="worldid"></param>
    /// <param name="userid"></param>
    public void SetAsset(CreateAssetRequest request, Uuid profileid, Uuid worldid, Uuid userid)
    {
        Set("creatorid", profileid.ToBsonBinaryData());
        Set("profileid", profileid.ToBsonBinaryData());
        if (userid != null) { Set("userid", userid.ToBsonBinaryData()); }
        //Set("townid", userid.ToBsonBinaryData()); // 타운아이디 나중에 생기면 넣을것
        Set("worldid", worldid.ToBsonBinaryData());
        Set("type", request.Type);
        Set("txt.title.ko", request.Txt.Title.Ko);
        if (request.Txt.Desc != null && request.Txt.Desc.Ko != null)
        {
            Set("txt.desc.ko", request.Txt.Desc.Ko);
        }
        if (request.Txt.Hashtag != null)
        {
            Set("txt.hashtag", new BsonArray(request.Txt.Hashtag));
        }
        if (request.Resource != null && request.Resource.Thumbnail != null)
        {
            Set("resource.thumbnail", request.Resource.Thumbnail);
        }

        Set("option.version", request.Option.Version);
        Set("option.sale_version", 0);
        Set("option.ready_status", 1); // 준비상태 (0 : 제작중, 1 : 저장됨)
        Set("option.sale_status", 0); // 판매상태 (0 : 없음, 1 : 판매심사중, 2: 판매허용, 3 : 판매중)
        Set("option.sale_review_status", 0); // 판매심사상태 (0 : 없음, 1 : 판매심사요청, 2 : 판매심사중, 3 : 판매심사반려, 4 : 판매심사승인)
        Set("option.judge_status", 0); // 사후심사상태(0 : 없음, 1: 사후심사요청, 2 : 사후심사중, 3: 사후심사반려, 4 : 사후심사승인)
        Set("option.blind", false);
        Set("option.delete", false);

        if (request.Option.Category != null)
        {
            var cateArray = request.Option.Category.Split(",");
            var cateSearch = new BsonArray
            {
                cateArray[0].ToString(),
                cateArray[0].ToString() + "," + cateArray[1].ToString(),
                cateArray[0].ToString() + "," + cateArray[1].ToString() + "," + cateArray[2].ToString()
            };
            Set("option.category", cateSearch);
        }
        var noneSaleCnt = 0;
        if (request.Option.Price != null)
        {
            Set("option.price", new BsonDocument { { "type", request.Option.Price.Type }, { "amount", request.Option.Price.Amount } });
        }
        if (noneSaleCnt > 0)
        {
            Set("option.none_sale", true);
        }
        else
        {
            Set("option.none_sale", false);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="assetDoc"></param>
    /// <param name="profileid"></param>
    /// <exception cref="ErrorBadRequest"></exception>
    public void ValidationCheck(SaleStartAssetRequest request, AssetDoc assetDoc, Uuid profileid)
    {
        if (!assetDoc.GetUuid("profileid").Equals(profileid)) // 판매요청 권한체크
        {
            throw new ErrorBadRequest(request.Assetid + " Not Matching Profileid");
        }
        if (assetDoc.GetUInt32("option", "sale_status") != 2)
        {
            throw new ErrorBadRequest("saleStatus Not matching id=" + request.Assetid);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="assetDoc"></param>
    /// <param name="profileid"></param>
    /// <exception cref="ErrorBadRequest"></exception>
    public void ValidationCheck(SaleStopAssetRequest request, AssetDoc assetDoc, Uuid profileid)
    {
        if (!assetDoc.GetUuid("profileid").Equals(profileid)) // 판매요청 권한체크
        {
            throw new ErrorBadRequest(request.Assetid + " Not Matching Profileid");
        }
        if (assetDoc.GetUInt32("option", "sale_status") != 2)
        {
            throw new ErrorBadRequest("saleStatus Not matching id="+request.Assetid);
        }
    }

    /// <summary>
    /// ValidationCheck
    /// </summary>
    /// <param name="request"></param>
    /// <param name="isAdmin"></param>
    /// <exception cref="ErrorInvalidParam"></exception>
    public void ValidationCheck(CreateAssetRequest request, bool isAdmin)
    {
        if (request.Option.Price != null)
        {
            if (isAdmin)
            {
                return;
            }
            if (request.Option.Price != null && request.Option.Price.Type != 4)
            {
                // 사용자는 오직 유료재화로만 아이템 생성할수있음
                throw new ErrorInvalidParam(nameof(request.Option.Price.Type), "User Only PriceType 4");
            }
        }
    }

    /// <summary>
    /// ValidationCheck
    /// </summary>
    /// <param name="request"></param>
    /// <param name="assetDoc"></param>
    /// <param name="profileid"></param>
    /// <param name="isAdmin"></param>
    /// <exception cref="ErrorBadRequest"></exception>
    /// <exception cref="ErrorInvalidParam"></exception>
    public void ValidationCheck(SaleInspectionAssetRequest request, AssetDoc assetDoc, Uuid profileid, bool isAdmin)
    {
        if (!assetDoc.GetUuid("profileid").Equals(profileid)) // 판매심사요청 권한체크
        {
            throw new ErrorBadRequest(nameof(request.Assetid) + " Not Matching Profileid");
        }
        if (assetDoc.GetUInt32("option", "sale_status") >= 1) // 판매심사요청 그이상단계인지 확인
        {
            throw new ErrorBadRequest(nameof(request.Assetid) + "Aready Sale Inspection");
        }

        if (!isAdmin && request.Option.Price != null && request.Option.Price.Type != 4)
        {
            // 사용자는 오직 유료재화로만 아이템 생성할수있음
            throw new ErrorInvalidParam(nameof(request.Option.Price.Type), "User Only PriceType 4");
        }
    }

    /// <summary>
    /// SaleAsset
    /// </summary>
    /// <param name="request"></param>
    public void SaleInspectionAsset(SaleInspectionAssetRequest request)
    {
        Set("option.sale_status", 1); // 판매상태(0 : 없음, 1 : 판매심사중, 2: 판매허용, 3 : 판매중)
        Set("option.sale_review_status", 1); // 판매심사상태 (0 : 없음, 1 : 판매심사요청, 2 : 판매심사중, 3 : 판매심사반려, 4 : 판매심사승인)
        Set("optin.instant_sale", request.Option.InstantSale);
        Set("option.price", new BsonDocument { {"type", request.Option!.Price.Type}, {"amount", request.Option!.Price.Amount} });
    }

    /// <summary>
    /// 임시사용 백오피스 완료되면 제거
    /// </summary>
    /// <param name="request"></param>
    public void SaleInspectionAsset2(SaleInspectionAssetRequest request)
    {
        Set("option.sale_review_status", 4); // 판매심사상태 (0 : 없음, 1 : 판매심사요청, 2 : 판매심사중, 3 : 판매심사반려, 4 : 판매심사승인)
        Set("optin.instant_sale", request.Option.InstantSale);
        if (request.Option.InstantSale)
        {
            Set("option.sale_status", 3); // 판매상태(0 : 없음, 1 : 판매심사중, 2: 판매허용, 3 : 판매중)
        }
        else
        {
            Set("option.sale_status", 2); // 판매상태(0 : 없음, 1 : 판매심사중, 2: 판매허용, 3 : 판매중)
        }
        Set("option.price", new BsonDocument { { "type", request.Option!.Price.Type }, { "amount", request.Option!.Price.Amount } });
    }

    /// <summary>
    /// SaleStartAsset
    /// </summary>
    public void SaleStartAsset()
    {
        Set("option.sale_status", 3); // 판매상태(0 : 없음, 1 : 판매심사중, 2: 판매허용, 3 : 판매중)
    }

    /// <summary>
    /// SaleCancelAsset
    /// </summary>
    public void SaleCancelAsset()
    {
        Set("option.sale_status", 2); // 판매상태(0 : 없음, 1 : 판매심사중, 2: 판매허용, 3 : 판매중)
    }

    /// <summary>
    /// 
    /// </summary>
    public void UpdateLiked()
    {
        Inc("stat.like", 1);
    }
    
    /// <summary>
    /// 
    /// </summary>
    public void UpdateCommented()
    {
        Inc("stat.comment", 1);
    }
}
