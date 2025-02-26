// Copyright (c) Richasy. All rights reserved.

using RodelAgent.Models.Feature;

namespace RodelAgent.UI.ViewModels.Items;

public sealed partial class ChatHistoryItemViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChatHistoryItemViewModel"/> class.
    /// </summary>
    public ChatHistoryItemViewModel(ChatConversation conversation)
    {
        Conversation = conversation;
        Name = conversation.Name;
        Id = conversation.Id;
    }

    /// <summary>
    /// 显示名称.
    /// </summary>
    [ObservableProperty]
    public partial string? Name { get; set; }

    [ObservableProperty]
    public partial bool IsSelected { get; set; }

    internal ChatConversation? Conversation { get; set; }

    internal string? Id { get; set; }

    public override bool Equals(object? obj) => obj is ChatHistoryItemViewModel model && Id == model.Id;

    public override int GetHashCode() => HashCode.Combine(Id);
}
