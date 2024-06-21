// Copyright (c) Rodel. All rights reserved.

using RodelAgent.UI.ViewModels.Pages;

namespace RodelAgent.UI.Controls.Draw;

/// <summary>
/// 绘图服务页面控件基类.
/// </summary>
public abstract class DrawServicePageControlBase : ReactiveUserControl<DrawServicePageViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DrawServicePageControlBase"/> class.
    /// </summary>
    protected DrawServicePageControlBase()
        => ViewModel = ServiceProvider.GetRequiredService<DrawServicePageViewModel>();
}
