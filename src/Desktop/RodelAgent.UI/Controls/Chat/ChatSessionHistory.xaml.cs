// Copyright (c) Rodel. All rights reserved.

using RodelAgent.UI.ViewModels.Components;

namespace RodelAgent.UI.Controls.Chat;

/// <summary>
/// 聊天会话历史.
/// </summary>
public sealed partial class ChatSessionHistory : ChatSessionControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChatSessionHistory"/> class.
    /// </summary>
    public ChatSessionHistory() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnViewModelChanged(ChatSessionViewModel? oldValue, ChatSessionViewModel? newValue)
    {
        if (oldValue is ChatSessionViewModel oldVm)
        {
            oldVm.RequestScrollToBottom -= OnRequestScrollToBottomAsync;
        }

        if (newValue is ChatSessionViewModel newVm)
        {
            newVm.RequestScrollToBottom += OnRequestScrollToBottomAsync;
        }
    }

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        if (ViewModel is not null)
        {
            ViewModel.RequestScrollToBottom -= OnRequestScrollToBottomAsync;
        }
    }

    private async void OnRequestScrollToBottomAsync(object sender, EventArgs e)
    {
        if (MessageViewer is not null)
        {
            await Task.Delay(200);
            MessageViewer.ChangeView(0, MessageViewer.ScrollableHeight + MessageViewer.ActualHeight + MessageViewer.VerticalOffset, default);
        }
    }
}
