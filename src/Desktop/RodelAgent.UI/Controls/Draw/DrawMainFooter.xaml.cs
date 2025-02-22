// Copyright (c) Richasy. All rights reserved.

using Microsoft.UI.Input;
using Richasy.WinUIKernel.AI.ViewModels;
using Windows.System;
using Windows.UI.Core;

namespace RodelAgent.UI.Controls.Draw;

/// <summary>
/// 绘图页面底部.
/// </summary>
public sealed partial class DrawMainFooter : DrawPageControlBase
{
    public DrawMainFooter() => InitializeComponent();

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
                await ViewModel.StartDrawCommand.ExecuteAsync(default);
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

    private void OnDrawModelChanged(object sender, SelectionChangedEventArgs e)
    {
        if (DrawModelComboBox.SelectedItem is DrawModelItemViewModel model && ViewModel.SelectedModel != model)
        {
            ViewModel.SelectModelCommand.Execute(model);
        }
    }
}
