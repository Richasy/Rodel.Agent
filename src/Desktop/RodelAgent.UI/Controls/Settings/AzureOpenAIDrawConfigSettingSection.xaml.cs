// Copyright (c) Rodel. All rights reserved.

using System.Diagnostics;
using System.Text.Json;
using RodelAgent.Models.Constants;
using RodelAgent.UI.ViewModels.Items;
using RodelDraw.Models.Client;

namespace RodelAgent.UI.Controls.Settings;

/// <summary>
/// Azure OpenAI 配置设置部分.
/// </summary>
public sealed partial class AzureOpenAIDrawConfigSettingSection : DrawServiceConfigControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AzureOpenAIDrawConfigSettingSection"/> class.
    /// </summary>
    public AzureOpenAIDrawConfigSettingSection()
    {
        InitializeComponent();
        InitializeApiVersion();
    }

    /// <inheritdoc/>
    protected override void OnViewModelChanged(DependencyPropertyChangedEventArgs e)
    {
        var newVM = e.NewValue as DrawServiceItemViewModel;
        if (newVM == null)
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
        VersionComboBox.SelectedIndex = (int)((AzureOpenAIClientConfig)ViewModel.Config).Version;
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

    private void InitializeApiVersion()
    {
        var versions = Enum.GetValues<AzureOpenAIVersion>();
        var json = JsonSerializer.Serialize(versions);
        var stringVersions = JsonSerializer.Deserialize<List<string>>(json);
        VersionComboBox.ItemsSource = stringVersions;
    }

    private void OnVersionChanged(object sender, SelectionChangedEventArgs e)
    {
        var index = VersionComboBox.SelectedIndex;
        if (index < 0)
        {
            return;
        }

        var version = (AzureOpenAIVersion)index;
        ((AzureOpenAIClientConfig)ViewModel.Config).Version = version;
    }
}
