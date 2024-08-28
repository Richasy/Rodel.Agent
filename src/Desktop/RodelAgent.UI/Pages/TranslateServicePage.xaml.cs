// Copyright (c) Rodel. All rights reserved.

using RodelAgent.UI.ViewModels.Pages;

namespace RodelAgent.UI.Pages;

/// <summary>
/// 翻译页面.
/// </summary>
public sealed partial class TranslateServicePage : TranslateServicePageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TranslateServicePage"/> class.
    /// </summary>
    public TranslateServicePage()
    {
        InitializeComponent();
        ViewModel = this.Get<TranslateServicePageViewModel>();
    }

    /// <inheritdoc/>
    protected override void OnPageLoaded()
    {
        ViewModel.InitializeBasisCommand.Execute(default);
        if (ViewModel.IsAvailableServicesEmpty)
        {
            ViewModel.ResetAvailableTranslateServicesCommand.Execute(default);
        }
    }
}

/// <summary>
/// 翻译页面基类.
/// </summary>
public abstract class TranslateServicePageBase : LayoutPageBase<TranslateServicePageViewModel>
{
}
