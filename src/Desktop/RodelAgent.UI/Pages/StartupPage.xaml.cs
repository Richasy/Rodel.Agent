// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.ViewModels.View;

namespace RodelAgent.UI.Pages;

/// <summary>
/// 启动页.
/// </summary>
public sealed partial class StartupPage : StartupPageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StartupPage"/> class.
    /// </summary>
    public StartupPage() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnPageLoaded()
        => ViewModel.InitializeCommand.Execute(default);
}

/// <summary>
/// 启动页基类.
/// </summary>
public abstract class StartupPageBase : LayoutPageBase<StartupPageViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StartupPageBase"/> class.
    /// </summary>
    protected StartupPageBase() => ViewModel = this.Get<StartupPageViewModel>();
}
