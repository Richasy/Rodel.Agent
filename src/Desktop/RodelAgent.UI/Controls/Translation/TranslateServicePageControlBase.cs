// Copyright (c) Rodel. All rights reserved.

using RodelAgent.UI.ViewModels.Pages;

namespace RodelAgent.UI.Controls.Translation;

/// <summary>
/// 翻译服务页面控件基类.
/// </summary>
public abstract class TranslateServicePageControlBase : ReactiveUserControl<TranslateServicePageViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TranslateServicePageControlBase"/> class.
    /// </summary>
    protected TranslateServicePageControlBase()
        => ViewModel = this.Get<TranslateServicePageViewModel>();
}
