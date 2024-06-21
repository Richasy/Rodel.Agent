// Copyright (c) Rodel. All rights reserved.

using Microsoft.UI.Input;
using Windows.System;
using Windows.UI.Core;

namespace RodelAgent.UI.Controls.Draw;

/// <summary>
/// 绘图会话输入控件.
/// </summary>
public sealed partial class DrawSessionInput : DrawSessionControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DrawSessionInput"/> class.
    /// </summary>
    public DrawSessionInput()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
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
                await ViewModel.DrawCommand.ExecuteAsync(default);
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
}
