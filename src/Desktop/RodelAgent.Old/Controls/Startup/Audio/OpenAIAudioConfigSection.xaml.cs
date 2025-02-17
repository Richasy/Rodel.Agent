// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.ViewModels.Items;
using RodelAudio.Models.Client;
using System.Diagnostics;

namespace RodelAgent.UI.Controls.Startup;

/// <summary>
/// OpenAI 配置部分.
/// </summary>
public sealed partial class OpenAIAudioConfigSection : AudioServiceConfigControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OpenAIAudioConfigSection"/> class.
    /// </summary>
    public OpenAIAudioConfigSection() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnViewModelChanged(AudioServiceItemViewModel? oldValue, AudioServiceItemViewModel? newValue)
    {
        if (newValue is not AudioServiceItemViewModel newVM)
        {
            return;
        }

        newVM.Config ??= CreateCurrentConfig();
        if (newVM.Config is OpenAIClientConfig config && string.IsNullOrEmpty(config.Endpoint))
        {
            config.Endpoint = ProviderConstants.OpenAIApi;
        }

        Debug.Assert(ViewModel.Config != null, "ViewModel.Config should not be null.");
        ViewModel.CheckCurrentConfig();
    }

    /// <inheritdoc/>
    protected override void OnControlLoaded()
        => OrganizationBox.Text = ((OpenAIClientConfig)ViewModel.Config).OrganizationId;

    private static OpenAIClientConfig CreateCurrentConfig()
    {
        var config = new OpenAIClientConfig
        {
            Endpoint = ProviderConstants.OpenAIApi,
            OrganizationId = string.Empty,
        };
        return config;
    }

    private void OnOrganizationBoxTextChanged(object sender, TextChangedEventArgs e)
    {
        ((OpenAIClientConfig)ViewModel.Config).OrganizationId = OrganizationBox.Text;
        ViewModel.CheckCurrentConfig();
    }
}
