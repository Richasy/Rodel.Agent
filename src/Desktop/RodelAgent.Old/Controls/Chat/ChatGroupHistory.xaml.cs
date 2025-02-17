// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.ViewModels.Components;

namespace RodelAgent.UI.Controls.Chat;

/// <summary>
/// 聊天会话历史.
/// </summary>
public sealed partial class ChatGroupHistory : ChatGroupControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChatGroupHistory"/> class.
    /// </summary>
    public ChatGroupHistory() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnViewModelChanged(ChatGroupViewModel? oldValue, ChatGroupViewModel? newValue)
    {
        if (oldValue is ChatGroupViewModel oldVm)
        {
            oldVm.RequestScrollToBottom -= OnRequestScrollToBottomAsync;
        }

        if (newValue is ChatGroupViewModel newVm)
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
