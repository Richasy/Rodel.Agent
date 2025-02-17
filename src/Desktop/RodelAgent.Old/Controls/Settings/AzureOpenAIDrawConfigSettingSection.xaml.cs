// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.ViewModels.Items;
using RodelDraw.Models.Client;
using System.Diagnostics;

namespace RodelAgent.UI.Controls.Settings;

/// <summary>
/// Azure OpenAI 配置设置部分.
/// </summary>
public sealed partial class AzureOpenAIDrawConfigSettingSection : DrawServiceConfigControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AzureOpenAIDrawConfigSettingSection"/> class.
    /// </summary>
    public AzureOpenAIDrawConfigSettingSection() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnViewModelChanged(DrawServiceItemViewModel? oldValue, DrawServiceItemViewModel? newValue)
    {
        if (newValue is not DrawServiceItemViewModel newVM)
        {
            return;
        }

        Logo.Provider = newVM.ProviderType.ToString();
        newVM.Config ??= new AzureOpenAIClientConfig();
        Debug.Assert(ViewModel.Config != null, "ViewModel.Config should not be null.");
        ViewModel.CheckCurrentConfig();
    }

    private void OnKeyBoxLoaded(object sender, RoutedEventArgs e)
    {
        KeyBox.Password = ViewModel.Config?.Key ?? string.Empty;
        EndpointBox.Text = (ViewModel.Config as ClientEndpointConfigBase)?.Endpoint ?? string.Empty;
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
}
