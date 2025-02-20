// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.ViewModels.View;

namespace RodelAgent.UI.Pages;

/// <summary>
/// Translate page.
/// </summary>
public sealed partial class TranslatePage : TranslatePageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TranslatePage"/> class.
    /// </summary>
    public TranslatePage() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnPageLoaded()
        => ViewModel.InitializeCommand.Execute(default);
}

/// <summary>
/// Translate page base.
/// </summary>
public abstract class TranslatePageBase : LayoutPageBase<TranslatePageViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TranslatePageBase"/> class.
    /// </summary>
    protected TranslatePageBase() => ViewModel = this.Get<TranslatePageViewModel>();
}