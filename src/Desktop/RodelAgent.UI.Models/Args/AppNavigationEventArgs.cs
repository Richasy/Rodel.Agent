// Copyright (c) Rodel. All rights reserved.

using System;

namespace RodelAgent.UI.Models.Args;

/// <summary>
/// 应用程序导航事件参数.
/// </summary>
public sealed class AppNavigationEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppNavigationEventArgs"/> class.
    /// </summary>
    public AppNavigationEventArgs(Type pageType, object parameter)
    {
        PageType = pageType;
        Parameter = parameter;
    }

    /// <summary>
    /// 页面类型.
    /// </summary>
    public Type PageType { get; }

    /// <summary>
    /// 页面参数.
    /// </summary>
    public object Parameter { get; }
}
