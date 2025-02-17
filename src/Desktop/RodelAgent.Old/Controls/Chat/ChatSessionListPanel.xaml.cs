// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.ViewModels.Components;

namespace RodelAgent.UI.Controls.Chat;

/// <summary>
/// 聊天会话列表面板.
/// </summary>
public sealed partial class ChatSessionListPanel : ChatServicePageControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChatSessionListPanel"/> class.
    /// </summary>
    public ChatSessionListPanel() => InitializeComponent();

    private void OnItemClick(object sender, ViewModels.Components.ChatSessionViewModel e)
        => ViewModel.SetSelectedSessionCommand.Execute(e);

    private async void OnRenameItemClickAsync(object sender, RoutedEventArgs e)
    {
        var context = (sender as FrameworkElement)?.DataContext as ChatSessionViewModel;
        var dialog = new SessionRenameDialog(context);
        await dialog.ShowAsync();
    }

    private void OnDeleteItemClick(object sender, RoutedEventArgs e)
    {
        var context = (sender as FrameworkElement)?.DataContext as ChatSessionViewModel;
        ViewModel.RemoveSessionCommand.Execute(context);
    }
}
