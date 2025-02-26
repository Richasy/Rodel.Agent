// Copyright (c) Richasy. All rights reserved.

using Microsoft.UI.Input;
using Windows.System;
using Windows.UI.Core;

namespace RodelAgent.UI.Controls.Chat;

/// <summary>
/// Chat session footer.
/// </summary>
public sealed partial class ChatSessionMainFooter : ChatSessionControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChatSessionMainFooter"/> class.
    /// </summary>
    public ChatSessionMainFooter() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        CheckEnterSendItem();
        ViewModel.RequestFocusInput += OnRequestFocusInput;
        ViewModel.RequestCloseFlyout += OnRequestCloseFlyout;
    }

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        ViewModel.RequestFocusInput -= OnRequestFocusInput;
        ViewModel.RequestCloseFlyout -= OnRequestCloseFlyout;
    }

    private void OnRequestFocusInput(object? sender, EventArgs? e)
    {
        CloseFlyout();
        InputBox.Focus(FocusState.Programmatic);
    }

    private void OnRequestCloseFlyout(object? sender, EventArgs e)
        => CloseFlyout();

    private void CloseFlyout()
    {
        if (ModelFlyout.IsOpen)
        {
            ModelFlyout.Hide();
        }
    }

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
                await ViewModel.StartGenerateCommand.ExecuteAsync(default);
                // ViewModel.CheckRegenerateButtonShownCommand.Execute(default);
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
        _ = this;
        sender.IsOpen = false;
    }

    private void OnCleanMessageTipClosed(TeachingTip sender, TeachingTipClosedEventArgs args)
        => InputBox.Focus(FocusState.Programmatic);
}
