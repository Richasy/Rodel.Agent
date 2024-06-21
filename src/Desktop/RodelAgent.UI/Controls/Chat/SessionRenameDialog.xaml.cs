// Copyright (c) Rodel. All rights reserved.

using RodelAgent.UI.ViewModels.Components;

namespace RodelAgent.UI.Controls.Chat;

/// <summary>
/// 会话重命名对话框.
/// </summary>
public sealed partial class SessionRenameDialog : AppContentDialog
{
    private readonly ChatSessionViewModel _sessionVM;

    /// <summary>
    /// Initializes a new instance of the <see cref="SessionRenameDialog"/> class.
    /// </summary>
    public SessionRenameDialog(ChatSessionViewModel sessionVM)
    {
        InitializeComponent();
        _sessionVM = sessionVM;
        RenameBox.Text = _sessionVM.Data.Title ?? string.Empty;
    }

    private void OnPrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        var newTitle = RenameBox.Text;
        if (newTitle == (_sessionVM.Data.Title ?? string.Empty))
        {
            return;
        }

        _sessionVM.ChangeTitleCommand.Execute(newTitle);
    }
}
