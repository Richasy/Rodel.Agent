// Copyright (c) Rodel. All rights reserved.

using Microsoft.UI.Input;
using RodelAgent.UI.ViewModels.Components;
using Windows.System;
using Windows.UI.Core;

namespace RodelAgent.UI.Controls.Chat;

/// <summary>
/// 聊天会话输入.
/// </summary>
public sealed partial class ChatSessionInput : ChatSessionControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChatSessionInput"/> class.
    /// </summary>
    public ChatSessionInput()
    {
        InitializeComponent();
        ImageButton.Visibility = GlobalFeatureSwitcher.IsChatImageSupported ? Visibility.Visible : Visibility.Collapsed;
    }

    /// <inheritdoc/>
    protected override void OnViewModelChanged(DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue is ChatSessionViewModel oldVm)
        {
            oldVm.RequestFocusInput -= OnRequestFocusInput;
        }

        if (e.NewValue is ChatSessionViewModel newVm)
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
    {
        if (ModelFlyout.IsOpen)
        {
            ModelFlyout.Hide();
        }

        InputBox.Focus(FocusState.Programmatic);
    }

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
                ViewModel.CheckRegenerateButtonShownCommand.Execute(default);
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
