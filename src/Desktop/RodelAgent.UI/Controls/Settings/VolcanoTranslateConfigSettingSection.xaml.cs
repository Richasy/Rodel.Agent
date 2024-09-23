// Copyright (c) Rodel. All rights reserved.

using RodelAgent.UI.ViewModels.Items;
using RodelTranslate.Models.Client;

namespace RodelAgent.UI.Controls.Settings;

/// <summary>
/// 翻译应用客户端配置设置部分.
/// </summary>
public sealed partial class VolcanoTranslateConfigSettingSection : TranslateServiceConfigControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VolcanoTranslateConfigSettingSection"/> class.
    /// </summary>
    public VolcanoTranslateConfigSettingSection() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnViewModelChanged(TranslateServiceItemViewModel? oldValue, TranslateServiceItemViewModel? newValue)
    {
        if (newValue is not TranslateServiceItemViewModel newVM)
        {
            return;
        }

        newVM.Config ??= new VolcanoClientConfig();
        ViewModel.CheckCurrentConfig();
    }

    private void OnKeyBoxLoaded(object sender, RoutedEventArgs e)
    {
        KeyBox.Password = ViewModel.Config?.Key ?? string.Empty;
        IdBox.Text = (ViewModel.Config as VolcanoClientConfig)?.KeyId ?? string.Empty;
        KeyBox.Focus(FocusState.Programmatic);
    }

    private void OnKeyBoxPasswordChanged(object sender, RoutedEventArgs e)
    {
        ViewModel.Config.Key = KeyBox.Password;
        ViewModel.CheckCurrentConfig();
    }

    private void OnIdBoxTextChanged(object sender, RoutedEventArgs e)
    {
        ((VolcanoClientConfig)ViewModel.Config).KeyId = IdBox.Text;
        ViewModel.CheckCurrentConfig();
    }
}
