// Copyright (c) Richasy. All rights reserved.

using Microsoft.UI.Input;
using Richasy.WinUIKernel.AI.ViewModels;
using RodelAgent.UI.ViewModels.Items;
using Windows.System;
using Windows.UI.Core;

namespace RodelAgent.UI.Controls.Audio;

/// <summary>
/// 音频主页底部.
/// </summary>
public sealed partial class AudioMainFooter : AudioPageControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AudioMainFooter"/> class.
    /// </summary>
    public AudioMainFooter() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnControlLoaded()
        => CheckEnterSendItem();

    private async void OnInputBoxPreviewKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == VirtualKey.Enter)
        {
            var shiftState = InputKeyboardSource.GetKeyStateForCurrentThread(Windows.System.VirtualKey.Shift);
            var isShiftDown = shiftState == CoreVirtualKeyStates.Down || shiftState == (CoreVirtualKeyStates.Down | CoreVirtualKeyStates.Locked);
            var ctrlState = InputKeyboardSource.GetKeyStateForCurrentThread(Windows.System.VirtualKey.Control);
            var isCtrlDown = ctrlState == CoreVirtualKeyStates.Down || ctrlState == (CoreVirtualKeyStates.Down | CoreVirtualKeyStates.Locked);

            if ((ViewModel.IsEnterSend && !isShiftDown)
                || (!ViewModel.IsEnterSend && isCtrlDown))
            {
                e.Handled = true;
                await ViewModel.StartAudioCommand.ExecuteAsync(default);
            }
        }
    }

    private void OnCtrlEnterSendItemClick(object sender, RoutedEventArgs e)
    {
        ViewModel.IsEnterSend = false;
        CheckEnterSendItem();
    }

    private void OnEnterSendItemClick(object sender, RoutedEventArgs e)
    {
        ViewModel.IsEnterSend = true;
        CheckEnterSendItem();
    }

    private void CheckEnterSendItem()
    {
        if (EnterSendItem != null && ViewModel != null)
        {
            EnterSendItem.IsChecked = ViewModel.IsEnterSend;
            CtrlEnterSendItem.IsChecked = !ViewModel.IsEnterSend;
        }
    }

    private void OnAudioModelChanged(object sender, SelectionChangedEventArgs e)
    {
        if (AudioModelComboBox.SelectedItem is AudioModelItemViewModel model && ViewModel.SelectedModel != model)
        {
            ViewModel.SelectModelCommand.Execute(model);
        }
    }

    private void OnAudioLanguageChanged(object sender, SelectionChangedEventArgs e)
    {
        if (AudioLanguageComboBox.SelectedItem is LanguageItemViewModel language && ViewModel.SelectedLanguage != language)
        {
            ViewModel.SelectLanguageCommand.Execute(language);
        }
    }
}
