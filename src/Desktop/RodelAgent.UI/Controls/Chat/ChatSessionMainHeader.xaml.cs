// Copyright (c) Richasy. All rights reserved.

using Richasy.WinUIKernel.AI.ViewModels;
using System.ComponentModel;

namespace RodelAgent.UI.Controls.Chat;

/// <summary>
/// Chat session main header.
/// </summary>
public sealed partial class ChatSessionMainHeader : ChatSessionControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChatSessionMainHeader"/> class.
    /// </summary>
    public ChatSessionMainHeader() => InitializeComponent();

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
        if ((sender as ComboBox)!.SelectedItem is ChatServiceItemViewModel selectedItem && ViewModel.SelectedService != selectedItem)
        {
            ViewModel.ChangeServiceCommand.Execute(selectedItem);
        }
    }
}
