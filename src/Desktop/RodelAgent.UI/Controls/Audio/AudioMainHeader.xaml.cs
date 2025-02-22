// Copyright (c) Richasy. All rights reserved.

using Richasy.WinUIKernel.AI.ViewModels;
using System.ComponentModel;

namespace RodelAgent.UI.Controls.Audio;

/// <summary>
/// 音频主页头部.
/// </summary>
public sealed partial class AudioMainHeader : AudioPageControlBase
{
    public AudioMainHeader() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        ViewModel.PropertyChanged += OnViewModelPropertyChanged;
        ServiceComboBox.SelectedItem = ViewModel.SelectedService;
    }

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
        => ViewModel.PropertyChanged -= OnViewModelPropertyChanged;

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ViewModel.SelectedService))
        {
            ServiceComboBox.SelectedItem = ViewModel.SelectedService;
        }
    }

    private void OnServiceSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if ((sender as ComboBox)!.SelectedItem is AudioServiceItemViewModel selectedItem && ViewModel.SelectedService != selectedItem)
        {
            ViewModel.SelectServiceCommand.Execute(selectedItem);
        }
    }
}
