// Copyright (c) Rodel. All rights reserved.

namespace RodelAgent.UI.Controls.Chat;

/// <summary>
/// 聊天模型选择.
/// </summary>
public sealed partial class ChatModelSelection : ChatSessionControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChatModelSelection"/> class.
    /// </summary>
    public ChatModelSelection()
    {
        InitializeComponent();
    }

    private void OnModelClick(object sender, ViewModels.Items.ChatModelItemViewModel e)
    {
        if (e.Id == ViewModel.Data?.Model)
        {
            return;
        }

        ViewModel.ChangeModelCommand.Execute(e);
    }
}
