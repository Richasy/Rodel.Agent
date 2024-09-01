// Copyright (c) Rodel. All rights reserved.

using System.Diagnostics;
using RodelAgent.UI.Toolkits;
using RodelAgent.UI.ViewModels.Items;
using RodelDraw.Models.Client;

namespace RodelAgent.UI.Controls.Settings;

/// <summary>
/// 百度千帆配置设置部分.
/// </summary>
public sealed partial class SparkDeskDrawConfigSettingSection : DrawServiceConfigControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SparkDeskDrawConfigSettingSection"/> class.
    /// </summary>
    public SparkDeskDrawConfigSettingSection() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnViewModelChanged(DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is not DrawServiceItemViewModel newVM)
        {
            return;
        }

        Logo.Provider = newVM.ProviderType.ToString();
        KeyCard.Description = string.Format(ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.AccessKeyDescription), newVM.Name);
        KeyBox.PlaceholderText = string.Format(ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.AccessKeyPlaceholder), newVM.Name);
        PredefinedCard.Description = string.Format(ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.PredefinedModelsDescription), newVM.Name);

        newVM.Config ??= new SparkDeskClientConfig();
        Debug.Assert(ViewModel.Config != null, "ViewModel.Config should not be null.");
        ViewModel.CheckCurrentConfig();
    }

    private void OnKeyBoxLoaded(object sender, RoutedEventArgs e)
    {
        KeyBox.Password = ViewModel.Config?.Key ?? string.Empty;
        SecretBox.Password = (ViewModel.Config as SparkDeskClientConfig)?.Secret ?? string.Empty;
        AppIdBox.Text = (ViewModel.Config as SparkDeskClientConfig)?.AppId ?? string.Empty;
        KeyBox.Focus(FocusState.Programmatic);
    }

    private void OnKeyBoxPasswordChanged(object sender, RoutedEventArgs e)
    {
        ViewModel.Config.Key = KeyBox.Password;
        ViewModel.CheckCurrentConfig();
    }

    private void OnPredefinedModelsButtonClick(object sender, RoutedEventArgs e)
        => FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);

    private void OnAppIdBoxTextChanged(object sender, TextChangedEventArgs e)
    {
        ((SparkDeskClientConfig)ViewModel.Config).AppId = AppIdBox.Text;
        ViewModel.CheckCurrentConfig();
    }

    private void OnSecretBoxTextChanged(object sender, RoutedEventArgs e)
    {
        ((SparkDeskClientConfig)ViewModel.Config).Secret = SecretBox.Password;
        ViewModel.CheckCurrentConfig();
    }
}
