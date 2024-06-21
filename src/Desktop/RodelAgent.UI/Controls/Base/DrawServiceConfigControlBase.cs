// Copyright (c) Rodel. All rights reserved.

using RodelAgent.UI.ViewModels.Items;

namespace RodelAgent.UI.Controls;

/// <summary>
/// 聊天服务配置控件基类.
/// </summary>
public abstract class DrawServiceConfigControlBase : ReactiveUserControl<DrawServiceItemViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DrawServiceConfigControlBase"/> class.
    /// </summary>
    protected DrawServiceConfigControlBase() => IsTabStop = false;
}
