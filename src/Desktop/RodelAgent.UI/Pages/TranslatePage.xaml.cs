// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.ViewModels.View;

namespace RodelAgent.UI.Pages;

/// <summary>
/// Translate page.
/// </summary>
public sealed partial class TranslatePage : TranslatePageBase, IInitializePage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TranslatePage"/> class.
    /// </summary>
    public TranslatePage() => InitializeComponent();

    public void Initialize()
        => ViewModel.InitializeCommand.Execute(default);

    /// <inheritdoc/>
    protected override void OnPageLoaded()
        => Initialize();
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