// Copyright (c) Richasy. All rights reserved.

using Microsoft.UI.Input;
using Windows.System;
using Windows.UI.Core;

namespace RodelAgent.UI.Controls.Audio;

/// <summary>
/// 音频会话输入控件.
/// </summary>
public sealed partial class AudioSessionInput : AudioSessionControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AudioSessionInput"/> class.
    /// </summary>
    public AudioSessionInput() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnControlLoaded()
        => CheckEnterSendItem();

    private async void OnInputBoxPreviewKeyDownAsync(object sender, KeyRoutedEventArgs e)
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
                await ViewModel.GenerateCommand.ExecuteAsync(default);
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

    private void OnModelClick(object sender, ViewModels.Items.AudioModelItemViewModel e)
    {
        ViewModel.ChangeModelCommand.Execute(e);
        ModelFlyout.Hide();
    }

    private void OnLanguageItemClick(object sender, RoutedEventArgs e)
    {
        ViewModel.ChangeLanguageCommand.Execute((sender as FrameworkElement)?.DataContext as ViewModels.Items.AudioLanguageViewModel);
        LanguageFlyout.Hide();
    }

    private void OnVoiceItemClick(object sender, RoutedEventArgs e)
    {
        ViewModel.ChangeVoiceCommand.Execute((sender as FrameworkElement)?.DataContext as ViewModels.Items.AudioVoiceViewModel);
        VoiceFlyout.Hide();
    }
}
