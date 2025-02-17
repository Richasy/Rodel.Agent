// Copyright (c) Richasy. All rights reserved.

using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml.Media.Animation;
using RodelAgent.UI.Models.Constants;
using RodelAgent.UI.Pages;
using RodelAgent.UI.Toolkits;
using RodelAgent.UI.ViewModels.Core;

namespace RodelAgent.UI.Forms;

/// <summary>
/// 初始引导窗口.
/// </summary>
public sealed partial class StartupWindow : WindowBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StartupWindow"/> class.
    /// </summary>
    public StartupWindow()
    {
        InitializeComponent();
        IsMaximizable = false;
        IsMinimizable = false;
        IsResizable = false;
        AppWindow.SetPresenter(AppWindowPresenterKind.CompactOverlay);

        Width = 720;
        Height = 460;

        Title = ResourceToolkit.GetLocalizedString(StringNames.AppName);

        this.CenterOnScreen();
        this.SetIcon("Assets/logo.ico");
        this.SetTitleBar(TitleBar);
        this.Get<AppViewModel>().Windows.Add(this);
        this.Get<AppViewModel>().ActivatedWindow = this;

        Activated += OnActivated;

        RootFrame.Navigate(typeof(StartupPage), default, new SuppressNavigationTransitionInfo());
    }

    private void OnActivated(object sender, WindowActivatedEventArgs args)
        => this.Get<AppViewModel>().ActivatedWindow = this;
}
