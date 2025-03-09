// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.ViewModels.Items;

namespace RodelAgent.UI.Controls.Chat;

public sealed partial class ChatHistoryRenameDialog : AppDialog
{
    private readonly ChatHistoryItemViewModel? _historyVM;

    public ChatHistoryRenameDialog(ChatHistoryItemViewModel vm)
    {
        InitializeComponent();
        _historyVM = vm;
        RenameBox.Text = _historyVM.Conversation!.Title ?? string.Empty;
    }

    public string NewTitle => RenameBox?.Text ?? string.Empty;
}
