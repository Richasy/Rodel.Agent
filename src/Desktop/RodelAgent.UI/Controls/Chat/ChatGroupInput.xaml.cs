// Copyright (c) Rodel. All rights reserved.

using Microsoft.UI.Input;
using RodelAgent.UI.ViewModels.Components;
using Windows.System;
using Windows.UI.Core;

namespace RodelAgent.UI.Controls.Chat;

/// <summary>
/// 群组会话输入.
/// </summary>
public sealed partial class ChatGroupInput : ChatGroupControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChatGroupInput"/> class.
    /// </summary>
    public ChatGroupInput() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnViewModelChanged(DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue is ChatGroupViewModel oldVm)
        {
            oldVm.RequestFocusInput -= OnRequestFocusInput;
        }

        if (e.NewValue is ChatGroupViewModel newVm)
        {
            newVm.RequestFocusInput += OnRequestFocusInput;
        }

        CheckEnterSendItem();
    }

    /// <inheritdoc/>
    protected override void OnControlLoaded()
        => CheckEnterSendItem();

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        if (ViewModel is not null)
        {
            ViewModel.RequestFocusInput -= OnRequestFocusInput;
        }
    }

    private void OnRequestFocusInput(object sender, EventArgs e)
        => InputBox.Focus(FocusState.Programmatic);

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
                await ViewModel.SendCommand.ExecuteAsync(default);
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

    private void OnCleanMessageButtonClick(object sender, RoutedEventArgs e)
        => CleanMessageTip.IsOpen = true;

    private void OnClearMessageActionButtonClick(TeachingTip sender, object args)
    {
        ViewModel.ClearMessageCommand.Execute(default);
        sender.IsOpen = false;
    }
}
