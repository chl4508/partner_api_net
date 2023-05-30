using Colorverse.Application.Mediator;
using Colorverse.Common;
using Colorverse.Common.DataTypes;
using Colorverse.Common.DataTypes.World;
using Colorverse.Common.Helper;
using CvFramework.Common;
using FluentValidation;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Colorverse.Meta.Apis.DataTypes.Item;

/// <summary>
/// UpdateItemRequest
/// </summary>
public class UpdateItemRequest : RequestBase<ItemResponse>
{
    /// <summary>
    /// 아이템아이디
    /// </summary>
    [BindNever]
    [JsonIgnore]
    public string Itemid { get; set; } = null!;

    /// <summary>
    /// txt
    /// </summary>
    public UpdateItemTxtRequest? Txt { get; set; }

    /// <summary>
    /// option
    /// </summary>
    public UpdateItemOptionRequest? Option { get; set; }

    /// <summary>
    /// Resource
    /// </summary>
    public UpdateItemResourceRequest? Resource { get; set; }
}

/// <summary>
/// UpdateItemResourceRequest
/// </summary>
public class UpdateItemResourceRequest
{
    /// <summary>
    /// Thumbnail cdn url
    /// </summary>
    public string? Thumbnail { get; set; }

    /// <summary>
    /// Preview cdn url
    /// </summary>
    public string? Preview { get; set; }
}

/// <summary>
/// UpdateItemTxt
/// </summary>
public class UpdateItemTxtRequest
{
    /// <summary>
    /// 제목
    /// </summary>
    public CvLocaleText? Title { get; set; }

    /// <summary>
    /// 설명
    /// </summary>
    public CvLocaleText? Desc { get; set; }

    /// <summary>
    /// 태그 최대갯수 6개
    /// </summary>
    public string[]? Hashtag { get; set; }
}

/// <summary>
/// UpdateItemOption
/// </summary>
public class UpdateItemOptionRequest
{
    /// <summary>
    /// 버전
    /// </summary>
    [Range(1, int.MaxValue)]
    public int Version { get; set; }

    /// <summary>
    /// Price
    /// </summary>
    public UpdateItemPriceRequest? Price { get; set; }
}

/// <summary>
/// UpdateItemPriceRequest
/// </summary>
public class UpdateItemPriceRequest
{
    /// <summary>
    /// 판매재화타입 ( 1 : 비매품 , 2: 무료(0) , 3: gold , 4 : 컬러벅스 )
    /// </summary>
    public int? Type { get; set; }

    /// <summary>
    /// 판매 가격
    /// </summary>
    public int? Amount { get; set; }
}

/// <summary>
/// 
/// </summary>
public class UpdateItemRequestValidator : AbstractValidator<UpdateItemRequest>
{
    public UpdateItemRequestValidator(IWorldSettingHelper worldSettingHelper)
    {
        WorldSetting _worldSetting = worldSettingHelper.GetSetting(Uuid.FromBase62("1mkSuyvWsZAZKE2cuDzmzI"));

        RuleFor(x => Uuid.FromBase62(x.Itemid).DomainType).NotNull().Equal((ushort)SvcDomainType.MetaItem);
        RuleFor(x => x.Txt!.Title!.Ko).Matches(_worldSetting.TitleRegex).When(x => x.Txt?.Title != null);
        RuleFor(x => x.Txt!.Desc!.Ko).Matches(_worldSetting.DescRegex).When(x => x.Txt?.Desc != null);
        RuleFor(x => x.Txt!.Hashtag).ForEach(x => x.Matches(_worldSetting.HashtagRegex)).When(x => x.Txt?.Hashtag != null && x.Txt.Hashtag.Length != 0);
        
        RuleFor(x => x.Txt!.Hashtag!.Length).LessThanOrEqualTo(_worldSetting.HashtagLength).When(x => x.Txt?.Hashtag != null);

        RuleFor(x => x.Resource!.Thumbnail).Matches(_worldSetting.Base62Regex).When(x => x.Resource != null && x.Resource.Thumbnail != null && x.Resource.Thumbnail != "");
        RuleFor(x => x.Resource!.Preview).Matches(_worldSetting.Base62Regex).When(x => x.Resource != null && x.Resource.Preview != null);
    }
}