// Copyright (c) Rodel. All rights reserved.

using RodelAgent.UI.ViewModels.Pages;

namespace RodelAgent.UI.Controls.Internal;

/// <summary>
/// 提示词测试页面控件基类.
/// </summary>
public abstract class PromptTestPageControlBase : LayoutUserControlBase<PromptTestPageViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PromptTestPageControlBase"/> class.
    /// </summary>
    protected PromptTestPageControlBase() => ViewModel = this.Get<PromptTestPageViewModel>();
}
