// Copyright (c) Rodel. All rights reserved.

using RodelAgent.UI.ViewModels.Items;
using RodelAudio.Models.Client;

namespace RodelAgent.UI.Controls.Startup;

/// <summary>
/// Azure OpenAI 配置部分.
/// </summary>
public sealed partial class AzureOpenAIAudioConfigSection : AudioServiceConfigControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AzureOpenAIAudioConfigSection"/> class.
    /// </summary>
    public AzureOpenAIAudioConfigSection() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnViewModelChanged(AudioServiceItemViewModel? oldValue, AudioServiceItemViewModel? newValue)
    {
        if (newValue is not AudioServiceItemViewModel newVM)
        {
            return;
        }

        newVM.Config ??= new AzureOpenAIClientConfig();
        ViewModel.CheckCurrentConfig();
    }
}
