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
    /// <param name="displaySeconds">显示的时间.</param>
    public async void ShowAsync(InfoType type = InfoType.Information, double displaySeconds = 3.5)
    {
        PopupContainer.Status = type;

        if (_targetWindow is not null)
        {
            await _targetWindow.ShowTipAsync(this, displaySeconds);
        }
    }
}
