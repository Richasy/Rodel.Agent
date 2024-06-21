// Copyright (c) Rodel. All rights reserved.

using System.Reflection;
using RodelAgent.UI.Toolkits;
using RodelAgent.UI.ViewModels.Items;
using RodelTranslate.Models.Client;

namespace RodelAgent.UI.Controls.Settings;

/// <summary>
/// 翻译应用客户端配置设置部分.
/// </summary>
public sealed partial class TranslateAppClientConfigSettingSection : TranslateServiceConfigControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TranslateAppClientConfigSettingSection"/> class.
    /// </summary>
    public TranslateAppClientConfigSettingSection() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnViewModelChanged(DependencyPropertyChangedEventArgs e)
    {
        var newVM = e.NewValue as TranslateServiceItemViewModel;
        if (newVM is null)
        {
            return;
        }

        KeyCard.Description = string.Format(ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.AccessKeyDescription), newVM.Name);

        newVM.Config ??= CreateCurrentConfig();
        ViewModel.CheckCurrentConfig();
        if (newVM.ProviderType == RodelTranslate.Models.Constants.ProviderType.Baidu)
        {
            Logo.Height = 32;
        }
    }

    private void OnKeyBoxLoaded(object sender, RoutedEventArgs e)
    {
        KeyBox.Password = ViewModel.Config?.Key ?? string.Empty;
        AppIdBox.Text = (ViewModel.Config as AppClientConfigBase)?.AppId ?? string.Empty;
        KeyBox.Focus(FocusState.Programmatic);
    }

    private void OnKeyBoxPasswordChanged(object sender, RoutedEventArgs e)
    {
        ViewModel.Config.Key = KeyBox.Password;
        ViewModel.CheckCurrentConfig();
    }

    private void OnAppIdBoxTextChanged(object sender, TextChangedEventArgs e)
    {
        ((AppClientConfigBase)ViewModel.Config).AppId = AppIdBox.Text;
        ViewModel.CheckCurrentConfig();
    }

    private ClientConfigBase CreateCurrentConfig()
    {
        var assembly = Assembly.GetAssembly(typeof(ClientConfigBase));
        var types = assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(ClientConfigBase)));

        foreach (var type in types)
        {
            if (type.Name.StartsWith(ViewModel.ProviderType.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                return (ClientConfigBase)Activator.CreateInstance(type);
            }
        }

        return null;
    }
}
