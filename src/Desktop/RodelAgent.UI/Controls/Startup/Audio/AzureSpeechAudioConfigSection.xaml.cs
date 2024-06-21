// Copyright (c) Rodel. All rights reserved.

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
    public AzureSpeechAudioConfigSection()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    /// <inheritdoc/>
    protected override void OnViewModelChanged(DependencyPropertyChangedEventArgs e)
    {
        var newVM = e.NewValue as AudioServiceItemViewModel;
        if (newVM == null)
        {
            return;
        }

        newVM.Config ??= new AzureSpeechClientConfig();
        ViewModel.CheckCurrentConfig();
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        RegionBox.Text = (ViewModel.Config as AzureSpeechClientConfig)?.Region ?? string.Empty;
    }

    private void OnRegionBoxTextChanged(object sender, TextChangedEventArgs e)
    {
        ((AzureSpeechClientConfig)ViewModel.Config).Region = RegionBox.Text;
        ViewModel.CheckCurrentConfig();
    }
}
