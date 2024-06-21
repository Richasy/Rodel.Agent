// Copyright (c) Rodel. All rights reserved.

using System;

namespace RodelAgent.Statics;

/// <summary>
/// 全局静态类.
/// </summary>
public static class GlobalStatics
{
    /// <summary>
    /// 服务提供程序.
    /// </summary>
    public static IServiceProvider ServiceProvider { get; private set; }

    /// <summary>
    /// 设置服务提供程序.
    /// </summary>
    public static void SetServiceProvider(IServiceProvider serviceProvider)
        => ServiceProvider = serviceProvider;
}
