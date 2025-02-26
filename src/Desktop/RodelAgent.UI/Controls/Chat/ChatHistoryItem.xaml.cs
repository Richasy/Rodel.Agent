// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.ViewModels.Core;
using RodelAgent.UI.ViewModels.Items;

namespace RodelAgent.UI.Controls.Chat;

/// <summary>
/// Chat history item.
/// </summary>
public sealed partial class ChatHistoryItem : ChatHistoryItemBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChatHistoryItem"/> class.
    /// </summary>
    public ChatHistoryItem() => InitializeComponent();

    private void OnHistoryItemClick(object sender, RoutedEventArgs e)
        => this.Get<ChatSessionViewModel>().LoadHistoryItemCommand.Execute(ViewModel);
}

public abstract class ChatHistoryItemBase : LayoutUserControlBase<ChatHistoryItemViewModel>;
