// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.ViewModels.Pages;

namespace RodelAgent.UI.Controls.Startup;

/// <summary>
/// 启动页面控件基类.
/// </summary>
public abstract class StartupPageControlBase : LayoutUserControlBase<StartupPageViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StartupPageControlBase"/> class.
    /// </summary>
    protected StartupPageControlBase()
        => ViewModel = this.Get<StartupPageViewModel>();
}
