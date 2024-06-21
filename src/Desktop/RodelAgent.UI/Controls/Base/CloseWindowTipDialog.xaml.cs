// Copyright (c) Rodel. All rights reserved.

namespace RodelAgent.UI.Controls;

/// <summary>
/// 关闭窗口提示对话框.
/// </summary>
public sealed partial class CloseWindowTipDialog : AppContentDialog
{
    /// <summary>
    /// Dependency property for <see cref="IsNeverAskChecked"/>.
    /// </summary>
    public static readonly DependencyProperty IsNeverAskCheckedProperty =
        DependencyProperty.Register(nameof(IsNeverAskChecked), typeof(bool), typeof(CloseWindowTipDialog), new PropertyMetadata(true));

    /// <summary>
    /// Initializes a new instance of the <see cref="CloseWindowTipDialog"/> class.
    /// </summary>
    public CloseWindowTipDialog() => InitializeComponent();

    /// <summary>
    /// Is never ask checked.
    /// </summary>
    public bool IsNeverAskChecked
    {
        get => (bool)GetValue(IsNeverAskCheckedProperty);
        set => SetValue(IsNeverAskCheckedProperty, value);
    }
}
