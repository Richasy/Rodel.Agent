// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.ViewModels.View;

namespace RodelAgent.UI.Controls.Translate;

/// <summary>
/// 翻译页面控件基类.
/// </summary>
public abstract class TranslatePageControlBase : LayoutUserControlBase<TranslatePageViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TranslatePageControlBase"/> class.
    /// </summary>
    protected TranslatePageControlBase() => ViewModel = this.Get<TranslatePageViewModel>();
}
