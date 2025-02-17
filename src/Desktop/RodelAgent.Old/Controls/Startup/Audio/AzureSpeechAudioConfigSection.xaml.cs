// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.ViewModels.Items;
using RodelAudio.Models.Client;

namespace RodelAgent.UI.Controls.Startup;

/// <summary>
/// Azure 语音的客户端配置部分.
/// </summary>
public sealed partial class AzureSpeechAudioConfigSection : AudioServiceConfigControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AzureSpeechAudioConfigSection"/> class.
    /// </summary>
    public AzureSpeechAudioConfigSection() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnViewModelChanged(AudioServiceItemViewModel? oldValue, AudioServiceItemViewModel? newValue)
    {
        if (newValue is not AudioServiceItemViewModel newVM)
        {
            return;
        }

        newVM.Config ??= new AzureSpeechClientConfig();
        ViewModel.CheckCurrentConfig();
    }

    /// <inheritdoc/>
    protected override void OnControlLoaded()
        => RegionBox.Text = (ViewModel.Config as AzureSpeechClientConfig)?.Region ?? string.Empty;

    private void OnRegionBoxTextChanged(object sender, TextChangedEventArgs e)
    {
        ((AzureSpeechClientConfig)ViewModel.Config).Region = RegionBox.Text;
        ViewModel.CheckCurrentConfig();
    }
}
