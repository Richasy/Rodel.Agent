// Copyright (c) Rodel. All rights reserved.

namespace RodelAgent.UI.Controls;

/// <summary>
/// 回车换行提示.
/// </summary>
public sealed partial class EnterWrapTip : UserControl
{
    /// <summary>
    /// <see cref="IsEnterSend"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty IsEnterSendProperty =
        DependencyProperty.Register(nameof(IsEnterSend), typeof(bool), typeof(EnterWrapTip), new PropertyMetadata(default));

    /// <summary>
    /// Initializes a new instance of the <see cref="EnterWrapTip"/> class.
    /// </summary>
    public EnterWrapTip()
    {
        InitializeComponent();
    }

    /// <summary>
    /// 是否回车发送.
    /// </summary>
    public bool IsEnterSend
    {
        get => (bool)GetValue(IsEnterSendProperty);
        set => SetValue(IsEnterSendProperty, value);
    }
}
