// Copyright (c) Richasy. All rights reserved.

using Richasy.AgentKernel;
using RodelAgent.UI.ViewModels.Items;

namespace RodelAgent.UI.ViewModels.Core;

public sealed partial class ChatSessionViewModel
{
    private async Task LoadConversationsWithProviderAsync(ChatProviderType provider)
    {
        IsHistoryInitializing = true;
        History.Clear();
        CheckHistoryEmpty();
        var conversations = await _storageService.GetChatConversationsAsync(provider);
        if (conversations is not null)
        {
            foreach (var conversation in conversations)
            {
                var item = new ChatHistoryItemViewModel(conversation);
                History.Add(item);
            }
        }

        IsHistoryInitializing = false;
    }

    private void TryCreateConversation()
    {
        _currentConversation ??= new RodelAgent.Models.Feature.ChatConversation
            {
                UseStreamOutput = true,
                History = [],
                Id = Guid.NewGuid().ToString("N"),
                Provider = CurrentProvider!.Value,
                Model = SelectedModel?.Id,
            };
    }

    private async Task SaveCurrentMessagesAsync()
    {
        TryCreateConversation();
        if (_currentConversation != null)
        {
            _currentConversation.History = [.. Messages];
            await _storageService.AddOrUpdateChatConversationAsync(_currentConversation);
        }
    }
}
