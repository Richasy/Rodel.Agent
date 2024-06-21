// Copyright (c) Rodel. All rights reserved.

using RodelAgent.UI.ViewModels;

namespace RodelAgent.UI.Controls.Main;

/// <summary>
/// 导航面板.
/// </summary>
public sealed partial class NavigationPanel : NavigationPanelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationPanel"/> class.
    /// </summary>
    public NavigationPanel()
    {
        InitializeComponent();
        ViewModel = ServiceProvider.GetRequiredService<AppViewModel>();
    }
}

/// <summary>
/// 导航面板基类.
/// </summary>
public abstract class NavigationPanelBase : ReactiveUserControl<AppViewModel>
{
}
