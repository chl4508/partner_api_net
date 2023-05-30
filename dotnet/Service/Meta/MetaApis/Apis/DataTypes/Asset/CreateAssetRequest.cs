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

namespace Colorverse.Meta.Apis.DataTypes.Asset;

/// <summary>
/// CreateAssetRequest
/// </summary>
public class CreateAssetRequest : RequestBase<AssetResponse>
{
    /// <summary>
    /// 애셋 아이디
    /// </summary>
    [BindNever]
    [JsonIgnore]
    public string Assetid { get; set; } = null!;

    /// <summary>
    /// manifestjson의 애셋타입 (item, avatar, land... )
    /// </summary>
    public int Type { get; set; }

    /// <summary>
    /// Txt
    /// </summary>
    [Required]
    public CreateAssetTxtRequest Txt { get; set; } = null!;

    /// <summary>
    /// option
    /// </summary>S
    [Required]
    public CreateAssetOptionRequest Option { get; set; } = null!;

    /// <summary>
    /// Resource
    /// </summary>
    [Required]
    public CreateAssetResourceRequest Resource { get; set; } = null!;
}

/// <summary>
/// CreateAssetResourceRequest
/// </summary>
public class CreateAssetResourceRequest
{
    /// <summary>
    /// Thumbnail cdn url
    /// </summary>
    public string? Thumbnail { get; set; }
}

/// <summary>
/// CreateItemTxtRequest
/// </summary>
public class CreateAssetTxtRequest
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
public class CreateAssetOptionRequest
{
    /// <summary>
    /// 버전
    /// </summary>
    [Range(1, int.MaxValue)]
    public int Version { get; set; }

    /// <summary>
    /// Category
    /// </summary>
    [Required]
    public string Category { get; set; } = null!;

    /// <summary>
    /// Price
    /// </summary>
    public CreateAssetPriceRequest? Price { get; set; }
}

/// <summary>
/// CreateItemPriceRequest
/// </summary>
public class CreateAssetPriceRequest
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
public class CreateAssetRequestValidator : AbstractValidator<CreateAssetRequest>
{
    public CreateAssetRequestValidator(IWorldSettingHelper worldSettingHelper)
    {
        WorldSetting _worldSetting = worldSettingHelper.GetSetting(Uuid.FromBase62("1mkSuyvWsZAZKE2cuDzmzI"));

        RuleFor(x => Uuid.FromBase62(x.Assetid).DomainType).NotNull().Equal((ushort)SvcDomainType.MetaAsset);
        RuleFor(x => x.Txt.Title.Ko).NotNull().Matches(_worldSetting.TitleRegex);
        RuleFor(x => x.Txt.Desc!.Ko).Matches(_worldSetting.DescRegex).When(x => x.Txt?.Desc != null);
        RuleFor(x => x.Txt!.Hashtag).ForEach(x => x.Matches(_worldSetting.HashtagRegex)).When(x => x.Txt?.Hashtag != null);
        RuleFor(x => x.Txt!.Hashtag!.Length).LessThanOrEqualTo(_worldSetting.HashtagLength).When(x => x.Txt?.Hashtag != null);

        RuleFor(x => x.Option).NotNull();
        RuleFor(x => x.Option.Category).NotNull().Must(cateogry => _worldSetting.Category.Check(cateogry));

        RuleFor(x => x.Resource.Thumbnail).Matches(_worldSetting.Base62Regex).When(x => x.Resource.Thumbnail != null);
    }
}