// Copyright (c) Rodel. All rights reserved.

using RodelAgent.UI.Models.Args;
using RodelAgent.UI.ViewModels.Pages;

namespace RodelAgent.UI.Pages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class StartupPage : StartupPageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StartupPage"/> class.
    /// </summary>
    public StartupPage()
    {
        InitializeComponent();
        ViewModel = this.Get<StartupPageViewModel>();
        ViewModel.SideNavigationRequested += OnSideNavigationRequested;
    }

    /// <inheritdoc/>
    protected override void OnPageLoaded()
        => ViewModel.CheckMigrationCommand.Execute(default);

    private void OnSideNavigationRequested(object sender, AppNavigationEventArgs e)
    {
        MainSplitView.IsPaneOpen = true;
        SideFrame.Navigate(e.PageType, e.Parameter);
    }

    private void OnPaneClosed(SplitView sender, object args)
    {
        if (SideFrame.Content is not null)
        {
            SideFrame.Navigate(typeof(Page));
        }
    }
}

/// <summary>
/// 启动页面基类.
/// </summary>
public abstract class StartupPageBase : LayoutPageBase<StartupPageViewModel>
{
}
