// Copyright (c) Rodel. All rights reserved.

namespace RodelAgent.UI.Controls.Chat;

/// <summary>
/// 群组会话头部.
/// </summary>
public sealed partial class ChatGroupHeader : ChatGroupControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChatGroupHeader"/> class.
    /// </summary>
    public ChatGroupHeader()
    {
        InitializeComponent();
        ShareButton.Visibility = GlobalFeatureSwitcher.IsChatShareEnabled ? Visibility.Visible : Visibility.Collapsed;
    }

    private void ShowRename()
    {
        TitleContainer.Visibility = Visibility.Collapsed;
        RenameBox.Visibility = Visibility.Visible;
        RenameBox.Text = ViewModel.Data.Title ?? string.Empty;
        RenameBox.Focus(FocusState.Programmatic);
    }

    private void HideRenameAndSave()
    {
        TitleContainer.Visibility = Visibility.Visible;
        RenameBox.Visibility = Visibility.Collapsed;
        if (RenameBox.Text != (ViewModel.Data.Title ?? string.Empty))
        {
            ViewModel.ChangeTitleCommand.Execute(RenameBox.Text);
        }
    }

    private void OnRenameBoxLostFocus(object sender, RoutedEventArgs e)
        => HideRenameAndSave();

    private void OnRenameBoxPreviewKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            HideRenameAndSave();
            e.Handled = true;
        }
    }

    private void OnTitleTapped(object sender, TappedRoutedEventArgs e)
        => ShowRename();
}
