// Copyright (c) Richasy. All rights reserved.

using Richasy.AgentKernel;
using RodelAgent.Models.Feature;
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
        var list = new List<ChatHistoryItemViewModel>();
        if (conversations is not null)
        {
            foreach (var conversation in conversations)
            {
                var item = new ChatHistoryItemViewModel(conversation);
                list.Add(item);
            }
        }

        list.OrderByDescending(p => p.GetLastMessageTime()).ToList().ForEach(History.Add);
        IsHistoryInitializing = false;
    }

    private void TryCreateConversation()
    {
        if (_currentConversation != null)
        {
            return;
        }

        var conversation = new ChatConversation
        {
            UseStreamOutput = true,
            History = [],
            Id = Guid.NewGuid().ToString("N"),
            Provider = CurrentProvider!.Value,
            Model = SelectedModel?.Id,
        };

        History.Insert(0, new ChatHistoryItemViewModel(conversation));
        SetCurrentConversation(conversation);
    }

    private async Task SaveCurrentMessagesAsync()
    {
        TryCreateConversation();
        if (_currentConversation != null)
        {
            _currentConversation.History = [.. Messages];
            await _storageService.AddOrUpdateChatConversationAsync(_currentConversation);
            var item = History.FirstOrDefault(p => p.Id == _currentConversation.Id);
            item?.Update();
        }
    }

    private void SetCurrentConversation(ChatConversation? data)
    {
        _currentConversation = data;
        if (_currentConversation != null)
        {
            Messages.Clear();
            foreach (var message in _currentConversation!.History ?? [])
            {
                Messages.Add(message);
            }

            foreach (var item in History)
            {
                item.IsSelected = item.Conversation == _currentConversation;
            }
        }
        else
        {
            foreach (var item in History)
            {
                item.IsSelected = false;
            }
        }
    }
}
