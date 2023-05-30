using Colorverse.Application.Mediator;
using CvFramework.Common;
using FluentValidation;
using MediatR;

namespace Colorverse.Meta.DataTypes.Resource;

/// <summary>
/// 
/// </summary>
public class DeleteResourceParam : RequestBase<bool>, IRequest<bool>
{
    /// <summary>
    /// 파일
    /// </summary>
    public Uuid Id { get; set; } = null!;

    /// <summary>
    /// 버전
    /// </summary>
    public int Version { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="version"></param>
    public DeleteResourceParam(Uuid id, int version)
    {
        Id = id;
        Version = version;
    }
}

/// <summary>
/// 
/// </summary>
public class DeleteResourceParamValidator : AbstractValidator<DeleteResourceParam>
{
    public DeleteResourceParamValidator()
    {
        RuleFor(x=>x.Id).NotNull();
    }
}