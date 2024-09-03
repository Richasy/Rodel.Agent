// Copyright (c) Rodel. All rights reserved.

using Microsoft.UI.Windowing;
using RodelAgent.UI.Controls;
using RodelAgent.UI.Models.Constants;
using RodelAgent.UI.Toolkits;
using RodelAgent.UI.ViewModels;
using Windows.Graphics;

namespace RodelAgent.UI.Forms;

/// <summary>
/// 主窗口.
/// </summary>
public sealed partial class MainWindow : WindowBase, ITipWindow
{
    private bool _isFirstActivated = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();
        SetTitleBar(RootLayout.GetMainTitleBar());
        Title = ResourceToolkit.GetLocalizedString(StringNames.AppName);
        this.SetIcon("Assets/logo.ico");
        AppWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Tall;
        MinWidth = 900;
        MinHeight = 640;
        AppWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Tall;
        this.Get<AppViewModel>().DisplayWindows.Add(this);

        Activated += OnWindowActivated;
        Closed += OnWindowClosed;

        MoveAndResize();
    }

    /// <inheritdoc/>
    public async Task ShowTipAsync(string text, InfoType type = InfoType.Error)
    {
        var popup = new TipPopup() { Text = text };
        TipContainer.Visibility = Visibility.Visible;
        TipContainer.Children.Add(popup);
        await popup.ShowAsync(type);
        TipContainer.Children.Remove(popup);
        TipContainer.Visibility = Visibility.Collapsed;
    }

    private static PointInt32 GetSavedWindowPosition()
    {
        var left = SettingsToolkit.ReadLocalSetting(SettingNames.MainWindowPositionLeft, 0);
        var top = SettingsToolkit.ReadLocalSetting(SettingNames.MainWindowPositionTop, 0);
        return new PointInt32(left, top);
    }

    private void OnWindowActivated(object sender, WindowActivatedEventArgs args)
    {
        if (args.WindowActivationState != WindowActivationState.Deactivated)
        {
            this.Get<AppViewModel>().ActivatedWindow = this;
        }

        if (!_isFirstActivated)
        {
            return;
        }

        var isMaximized = SettingsToolkit.ReadLocalSetting(SettingNames.IsMainWindowMaximized, false);
        if (isMaximized)
        {
            (AppWindow.Presenter as OverlappedPresenter).Maximize();
        }

        var localTheme = SettingsToolkit.ReadLocalSetting(SettingNames.AppTheme, ElementTheme.Default);
        this.Get<AppViewModel>().ChangeTheme(localTheme);
        _isFirstActivated = false;
    }

    private void OnWindowClosed(object sender, WindowEventArgs args)
    {
        foreach (var item in this.Get<AppViewModel>().DisplayWindows.ToArray())
        {
            if (item is not MainWindow)
            {
                item.Close();
            }
        }

        var hideWhenClose = SettingsToolkit.ReadLocalSetting(SettingNames.HideWhenCloseWindow, false);
        if (!hideWhenClose)
        {
            Activated -= OnWindowActivated;
            Closed -= OnWindowClosed;
        }

        SaveCurrentWindowStats();
    }

    private RectInt32 GetRenderRect(RectInt32 workArea)
    {
        var scaleFactor = this.GetDpiForWindow() / 96d;
        var previousWidth = SettingsToolkit.ReadLocalSetting(SettingNames.MainWindowWidth, 1120d);
        var previousHeight = SettingsToolkit.ReadLocalSetting(SettingNames.MainWindowHeight, 740d);
        var width = Convert.ToInt32(previousWidth * scaleFactor);
        var height = Convert.ToInt32(previousHeight * scaleFactor);

        // Ensure the window is not larger than the work area.
        if (height > workArea.Height - 20)
        {
            height = workArea.Height - 20;
        }

        var lastPoint = GetSavedWindowPosition();
        var isZeroPoint = lastPoint.X == 0 && lastPoint.Y == 0;
        var isValidPosition = lastPoint.X >= workArea.X && lastPoint.Y >= workArea.Y;
        var left = isZeroPoint || !isValidPosition
            ? (workArea.Width - width) / 2d
            : lastPoint.X;
        var top = isZeroPoint || !isValidPosition
            ? (workArea.Height - height) / 2d
            : lastPoint.Y;
        return new RectInt32(Convert.ToInt32(left), Convert.ToInt32(top), width, height);
    }

    private void MoveAndResize()
    {
        var lastPoint = GetSavedWindowPosition();
        var displayArea = DisplayArea.GetFromPoint(lastPoint, DisplayAreaFallback.Primary)
            ?? DisplayArea.Primary;
        var rect = GetRenderRect(displayArea.WorkArea);
        AppWindow.MoveAndResize(rect);
    }

    private void SaveCurrentWindowStats()
    {
        var left = AppWindow.Position.X;
        var top = AppWindow.Position.Y;
        var isMaximized = PInvoke.IsZoomed(new HWND(this.GetWindowHandle()));
        SettingsToolkit.WriteLocalSetting(SettingNames.IsMainWindowMaximized, (bool)isMaximized);

        if (!isMaximized)
        {
            SettingsToolkit.WriteLocalSetting(SettingNames.MainWindowPositionLeft, left);
            SettingsToolkit.WriteLocalSetting(SettingNames.MainWindowPositionTop, top);
            SettingsToolkit.WriteLocalSetting(SettingNames.MainWindowHeight, Height < 640 ? 640d : Height);
            SettingsToolkit.WriteLocalSetting(SettingNames.MainWindowWidth, Width);
        }
    }
}
