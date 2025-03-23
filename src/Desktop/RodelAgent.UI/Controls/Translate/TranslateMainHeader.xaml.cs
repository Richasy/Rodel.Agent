// Copyright (c) Richasy. All rights reserved.

using Richasy.WinUIKernel.AI.ViewModels;
using System.ComponentModel;

namespace RodelAgent.UI.Controls.Translate;

/// <summary>
/// 翻译页面的头部.
/// </summary>
public sealed partial class TranslateMainHeader : TranslatePageControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TranslateMainHeader"/> class.
    /// </summary>
    public TranslateMainHeader() => InitializeComponent();

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
        if ((sender as ComboBox)!.SelectedItem is TranslateServiceItemViewModel selectedItem && ViewModel.SelectedService != selectedItem)
        {
            ViewModel.SelectServiceCommand.Execute(selectedItem);
        }
    }
}
