// Copyright (c) Richasy. All rights reserved.

using RichasyKernel;

namespace RodelAgent.Statics;

/// <summary>
/// 全局静态类.
/// </summary>
public static class GlobalStatics
{
    /// <summary>
    /// 服务提供程序.
    /// </summary>
    public static Kernel Kernel { get; private set; }

    /// <summary>
    /// 设置服务提供程序.
    /// </summary>
    public static void SetKernel(Kernel kernel)
        => Kernel = kernel;
}
