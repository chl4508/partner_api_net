using Colorverse.Common.DataTypes;
using Colorverse.Common.Exceptions;
using Colorverse.Meta.Apis.DataTypes.Item;
using CvFramework.Common;
using CvFramework.MongoDB.Documents;
using CvFramework.MongoDB.Extensions;
using MongoDB.Bson;

namespace Colorverse.Meta.Documents;

/// <summary>
/// ItemDoc
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
    /// SetTemplateItem
    /// </summary>
    /// <param name="request"></param>
    /// <param name="isAdmin"></param>
    public void SetTemplateItem(CreateItemRequest request, bool isAdmin)
    {
        if (request.Option.Template != null && request.Option.Template != false)
        {
            if (isAdmin)
            {
                Set("option.template", request.Option.Template);
                if (request.Option.Templateid != null) Set("option.templateid", request.Option.Templateid);
                if (request.Option.TemplateVersion != null) Set("option.template_version", request.Option.TemplateVersion);
            }
            else
            {
                throw new ErrorInvalidParam(nameof(request.Option.Template), "Invalid Template parameter. User Not Create Template.");
            }
        }
        Set("option.template", false);
    }

    /// <summary>
    /// SetItem
    /// </summary>
    /// <param name="request"></param>
    /// <param name="profileid"></param>
    /// <param name="worldid"></param>
    /// <param name="userid"></param>
    public void SetItem(CreateItemRequest request, Uuid profileid, Uuid worldid, Uuid? userid)
    {
        Set("creatorid", profileid.ToBsonBinaryData());
        Set("profileid", profileid.ToBsonBinaryData());
        if (userid != null) { Set("userid", userid.ToBsonBinaryData()); }
        //Set("townid", userid.ToBsonBinaryData()); // 타운아이디 나중에 생기면 넣을것
        Set("worldid", worldid.ToBsonBinaryData());
        Set("asset_type", 1); // 아이템은 1
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
        if(request.Resource != null && request.Resource.Preview != null)
        {
            Set("resource.preview", request.Resource.Preview);
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
            Set("option.price", new BsonDocument { {"type", request.Option.Price.Type}, {"amount", request.Option.Price.Amount} });
        }
        if(noneSaleCnt > 0)
        {
            Set("option.none_sale", true);
        }
        else
        {
            Set("option.none_sale", false);
        }
    }

    /// <summary>
    /// ValidationCheck
    /// </summary>
    /// <param name="request"></param>
    /// <param name="itemDoc"></param>
    /// <param name="profileid"></param>
    /// <param name="isAdmin"></param>
    /// <exception cref="ErrorInvalidParam"></exception>
    public void ValidationCheck(UpdateItemRequest request, ItemDoc itemDoc, Uuid profileid, bool isAdmin)
    {

        if (!itemDoc.GetUuid("profileid").Equals(profileid)) // 수정 권한이있는지 체크
        {
            throw new ErrorBadRequest(nameof(request.Itemid) + " Not Matching Profileid");
        }
        
        if(itemDoc.GetInt32("option", "sale_status") != 0 && request.Resource != null && request.Resource.Preview != null)
        {
            throw new ErrorBadRequest("Unable to modify Preview : "+ itemDoc.GetInt32("option", "sale_status")); // preview는 판매프로세스를 요청하기전까지만 수정가능
        }

        if (itemDoc.GetInt32("option", "sale_status") >= 2 && itemDoc.GetNullableDateTime("option","edit_date") != null && ((request.Txt != null && request.Txt.Desc != null && request.Txt.Desc.Ko != null)
                || (request.Txt != null && request.Txt.Hashtag != null)
                || (request.Option != null && request.Option.Price != null))) 
        {
            var toDay = DateTime.UtcNow;
            var compareDay = itemDoc.GetDateTime("option", "edit_date").AddHours(CvMetaStaticTypes.EditDateTime); // 24시간 후에 수정가능
            if (compareDay > toDay)
            {
                throw new ErrorManyRequest("Update CoolTime. : " + itemDoc.GetDateTime("option", "edit_date"));
            }
        }

        // template admin role
        if (itemDoc.GetNullableBoolean("option", "template") != null && itemDoc.GetNullableBoolean("option", "template") == true && !isAdmin)
        {
            throw new ErrorBadRequest("User Not Update Template.");
        }

        if (itemDoc.GetInt32("option", "sale_status") == 1) // 판매요청시 수정불가
        {
            throw new ErrorBadRequest("SaleStatus is Judging "+ itemDoc.GetInt32("option", "sale_status"));
        }
        if (itemDoc.GetInt32("option", "sale_status") >= 2 && (request.Txt?.Title?.Ko != null || (request.Resource != null && request.Resource.Thumbnail != null)))  // 판매허용 또는 판매중일시 이름, 섬네일 수정불가
        {
            throw new ErrorBadRequest("SaleStatus is None or Judging " + itemDoc.GetInt32("option", "sale_status"));
        }

        if (request.Option != null && request.Option.Price != null)
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
    /// <param name="isAdmin"></param>
    /// <exception cref="ErrorInvalidParam"></exception>
    public void ValidationCheck(CreateItemRequest request, bool isAdmin)
    {
        var validCateArray = CvMetaStaticTypes.ValidCategory;

        foreach (var validcate in validCateArray)
        {
            if (validcate == request.Option.Category)
            {
                throw new ErrorInvalidParam(nameof(request.Option.Category), "Invalid Category Not Create User");
            }
        }

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
    /// <param name="itemDoc"></param>
    /// <param name="profileid"></param>
    /// <param name="isAdmin"></param>
    /// <exception cref="ErrorInvalidParam"></exception>
    public void ValidationCheck(SaleInspectionItemRequest request, ItemDoc itemDoc, Uuid profileid, bool isAdmin)
    {
        if(!itemDoc.GetUuid("profileid").Equals(profileid)) // 판매심사요청 권한체크
        {
            throw new ErrorBadRequest(nameof(request.Itemid)+" Not Matching Profileid");
        }
        if(itemDoc.GetInt32("option", "sale_status") >= 1) // 판매심사요청 그이상단계인지 확인
        {
            throw new ErrorBadRequest(nameof(request.Itemid) + "Aready Sale Inspection");
        }
        if (itemDoc.GetNullableBoolean("option", "template") == true) // 아이템 템플릿일경우 판매요청 불가 ( 확인필요 )
        {
            throw new ErrorBadRequest(nameof(request.Itemid) + "Item Template");
        }

        if (!isAdmin && request.Option.Price != null && request.Option.Price.Type != 4)
        {
            // 사용자는 오직 유료재화로만 아이템 생성할수있음
            throw new ErrorInvalidParam(nameof(request.Option.Price.Type), "User Only PriceType 4");
        }
    }

    /// <summary>
    /// ValidationCheck
    /// </summary>
    /// <param name="request"></param>
    /// <param name="itemDoc"></param>
    /// <param name="profileid"></param>
    /// <exception cref="ErrorInvalidParam"></exception>
    public void ValidationCheck(SaleStartItemRequest request, ItemDoc itemDoc, Uuid profileid)
    {
        if (!itemDoc.GetUuid("profileid").Equals(profileid)) // 판매요청 권한체크
        {
            throw new ErrorBadRequest(profileid + " Not Matching Profileid");
        }
        if(itemDoc.GetNullableBoolean("option", "template") == true) 
        {
            throw new ErrorBadRequest("Template Item");
        }
        if (itemDoc.GetInt32("option","sale_status") != 2)
        {
            throw new ErrorBadRequest("saleStatus Not matching id="+request.Itemid);
        }
    }

    /// <summary>
    /// ValidationCheck
    /// </summary>
    /// <param name="request"></param>
    /// <param name="itemDoc"></param>
    /// <param name="profileid"></param>
    /// <exception cref="ErrorBadRequest"></exception>
    public void ValidationCheck(SaleStopItemRequest request, ItemDoc itemDoc, Uuid profileid)
    {
        if (!itemDoc.GetUuid("profileid").Equals(profileid)) // 판매요청 권한체크
        {
            throw new ErrorBadRequest(profileid + " Not Matching Profileid");
        }
        if (itemDoc.GetNullableBoolean("option", "template") == true)
        {
            throw new ErrorBadRequest("Template Item");
        }
        if (itemDoc.GetInt32("option", "sale_status") != 3)
        {
            throw new ErrorBadRequest("saleStatus Not matching id=" + request.Itemid);
        }
    }

    /// <summary>
    /// ValidationCheck
    /// </summary>
    /// <param name="request"></param>
    /// <param name="itemDoc"></param>
    /// <param name="profileid"></param>
    /// <param name="isAdmin"></param>
    /// <exception cref="ErrorBadRequest"></exception>
    public void ValidationCheck(CreateItemTemplateRequest request, ItemDoc itemDoc, Uuid profileid, bool isAdmin)
    {
        if (!isAdmin)
        {
            throw new ErrorBadRequest("User Not Create Template.");
        }
        if (!itemDoc.GetUuid("profileid").Equals(profileid)) // 판매요청 권한체크
        {
            throw new ErrorBadRequest(profileid + " Not Matching Profileid");
        }
        if (itemDoc.GetNullableBoolean("option", "template") == false)
        {
            throw new ErrorBadRequest("Not Template Item id="+request.Itemid);
        }
    }

    /// <summary>
    /// UpdateItem
    /// </summary>
    /// <param name="request"></param>
    public void UpdateItem(UpdateItemRequest request)
    {
        if(request.Txt != null)
        {
            if (request.Txt.Title != null && request.Txt.Title.Ko != null)
            {
                Set("txt.title.ko", request.Txt.Title.Ko);
            }
            if(request.Txt.Desc != null && request.Txt.Desc.Ko != null)
            {
                Set("txt.desc.ko", request.Txt.Desc.Ko);
            }
            if(request.Txt.Hashtag != null)
            {
                Set("txt.hashtag", new BsonArray(request.Txt.Hashtag));
            }
        }
        Set("option.edit_date", DateTime.UtcNow);
        if (request.Option != null)
        {
            Set("option.version", request.Option.Version);
        }
        if (request.Resource != null)
        {
            if(request.Resource.Thumbnail != null) Set("resource.thumbnail", request.Resource.Thumbnail);
            if(request.Resource.Preview != null) Set("resource.preview", request.Resource.Preview);
        }
        
        var noneSaleCnt = 0;
        if (request.Option != null && request.Option.Price != null)
        {
            if (request.Option.Price.Type == 1) noneSaleCnt++;
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
    /// SaleInspectionItemParam
    /// </summary>
    /// <param name="request"></param>
    public void SaleInspectionItem(SaleInspectionItemRequest request)
    {
        Set("option.sale_status", 1); // 판매상태(0 : 없음, 1 : 판매심사중, 2: 판매허용, 3 : 판매중)
        Set("option.sale_review_status", 1); // 판매심사상태 (0 : 없음, 1 : 판매심사요청, 2 : 판매심사중, 3 : 판매심사반려, 4 : 판매심사승인)
        Set("option.instant_sale", request.Option.InstantSale);
        Set("option.price", new BsonDocument { { "type", request.Option.Price.Type }, { "amount", request.Option.Price.Amount } });
    }

    /// <summary>
    /// 임시사용 백오피스완료되면 제거
    /// </summary>
    /// <param name="request"></param>
    public void SaleInspectionItem2(SaleInspectionItemRequest request)
    {
        Set("option.sale_review_status", 4); // 판매심사상태 (0 : 없음, 1 : 판매심사요청, 2 : 판매심사중, 3 : 판매심사반려, 4 : 판매심사승인)
        Unset("option.edit_date"); // 수정일 초기화
        Set("option.instant_sale", request.Option.InstantSale);
        if (request.Option.InstantSale)
        {
            Set("option.sale_status", 3); // 판매상태(0 : 없음, 1 : 판매심사중, 2: 판매허용, 3 : 판매중)
        }
        else
        {
            Set("option.sale_status", 2); // 판매상태(0 : 없음, 1 : 판매심사중, 2: 판매허용, 3 : 판매중)
        }
        Set("option.price", new BsonDocument { { "type", request.Option.Price.Type }, { "amount", request.Option.Price.Amount } });
    }


    /// <summary>
    /// SaleStartItemParam
    /// </summary>
    public void SaleStartItem()
    {
        Set("option.sale_status", 3); // 판매상태(0 : 없음, 1 : 판매심사중, 2: 판매허용, 3 : 판매중)
    }

    /// <summary>
    /// SaleCancelItem
    /// </summary>
    public void SaleCancelItem()
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
