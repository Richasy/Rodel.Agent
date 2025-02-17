// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.ViewModels.Items;

namespace RodelAgent.UI.Pages.Startup;

/// <summary>
/// 翻译配置页面.
/// </summary>
public sealed partial class TranslateConfigurationPage : TranslateConfigurationPageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TranslateConfigurationPage"/> class.
    /// </summary>
    public TranslateConfigurationPage() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnNavigatedTo(NavigationEventArgs e)
        => ViewModel = e.Parameter as TranslateServiceItemViewModel;

    /// <inheritdoc/>
    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        ViewModel.CheckCurrentConfig();
        ViewModel = default;
    }
}

/// <summary>
/// 聊天配置页面基类.
/// </summary>
public abstract class TranslateConfigurationPageBase : LayoutPageBase<TranslateServiceItemViewModel>
{
}
