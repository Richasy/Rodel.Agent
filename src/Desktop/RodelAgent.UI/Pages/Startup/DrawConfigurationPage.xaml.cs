// Copyright (c) Rodel. All rights reserved.

using RodelAgent.UI.ViewModels.Items;

namespace RodelAgent.UI.Pages.Startup;

/// <summary>
/// 绘图会话配置页面.
/// </summary>
public sealed partial class DrawConfigurationPage : DrawConfigurationPageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DrawConfigurationPage"/> class.
    /// </summary>
    public DrawConfigurationPage() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnNavigatedTo(NavigationEventArgs e)
        => ViewModel = e.Parameter as DrawServiceItemViewModel;

    /// <inheritdoc/>
    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        ViewModel.CheckCurrentConfig();
        ViewModel = default;
    }
}

/// <summary>
/// 绘图配置页面基类.
/// </summary>
public abstract class DrawConfigurationPageBase : LayoutPageBase<DrawServiceItemViewModel>
{
}
