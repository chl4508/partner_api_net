using Colorverse.Common;
using Colorverse.Meta.Apis.DataTypes.Cart;
using Colorverse.Meta.Apis.DataTypes.Market;
using CvFramework.Common;
using CvFramework.MongoDB.Documents;
using CvFramework.MongoDB.Extensions;
using MongoDB.Bson;

namespace Colorverse.Meta.Documents;

/// <summary>
/// CartDoc
/// </summary>
public class CartDoc : MongoDoc
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="cartid"></param>
    /// <returns></returns>
    public static CartDoc Create(Uuid cartid) => new(cartid);

    /// <summary>
    /// 
    /// </summary>
    public CartDoc() : base() { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="doc"></param>
    public CartDoc(BsonDocument doc) : base(doc) { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="uuid"></param>
    public CartDoc(Uuid uuid) : base(uuid) { }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override string GetDatabaseName() => "meta";

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override string GetCollectionName() => "cart";

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
    /// SetCart
    /// </summary>
    /// <param name="items"></param>
    /// <param name="worldid"></param>
    public void SetCart(CreateCartItemsRequest[] items, Uuid worldid)
    {
        Set("worldid", new BsonBinaryData(worldid, GuidRepresentation.Standard));
        
        var array = new BsonArray();
        foreach (var item in items) {
            var cateSplitArray = item.Category.Split(",");
            var cateArray = new BsonArray {
                        cateSplitArray[0],
                        cateSplitArray[0]+","+cateSplitArray[1],
                        cateSplitArray[0] + "," + cateSplitArray[1]+","+cateSplitArray[2]
            };
            var domainType = (SvcDomainType)Uuid.FromBase62(item.Productid).DomainType;
            var type = 1;
            if (domainType.ToString().Equals("MetaAsset"))
            {
                type = 2;
            }
            var bson = new BsonDocument {
                {"productid", Uuid.FromBase62(item.Productid).ToBsonBinaryData()},  // 마켓아이디 (구매해야될 아이디 (아이템아이디, 애셋아이디))
                {"quantity", 1}, // 수량
                {"type", type}, // 아이템 1 , 애셋 2
                {"price_type", item.PriceType}, // 판매재화타입 ( 1 : 비매품 , 2: 무료(0) , 3: gold , 4 : 컬러벅스 )
                {"price_amount", item.PriceAmount}, // 판매 가격
                {"category", cateArray} // 카테고리
                
            };
            array.Add(bson);
        }
        Set("items", array);
        
    }

    /// <summary>
    /// UpdateCart
    /// </summary>
    /// <param name="items"></param>
    /// <param name="cart"></param>
    public void UpdateCart(CreateCartItemsRequest[] items, CartResponse cart)
    {
        if (cart.Items != null && cart.Items.Length != 0)
        {
            List<CartItemsResponse> list = new();
            for (int i = 0; i < cart.Items.Length; i++)
            {
                list.Add(cart.Items[i]);
                foreach (var item in items)
                {
                    if (cart.Items[i].Productid == Uuid.FromBase62(item.Productid))
                    {
                        list.RemoveAt(i); // 기존 카트목록에서 중복된 값 제거
                    }
                }
            }

            var array = new BsonArray();
            var delArray = list.ToArray();
            // 제거된 기존값 추가
            for (int i = 0; i < delArray.Length; i++)
            {
                var cateArray = new BsonArray();
                foreach (var cate in delArray[i].Category)
                {
                    cateArray.Add(cate);
                }
                var domainType = (SvcDomainType)delArray[i].Productid!.DomainType;
                var type = 1;
                if (domainType.ToString().Equals("MetaAsset"))
                {
                    type = 2;
                }
                var bson = new BsonDocument {
                    {"productid", delArray[i].Productid!.ToBsonBinaryData()},  // 마켓아이디 (구매해야될 아이디 (아이템아이디, 애셋아이디))
                    {"quantity", 1}, // 수량
                    {"type", type}, // 아이템 1 , 애셋 2
                    {"price_type", delArray[i].PriceType}, // 판매재화타입 ( 1 : 비매품 , 2: 무료(0) , 3: gold , 4 : 컬러벅스 )
                    {"price_amount", delArray[i].PriceAmount}, // 판매 가격
                    {"category", cateArray} // 카테고리
                };
                array.Add(bson);
            }

            // 신규 값 추가
            foreach (var item in items)
            {
                var cateSplitArray = item.Category.Split(",");
                var cateArray = new BsonArray {
                        cateSplitArray[0],
                        cateSplitArray[0]+","+cateSplitArray[1],
                        cateSplitArray[0] + "," + cateSplitArray[1]+","+cateSplitArray[2]
                };
                var domainType = (SvcDomainType)Uuid.FromBase62(item.Productid).DomainType;
                var type = 1;
                if (domainType.ToString().Equals("MetaAsset"))
                {
                    type = 2;
                }
                var bson = new BsonDocument {
                    {"productid", Uuid.FromBase62(item.Productid).ToBsonBinaryData()},  // 마켓아이디 (구매해야될 아이디 (아이템아이디, 애셋아이디))
                    {"quantity", 1}, // 수량
                    {"type", type}, // 아이템 1 , 애셋 2
                    {"price_type", item.PriceType}, // 판매재화타입 ( 1 : 비매품 , 2: 무료(0) , 3: gold , 4 : 컬러벅스 )
                    {"price_amount", item.PriceAmount}, // 판매 가격
                    {"category", cateArray} // 카테고리
                };
                array.Add(bson);
            }

            Set("items", array);

        }
        else
        {
            var array = new BsonArray();
            foreach (var item in items)
            {
                var cateSplitArray = item.Category.Split(",");
                var cateArray = new BsonArray {
                        cateSplitArray[0],
                        cateSplitArray[0]+","+cateSplitArray[1],
                        cateSplitArray[0] + "," + cateSplitArray[1]+","+cateSplitArray[2]
                };
                var domainType = (SvcDomainType)Uuid.FromBase62(item.Productid).DomainType;
                var type = 1;
                if (domainType.ToString().Equals("MetaAsset"))
                {
                    type = 2;
                }
                var bson = new BsonDocument {
                    {"productid", Uuid.FromBase62(item.Productid).ToBsonBinaryData()},  // 마켓아이디 (구매해야될 아이디 (아이템아이디, 애셋아이디))
                    {"quantity", 1}, // 수량
                    {"type", type}, // 아이템 1 , 애셋 2
                    {"price_type", item.PriceType}, // 판매재화타입 ( 1 : 비매품 , 2: 무료(0) , 3: gold , 4 : 컬러벅스 )
                    {"price_amount", item.PriceAmount}, // 판매 가격
                    {"category", cateArray} // 카테고리
                };
                array.Add(bson);
            }
            Set("items", array);
        }
    }

    /// <summary>
    /// DeleteCart
    /// </summary>
    /// <param name="productid"></param>
    public void DeleteCart(Uuid productid)
    {
        var bson = new BsonDocument
            {
                 {"productid", productid.ToBsonBinaryData()}
            };
        Pull("items", bson);
    }

    /// <summary>
    /// DeletePurchaseCart
    /// </summary>
    /// <param name="request"></param>
    public void DeletePurchaseCart(PurchaseCartListRequest request)
    {
        var array = new BsonArray();
        foreach (var item in request.Items)
        {
            var bson = new BsonDocument
            {
                 {"productid", Uuid.FromBase62(item.Productid).ToBsonBinaryData()}
            };
            array.Add(bson);
        }
        PullAll("items", array); // array pull 또는 pullall 은 update many 일때 사용가능
    }
}
