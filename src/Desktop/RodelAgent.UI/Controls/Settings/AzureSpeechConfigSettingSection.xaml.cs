// Copyright (c) Rodel. All rights reserved.

using RodelAgent.UI.ViewModels.Items;
using RodelAudio.Models.Client;

namespace RodelAgent.UI.Controls.Settings;

/// <summary>
/// Azure 语音配置设置部分.
/// </summary>
public sealed partial class AzureSpeechConfigSettingSection : AudioServiceConfigControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AzureSpeechConfigSettingSection"/> class.
    /// </summary>
    public AzureSpeechConfigSettingSection() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnViewModelChanged(DependencyPropertyChangedEventArgs e)
    {
        var newVM = e.NewValue as AudioServiceItemViewModel;
        if (newVM is null)
        {
            return;
        }

        newVM.Config ??= new AzureSpeechClientConfig();
        ViewModel.CheckCurrentConfig();
    }

    private void OnKeyBoxLoaded(object sender, RoutedEventArgs e)
    {
        KeyBox.Password = ViewModel.Config?.Key ?? string.Empty;
        RegionBox.Text = (ViewModel.Config as AzureSpeechClientConfig)?.Region ?? string.Empty;
        KeyBox.Focus(FocusState.Programmatic);
    }

    private void OnKeyBoxPasswordChanged(object sender, RoutedEventArgs e)
    {
        ViewModel.Config.Key = KeyBox.Password;
        ViewModel.CheckCurrentConfig();
    }

    private void OnRegionBoxTextChanged(object sender, TextChangedEventArgs e)
    {
        ((AzureSpeechClientConfig)ViewModel.Config).Region = RegionBox.Text;
        ViewModel.CheckCurrentConfig();
    }
}
