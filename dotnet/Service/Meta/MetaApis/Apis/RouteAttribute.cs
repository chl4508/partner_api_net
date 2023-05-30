﻿using Colorverse.Apis.Controller;

namespace Colorverse.Meta.Apis;

/// <summary>
/// 
/// </summary>
public class CvRouteAttribute : ApiRouteAttribute
{
    /// <summary>
    /// 
    /// </summary>
    public const string SERVICE_NAME = "meta";

    /// <summary>
    /// 
    /// </summary>
    public const string API_VERSION = "v1";

    /// <summary>
    /// 
    /// </summary>
    public CvRouteAttribute() : base(SERVICE_NAME, API_VERSION)
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="path"></param>
    public CvRouteAttribute(string path) : base(SERVICE_NAME, API_VERSION, path)
    {
    }
}
