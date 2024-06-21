// Copyright (c) Rodel. All rights reserved.

namespace RodelAgent.UI.Controls;

/// <summary>
/// 彩色边框.
/// </summary>
public sealed partial class ColorfulBorder : UserControl
{
    /// <summary>
    /// <see cref="IsLoading"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty IsLoadingProperty =
        DependencyProperty.Register(nameof(IsLoading), typeof(bool), typeof(ColorfulBorder), new PropertyMetadata(default, new PropertyChangedCallback(OnIsLoadingChanged)));

    /// <summary>
    /// Initializes a new instance of the <see cref="ColorfulBorder"/> class.
    /// </summary>
    public ColorfulBorder()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    /// <summary>
    /// 是否处于加载状态.
    /// </summary>
    public bool IsLoading
    {
        get => (bool)GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }

    private static void OnIsLoadingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var instance = d as ColorfulBorder;
        instance?.CheckState();
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
        => CheckState();

    private void CheckState()
    {
        var stateName = IsLoading ? nameof(LoadingState) : nameof(NormalState);
        VisualStateManager.GoToState(this, stateName, false);
    }
}
