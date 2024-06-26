﻿// Copyright (c) Rodel. All rights reserved.

using Microsoft.UI.Windowing;
using RodelAgent.UI.Pages;

namespace RodelAgent.UI.Forms;

/// <summary>
/// 初始化启动窗口，用于进行初始化配置.
/// </summary>
public sealed partial class StartupWindow : WindowBase, ITipWindow
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

        this.CenterOnScreen();
        _ = MainFrame.Navigate(typeof(StartupPage));
    }

    /// <inheritdoc/>
    public async Task ShowTipAsync(UIElement element, double delaySeconds)
    {
        TipContainer.Visibility = Visibility.Visible;
        TipContainer.Children.Add(element);
        element.Visibility = Visibility.Visible;
        await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
        element.Visibility = Visibility.Collapsed;
        _ = TipContainer.Children.Remove(element);
        if (TipContainer.Children.Count == 0)
        {
            TipContainer.Visibility = Visibility.Collapsed;
        }
    }
}
