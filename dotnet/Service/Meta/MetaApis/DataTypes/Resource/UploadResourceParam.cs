using Colorverse.Common.Enums;
using Colorverse.Application.Mediator;
using CvFramework.Common;
using FluentValidation;
using MediatR;

namespace Colorverse.Meta.DataTypes.Resource;

/// <summary>
/// 아이템 생성 Param
/// </summary>
public class UploadResourceParam : RequestBase<ResourceDto>, IRequest<ResourceDto>
{
    /// <summary>
    /// 파일
    /// </summary>
    public IFormFile File { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    public EResourceType Type { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public Uuid? TargetUuid { get; set; } = null!;

}

/// <summary>
/// 
/// </summary>
public class UploadResourceParamValidator : AbstractValidator<UploadResourceParam>
{
    public UploadResourceParamValidator()
    {
        RuleFor(x=>x.File).NotNull();
        RuleFor(x=>x.Type).NotEqual(EResourceType.Default);
    }
}