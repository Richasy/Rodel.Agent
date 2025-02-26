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
        Id = conversation.Id;
        Update();
    }

    /// <summary>
    /// 显示名称.
    /// </summary>
    [ObservableProperty]
    public partial string? Name { get; set; }

    [ObservableProperty]
    public partial bool IsSelected { get; set; }

    [ObservableProperty]
    public partial string? LastMessageDate { get; set; }

    internal ChatConversation? Conversation { get; set; }

    internal string? Id { get; set; }

    public override bool Equals(object? obj) => obj is ChatHistoryItemViewModel model && Id == model.Id;

    public override int GetHashCode() => HashCode.Combine(Id);

    public long GetLastMessageTime()
    {
        return Conversation?.History?.Count > 0
            ? Conversation.History.Last().Time
            : 0;
    }

    public void Update()
    {
        Name = Conversation?.Name ?? string.Empty;
        LastMessageDate = Conversation?.History?.Count > 0
                ? DateTimeOffset.FromUnixTimeSeconds(Conversation.History.Last().Time).ToString("MM/dd")
                : string.Empty;
    }
}
