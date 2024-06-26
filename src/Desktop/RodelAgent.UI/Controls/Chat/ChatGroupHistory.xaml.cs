// Copyright (c) Rodel. All rights reserved.

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
    public ChatGroupHistory()
    {
        InitializeComponent();
        Unloaded += OnUnloaded;
    }

    /// <inheritdoc/>
    protected override void OnViewModelChanged(DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue is ChatGroupViewModel oldVm)
        {
            oldVm.RequestScrollToBottom -= OnRequestScrollToBottomAsync;
        }

        if (e.NewValue is ChatGroupViewModel newVm)
        {
            newVm.RequestScrollToBottom += OnRequestScrollToBottomAsync;
        }
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
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
