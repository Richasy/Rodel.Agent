// Copyright (c) Rodel. All rights reserved.

using RodelAgent.UI.Forms;
using RodelAgent.UI.Models.Constants;

namespace RodelAgent.UI.Controls;

/// <summary>
/// 消息提醒.
/// </summary>
public sealed partial class TipPopup : UserControl
{
    /// <summary>
    /// <see cref="Text"/>的依赖属性.
    /// </summary>
    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string), typeof(TipPopup), new PropertyMetadata(string.Empty));

    private readonly ITipWindow _targetWindow;

    /// <summary>
    /// Initializes a new instance of the <see cref="TipPopup"/> class.
    /// </summary>
    public TipPopup() => InitializeComponent();

    /// <summary>
    /// Initializes a new instance of the <see cref="TipPopup"/> class.
    /// </summary>
    /// <param name="text">要显示的文本.</param>
    public TipPopup(string text, ITipWindow targetWindow = default)
        : this()
    {
        Text = text;
        _targetWindow = targetWindow;
    }

    /// <summary>
    /// 显示文本.
    /// </summary>
    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    /// <summary>
    /// 显示内容.
    /// </summary>
    /// <param name="type">信息级别.</param>
    /// <returns><see cref="Task"/>.</returns>
    public async Task ShowAsync(InfoType type = InfoType.Information)
    {
        PopupContainer.Status = type;
        Visibility = Visibility.Visible;
        await Task.Delay(TimeSpan.FromSeconds(3.5));
        Visibility = Visibility.Collapsed;
    }
}
