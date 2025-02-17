// Copyright (c) Richasy. All rights reserved.

using Microsoft.UI.Input;
using RodelChat.Models.Client;
using RodelChat.Models.Constants;
using Windows.System;
using Windows.UI.Core;

namespace RodelAgent.UI.Controls.Chat;

/// <summary>
/// 预设详情面板.
/// </summary>
public sealed partial class PresetDetailPanel : ChatPresetControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PresetDetailPanel"/> class.
    /// </summary>
    public PresetDetailPanel()
    {
        InitializeComponent();
    }

    private void OnInputBoxPreviewKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == VirtualKey.Enter)
        {
            var shiftState = InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Shift);
            var isShiftDown = shiftState == CoreVirtualKeyStates.Down || shiftState == (CoreVirtualKeyStates.Down | CoreVirtualKeyStates.Locked);

            if (!isShiftDown)
            {
                e.Handled = true;
                var currentRole = RoleSelection.SelectedIndex == 0 ? MessageRole.User : MessageRole.Assistant;
                var currentContent = MessageInput.Text;
                if (string.IsNullOrEmpty(currentContent))
                {
                    return;
                }

                var message = new ChatMessage
                {
                    Role = currentRole,
                    Content = new List<ChatMessageContent>
                    {
                        new ChatMessageContent
                        {
                            Type = ChatContentType.Text,
                            Text = currentContent,
                        },
                    },
                };

                ViewModel.AddMessageCommand.Execute(message);
                MessageInput.Text = string.Empty;
                RoleSelection.SelectedIndex = currentRole == MessageRole.User ? 1 : 0;
            }
        }
    }
}
