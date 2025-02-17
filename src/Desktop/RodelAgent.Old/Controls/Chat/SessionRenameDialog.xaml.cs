// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.ViewModels.Components;

namespace RodelAgent.UI.Controls.Chat;

/// <summary>
/// 会话重命名对话框.
/// </summary>
public sealed partial class SessionRenameDialog : AppContentDialog
{
    private readonly ChatSessionViewModel _sessionVM;
    private readonly ChatGroupViewModel _groupVM;

    /// <summary>
    /// Initializes a new instance of the <see cref="SessionRenameDialog"/> class.
    /// </summary>
    public SessionRenameDialog(ChatSessionViewModel sessionVM)
    {
        InitializeComponent();
        _sessionVM = sessionVM;
        RenameBox.Text = _sessionVM.Data.Title ?? string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SessionRenameDialog"/> class.
    /// </summary>
    public SessionRenameDialog(ChatGroupViewModel groupVM)
    {
        InitializeComponent();
        _groupVM = groupVM;
        RenameBox.Text = _groupVM.Data.Title ?? string.Empty;
    }

    private void OnPrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        var newTitle = RenameBox.Text;

        if (_groupVM != null)
        {
            if (newTitle == (_groupVM.Data.Title ?? string.Empty))
            {
                return;
            }

            _groupVM.ChangeTitleCommand.Execute(newTitle);
        }
        else if (_sessionVM != null)
        {
            if (newTitle == (_sessionVM.Data.Title ?? string.Empty))
            {
                return;
            }

            _sessionVM.ChangeTitleCommand.Execute(newTitle);
        }
    }
}
