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
/// CreateItemRequest
/// </summary>
public class CreateItemRequest : RequestBase<ItemResponse>
{
    /// <summary>
    /// 아이템아이디
    /// </summary>
    [BindNever]
    [JsonIgnore]
    public string Itemid { get; set; } = null!;

    /// <summary>
    /// Txt
    /// </summary>
    [Required]
    public CreateItemTxtRequest Txt { get; set; } = null!;

    /// <summary>
    /// option
    /// </summary>
    [Required]
    public CreateItemOptionRequest Option { get; set; } = null!;

    /// <summary>
    /// resource
    /// </summary>
    [Required]
    public CreateItemResourceRequest Resource { get; set; } = null!;
}

/// <summary>
/// CreateItemResourceRequest
/// </summary>
public class CreateItemResourceRequest
{
    /// <summary>
    /// Thumbnail cdn url
    /// </summary>
    public string? Thumbnail { get; set; }

    /// <summary>
    /// Preview cdn url
    /// </summary>
    public string Preview { get; set; } = null!;
}

/// <summary>
/// CreateItemTxtRequest
/// </summary>
public class CreateItemTxtRequest
{
    /// <summary>
    /// 제목
    /// </summary>
    [Required]
    public CvLocaleText Title { get; set; } = null!;

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
/// CreateItemOptionRequest
/// </summary>
public class CreateItemOptionRequest
{
    /// <summary>
    /// 템플릿 유무 (true : 템플릿 , 없을경우 nullable)
    /// </summary>
    public bool? Template { get; set; }

    /// <summary>
    /// 템플릿 아이디
    /// </summary>
    public string? Templateid { get; set; }

    /// <summary>
    /// 템플릿 버전
    /// </summary>
    public int? TemplateVersion { get; set; }

    /// <summary>
    /// 버전
    /// </summary>
    [Range(1, int.MaxValue)]
    public int Version { get; set; }

    /// <summary>
    /// Category ,로 카테고리뎁스 구분 ex) 1,10000,10001
    /// </summary>
    [Required]
    public string Category { get; set; } = null!;

    /// <summary>
    /// Price
    /// </summary>
    public CreateItemPriceRequest? Price { get; set; }
}

/// <summary>
/// CreateItemPriceRequest
/// </summary>
public class CreateItemPriceRequest
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
public class CreateItemRequestValidator : AbstractValidator<CreateItemRequest>
{
    public CreateItemRequestValidator(IWorldSettingHelper worldSettingHelper)
    {
        WorldSetting _worldSetting = worldSettingHelper.GetSetting(Uuid.FromBase62("1mkSuyvWsZAZKE2cuDzmzI"));

        RuleFor(x => Uuid.FromBase62(x.Itemid).DomainType).NotNull().Equal((ushort)SvcDomainType.MetaItem);
        RuleFor(x => x.Txt.Title.Ko).NotNull().Matches(_worldSetting.TitleRegex);
        RuleFor(x => x.Txt.Desc!.Ko).Matches(_worldSetting.DescRegex).When(x => x.Txt?.Desc != null);
        RuleFor(x => x.Txt!.Hashtag).ForEach(x => x.Matches(_worldSetting.HashtagRegex)).When(x => x.Txt?.Hashtag != null);
        RuleFor(x => x.Txt!.Hashtag!.Length).LessThanOrEqualTo(_worldSetting.HashtagLength).When(x => x.Txt?.Hashtag != null);

        RuleFor(x => x.Option).NotNull();
        RuleFor(x => x.Option.Category).NotNull().Must(cateogry => _worldSetting.Category.Check(cateogry));


        RuleFor(x => x.Resource.Thumbnail).Matches(_worldSetting.Base62Regex).When(x => x.Resource.Thumbnail != null);
        RuleFor(x => x.Resource.Preview).NotNull().Matches(_worldSetting.Base62Regex);
    }
}