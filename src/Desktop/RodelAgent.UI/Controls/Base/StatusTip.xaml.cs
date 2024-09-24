// Copyright (c) Rodel. All rights reserved.

using RodelAgent.UI.Models.Constants;

namespace RodelAgent.UI.Controls;

/// <summary>
/// 状态提示.
/// </summary>
public sealed partial class StatusTip : LayoutUserControlBase
{
    /// <summary>
    /// <see cref="Text"/>的依赖属性.
    /// </summary>
    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string), typeof(StatusTip), new PropertyMetadata(string.Empty));

    /// <summary>
    /// <see cref="Status"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty StatusProperty =
        DependencyProperty.Register(nameof(Status), typeof(InfoType), typeof(StatusTip), new PropertyMetadata(default, new PropertyChangedCallback(OnStatusChanged)));

    /// <summary>
    /// Initializes a new instance of the <see cref="StatusTip"/> class.
    /// </summary>
    public StatusTip() => InitializeComponent();

    /// <summary>
    /// 当前状态.
    /// </summary>
    public InfoType Status
    {
        get => (InfoType)GetValue(StatusProperty);
        set => SetValue(StatusProperty, value);
    }

    /// <summary>
    /// 显示文本.
    /// </summary>
    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    /// <inheritdoc/>
    protected override void OnControlLoaded()
        => ChangeStatus();

    private static void OnStatusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var instance = d as StatusTip;
        instance.ChangeStatus();
    }

    private void ChangeStatus()
    {
        InformationIcon.Visibility = Visibility.Collapsed;
        SuccessIcon.Visibility = Visibility.Collapsed;
        WarningIcon.Visibility = Visibility.Collapsed;
        ErrorIcon.Visibility = Visibility.Collapsed;
        LoadingRing.Visibility = Visibility.Collapsed;
        LoadingRing.IsActive = false;

        switch (Status)
        {
            case InfoType.Information:
                InformationIcon.Visibility = Visibility.Visible;
                break;
            case InfoType.Success:
                SuccessIcon.Visibility = Visibility.Visible;
                break;
            case InfoType.Warning:
                WarningIcon.Visibility = Visibility.Visible;
                break;
            case InfoType.Error:
                ErrorIcon.Visibility = Visibility.Visible;
                break;
            case InfoType.Loading:
                LoadingRing.Visibility = Visibility.Visible;
                LoadingRing.IsActive = true;
                break;
            default:
                break;
        }
    }
}

