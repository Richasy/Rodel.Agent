// Copyright (c) Rodel. All rights reserved.

using RodelAgent.UI.Controls;
using RodelAgent.UI.Controls.Settings;
using RodelAgent.UI.ViewModels.Items;
using RodelTranslate.Models.Constants;

namespace RodelAgent.UI.Pages.Settings;

/// <summary>
/// 翻译服务页面.
/// </summary>
public sealed partial class TranslateServicesPage : SettingsPageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TranslateServicesPage"/> class.
    /// </summary>
    public TranslateServicesPage()
    {
        InitializeComponent();
    }

    /// <inheritdoc/>
    protected override async void OnPageLoaded()
    {
        await ViewModel.InitializeOnlineTranslateServicesCommand.ExecuteAsync(default);
        foreach (var item in ViewModel.OnlineTranslateServices)
        {
            var section = item.ProviderType switch
            {
                ProviderType.Baidu
                or ProviderType.Youdao => CreateForm<TranslateAppClientConfigSettingSection>(item),
                ProviderType.Ali => CreateForm<AliTranslateConfigSettingSection>(item),
                ProviderType.Azure => CreateForm<AzureTranslateConfigSettingSection>(item),
                ProviderType.Tencent => CreateForm<TencentTranslateConfigSettingSection>(item),
                ProviderType.Volcano => CreateForm<VolcanoTranslateConfigSettingSection>(item),
                _ => default
            };

            if (section is not null)
            {
                RootContainer.Children.Add(section);
            }
        }
    }

    private static FrameworkElement CreateForm<TControl>(TranslateServiceItemViewModel vm)
    {
        var control = Activator.CreateInstance<TControl>();
        if (control is TranslateServiceConfigControlBase form)
        {
            form.ViewModel = vm;
        }

        return control as FrameworkElement;
    }
}
