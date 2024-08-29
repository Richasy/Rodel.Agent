// Copyright (c) Rodel. All rights reserved.

using Microsoft.UI.Input;
using Microsoft.UI.Windowing;
using RodelAgent.UI.Toolkits;
using RodelAgent.UI.ViewModels;
using Windows.Graphics;

namespace RodelAgent.UI.Controls.Main;

/// <summary>
/// 应用程序标题栏.
/// </summary>
public sealed partial class AppTitleBar : AppTitleBarBase
{
    /// <summary>
    /// <see cref="AttachedWindow"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty AttachedWindowProperty =
        DependencyProperty.Register(nameof(AttachedWindow), typeof(WindowBase), typeof(AppTitleBar), new PropertyMetadata(default));

    /// <summary>
    /// Initializes a new instance of the <see cref="AppTitleBar"/> class.
    /// </summary>
    public AppTitleBar()
    {
        InitializeComponent();
        ViewModel = this.Get<AppViewModel>();
        Loaded += OnLoaded;
        SizeChanged += OnSizeChanged;

        if (!GlobalFeatureSwitcher.IsGlobalSearchEnabled)
        {
            AppSearchBox.Visibility = Visibility.Collapsed;
        }
    }

    /// <summary>
    /// 附加的窗口对象.
    /// </summary>
    public WindowBase AttachedWindow
    {
        get => (WindowBase)GetValue(AttachedWindowProperty);
        set => SetValue(AttachedWindowProperty, value);
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
        => SetDragRegion();

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        => SetDragRegion();

    private void SetDragRegion()
    {
        if (AttachedWindow.AppWindow == null || !AppWindowTitleBar.IsCustomizationSupported())
        {
            return;
        }

        var scaleFactor = AttachedWindow.GetDpiForWindow() / 96d;
        var transform = AppSearchBox.TransformToVisual(default);
        var searchBoxBounds = transform.TransformBounds(new Rect(0, 0, AppSearchBox.ActualWidth, AppSearchBox.ActualHeight));
        var searchBoxRect = AppToolkit.GetRectInt32(searchBoxBounds, scaleFactor);

        var nonClientInputSrc = InputNonClientPointerSource.GetForWindowId(Win32Interop.GetWindowIdFromWindow(AttachedWindow.GetWindowHandle()));
        nonClientInputSrc.SetRegionRects(NonClientRegionKind.Passthrough, new RectInt32[] { searchBoxRect });
    }
}

/// <summary>
/// 应用程序标题栏基类.
/// </summary>
public abstract class AppTitleBarBase : LayoutUserControlBase<AppViewModel>
{
}
