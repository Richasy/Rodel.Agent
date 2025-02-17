// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.Toolkits;
using RodelAgent.UI.ViewModels.Items;
using RodelAudio.Models.Client;

namespace RodelAgent.UI.Controls.Settings;

/// <summary>
/// OpenAI 配置设置部分.
/// </summary>
public sealed partial class OpenAIAudioConfigSettingSection : AudioServiceConfigControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OpenAIAudioConfigSettingSection"/> class.
    /// </summary>
    public OpenAIAudioConfigSettingSection() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnViewModelChanged(AudioServiceItemViewModel? oldValue, AudioServiceItemViewModel? newValue)
    {
        if (newValue is not AudioServiceItemViewModel newVM)
        {
            return;
        }

        Logo.Provider = newVM.ProviderType.ToString();
        KeyCard.Description = string.Format(ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.AccessKeyDescription), newVM.Name);
        KeyBox.PlaceholderText = string.Format(ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.AccessKeyPlaceholder), newVM.Name);
        PredefinedCard.Description = string.Format(ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.PredefinedModelsDescription), newVM.Name);

        newVM.Config ??= new OpenAIClientConfig() { Endpoint = ProviderConstants.OpenAIApi };
        if (newVM.Config is OpenAIClientConfig config && string.IsNullOrEmpty(config.Endpoint))
        {
            config.Endpoint = ProviderConstants.OpenAIApi;
        }

        ViewModel.CheckCurrentConfig();
    }

    private void OnKeyBoxLoaded(object sender, RoutedEventArgs e)
    {
        KeyBox.Password = ViewModel.Config?.Key ?? string.Empty;
        EndpointBox.Text = (ViewModel.Config as ClientEndpointConfigBase)?.Endpoint ?? string.Empty;
        OrganizationBox.Text = (ViewModel.Config as OpenAIClientConfig)?.OrganizationId ?? string.Empty;
        KeyBox.Focus(FocusState.Programmatic);
    }

    private void OnKeyBoxPasswordChanged(object sender, RoutedEventArgs e)
    {
        ViewModel.Config.Key = KeyBox.Password;
        ViewModel.CheckCurrentConfig();
    }

    private void OnEndpointBoxTextChanged(object sender, TextChangedEventArgs e)
    {
        ((ClientEndpointConfigBase)ViewModel.Config).Endpoint = EndpointBox.Text;
        ViewModel.CheckCurrentConfig();
    }

    private void OnOrganizationBoxTextChanged(object sender, TextChangedEventArgs e)
        => ((OpenAIClientConfig)ViewModel.Config).OrganizationId = OrganizationBox.Text;

    private void OnPredefinedModelsButtonClick(object sender, RoutedEventArgs e)
        => FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
}
