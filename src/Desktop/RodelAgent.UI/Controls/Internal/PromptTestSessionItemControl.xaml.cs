// Copyright (c) Rodel. All rights reserved.

using RodelAgent.UI.ViewModels;

namespace RodelAgent.UI.Controls.Internal;

/// <summary>
/// 提示词测试会话项.
/// </summary>
public sealed partial class PromptTestSessionItemControl : PromptTestSessionItemControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PromptTestSessionItemControl"/> class.
    /// </summary>
    public PromptTestSessionItemControl() => InitializeComponent();

    private void OnResultButtonClick(object sender, RoutedEventArgs e)
        => FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
}

/// <summary>
/// 提示词测试会话项控件基类.
/// </summary>
public abstract class PromptTestSessionItemControlBase : LayoutUserControlBase<PromptTestSessionItemViewModel>
{
}
