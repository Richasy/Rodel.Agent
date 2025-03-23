// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.ViewModels.View;

namespace RodelAgent.UI.Controls.Startup;

/// <summary>
/// Startup page control base.
/// </summary>
public abstract class StartupPageControlBase : LayoutUserControlBase<StartupPageViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StartupPageControlBase"/> class.
    /// </summary>
    protected StartupPageControlBase() => ViewModel = this.Get<StartupPageViewModel>();
}
