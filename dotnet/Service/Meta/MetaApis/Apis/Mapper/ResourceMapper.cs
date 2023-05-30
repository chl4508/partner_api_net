using Colorverse.Meta.Apis.DataTypes.Resource;
using Colorverse.Meta.DataTypes.Resource;
using CvFramework.Common;

namespace Colorverse.Meta.Apis.Mapper;

/// <summary>
/// 
/// </summary>
public static class ResourceMapper
{
    public static UploadResourceParam To(UploadResourceRequest source)
    {
        var to = new UploadResourceParam()
        {
            File = source.File,
            Type = source.Type
        };

        if (source.TargetId != null)
        {
            to.TargetUuid = Uuid.FromBase62(source.TargetId);
        }
        return to;
    }
}
