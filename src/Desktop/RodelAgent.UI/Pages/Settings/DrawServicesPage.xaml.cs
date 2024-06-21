// Copyright (c) Rodel. All rights reserved.

using RodelAgent.UI.Controls;
using RodelAgent.UI.Controls.Settings;
using RodelAgent.UI.ViewModels.Items;
using RodelDraw.Models.Constants;

namespace RodelAgent.UI.Pages.Settings;

/// <summary>
/// 绘图服务页面.
/// </summary>
public sealed partial class DrawServicesPage : SettingsPageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DrawServicesPage"/> class.
    /// </summary>
    public DrawServicesPage() => InitializeComponent();

    /// <inheritdoc/>
    protected override async void OnPageLoaded()
    {
        await ViewModel.InitializeOnlineDrawServicesCommand.ExecuteAsync(default);
        foreach (var item in ViewModel.OnlineDrawServices)
        {
            var section = item.ProviderType switch
            {
                ProviderType.OpenAI => CreateForm<OpenAIDrawConfigSettingSection>(item),
                ProviderType.AzureOpenAI => CreateForm<AzureOpenAIDrawConfigSettingSection>(item),
                ProviderType.HunYuan => CreateForm<HunYuanDrawConfigSettingSection>(item),
                ProviderType.QianFan => CreateForm<QianFanDrawConfigSettingSection>(item),
                ProviderType.SparkDesk => CreateForm<SparkDeskDrawConfigSettingSection>(item),
                _ => default
            };

            if (section is not null)
            {
                RootContainer.Children.Add(section);
            }
        }
    }

    private static FrameworkElement CreateForm<TControl>(DrawServiceItemViewModel vm)
    {
        var control = Activator.CreateInstance<TControl>();
        if (control is DrawServiceConfigControlBase form)
        {
            form.ViewModel = vm;
        }

        return control as FrameworkElement;
    }
}
