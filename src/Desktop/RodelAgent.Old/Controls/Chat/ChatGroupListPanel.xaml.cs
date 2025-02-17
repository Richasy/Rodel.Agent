// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.ViewModels.Components;

namespace RodelAgent.UI.Controls.Chat;

/// <summary>
/// 群组会话列表面板.
/// </summary>
public sealed partial class ChatGroupListPanel : ChatServicePageControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChatGroupListPanel"/> class.
    /// </summary>
    public ChatGroupListPanel() => InitializeComponent();

    private void OnItemClick(object sender, ViewModels.Components.ChatGroupViewModel e)
        => ViewModel.SetSelectedGroupSessionCommand.Execute(e);

    private async void OnRenameItemClickAsync(object sender, RoutedEventArgs e)
    {
        var context = (sender as FrameworkElement)?.DataContext as ChatGroupViewModel;
        var dialog = new SessionRenameDialog(context);
        await dialog.ShowAsync();
    }

    private void OnDeleteItemClick(object sender, RoutedEventArgs e)
    {
        var context = (sender as FrameworkElement)?.DataContext as ChatGroupViewModel;
        ViewModel.RemoveGroupCommand.Execute(context);
    }
}
