// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.ViewModels.Items;
using RodelTranslate.Models.Constants;

namespace RodelAgent.UI.Controls.Startup;

/// <summary>
/// 聊天配置部分.
/// </summary>
public sealed partial class TranslateConfigurationSection : TranslateServiceConfigControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TranslateConfigurationSection"/> class.
    /// </summary>
    public TranslateConfigurationSection()
    {
        InitializeComponent();
    }

    /// <inheritdoc/>
    protected override void OnViewModelChanged(TranslateServiceItemViewModel? oldValue, TranslateServiceItemViewModel? newValue)
    {
        if (newValue is not TranslateServiceItemViewModel newVM)
        {
            return;
        }

        var formControl = newVM.ProviderType switch
        {
            ProviderType.Baidu
            or ProviderType.Youdao => CreateForm<TranslateAppClientConfigSection>(),
            ProviderType.Azure => CreateForm<AzureTranslateConfigSection>(),
            ProviderType.Ali => CreateForm<AliTranslateConfigSection>(),
            ProviderType.Tencent => CreateForm<TencentTranslateConfigSection>(),
            ProviderType.Volcano => CreateForm<VolcanoTranslateConfigSection>(),
            _ => default,
        };

        if (formControl is not null)
        {
            FormPresenter.Content = formControl;
        }
    }

    private DependencyObject CreateForm<TControl>()
    {
        var control = Activator.CreateInstance<TControl>();
        if (control is TranslateServiceConfigControlBase form)
        {
            form.ViewModel = ViewModel;
        }

        return control as DependencyObject;
    }
}
