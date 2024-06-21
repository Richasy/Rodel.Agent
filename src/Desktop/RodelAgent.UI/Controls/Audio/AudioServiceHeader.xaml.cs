// Copyright (c) Rodel. All rights reserved.

using RodelAgent.UI.ViewModels.Items;

namespace RodelAgent.UI.Controls.Audio;

/// <summary>
/// 音频服务页面控件基类.
/// </summary>
public sealed partial class AudioServiceHeader : AudioServicePageControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AudioServiceHeader"/> class.
    /// </summary>
    public AudioServiceHeader() => InitializeComponent();

    private void OnServiceSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var service = (sender as ComboBox)?.SelectedItem as AudioServiceItemViewModel;
        if (service == null || ViewModel.Session?.AudioService == service)
        {
            return;
        }

        ViewModel.SetSelectedAudioServiceCommand.Execute(service);
    }
}
