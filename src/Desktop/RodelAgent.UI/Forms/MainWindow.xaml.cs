// Copyright (c) Rodel. All rights reserved.

using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml.Media.Animation;
using RodelAgent.UI.Controls;
using RodelAgent.UI.Models.Args;
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
        Title = ResourceToolkit.GetLocalizedString(StringNames.AppName);
        this.SetIcon("Assets/logo.ico");
        AppWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Tall;
        MinWidth = 900;
        MinHeight = 640;
        AppTitleBar.AttachedWindow = this;
        SetTitleBar(AppTitleBar);
        AppWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Tall;
        this.Get<AppViewModel>().DisplayWindows.Add(this);

        Activated += OnWindowActivated;
        Closed += OnWindowClosed;

        MoveAndResize();
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

    private static PointInt32 GetSavedWindowPosition()
    {
        var left = SettingsToolkit.ReadLocalSetting(SettingNames.MainWindowPositionLeft, 0);
        var top = SettingsToolkit.ReadLocalSetting(SettingNames.MainWindowPositionTop, 0);
        return new PointInt32(left, top);
    }

    private void OnFrameUnloaded(object sender, RoutedEventArgs e)
    {
        this.Get<AppViewModel>().NavigationRequested -= OnNavigationRequested;
        this.Get<AppViewModel>().RequestShowTip -= OnRequestShowTip;
    }

    private void OnFrameLoaded(object sender, RoutedEventArgs e)
    {
        this.Get<AppViewModel>().NavigationRequested += OnNavigationRequested;
        this.Get<AppViewModel>().RequestShowTip += OnRequestShowTip;
        this.Get<AppViewModel>().InitializeCommand.Execute(default);
    }

    private void OnWindowActivated(object sender, WindowActivatedEventArgs args)
    {
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

        SaveCurrentWindowStats();
    }

    private void OnRequestShowTip(object sender, AppTipNotification e)
    {
        if (e.TargetWindow is ITipWindow window)
        {
            new TipPopup(e.Message, window).ShowAsync(e.Type);
        }
    }

    private void OnNavigationRequested(object sender, AppNavigationEventArgs e)
        => MainFrame.Navigate(e.PageType, e.Parameter, new DrillInNavigationTransitionInfo());

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
