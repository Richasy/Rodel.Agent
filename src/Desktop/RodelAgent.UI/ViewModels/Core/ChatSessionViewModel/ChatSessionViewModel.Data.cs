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
                var item = new ChatHistoryItemViewModel(conversation, RemoveHistoryAsync);
                list.Add(item);
            }
        }

        list.OrderByDescending(p => p.GetLastMessageTime()).ToList().ForEach(History.Add);
        IsHistoryInitializing = false;
    }

    private async Task RemoveHistoryAsync(ChatHistoryItemViewModel item)
    {
        if (item.IsSelected)
        {
            SetCurrentConversation(null);
        }

        if (item.Conversation != null)
        {
            await _storageService.RemoveChatConversationAsync(item.Conversation.Id);
            History.Remove(item);
            CheckHistoryEmpty();
        }
    }

    private void TryCreateConversation()
    {
        if (_currentConversation != null)
        {
            return;
        }

        var options = _getCurrentOptions?.Invoke();
        var isStreamOutput = _getIsStreamOutput?.Invoke() ?? true;
        var maxRounds = _getMaxRounds?.Invoke() ?? 0;

        var conversation = new ChatConversation
        {
            UseStreamOutput = isStreamOutput,
            MaxRounds = maxRounds,
            History = [],
            Id = Guid.NewGuid().ToString("N"),
            Provider = CurrentProvider!.Value,
            Model = SelectedModel?.Id,
            SystemInstruction = SystemInstruction,
            Options = options,
        };

        History.Insert(0, new ChatHistoryItemViewModel(conversation, RemoveHistoryAsync));
        SetCurrentConversation(conversation);
    }

    private async Task SaveCurrentMessagesAsync()
    {
        TryCreateConversation();
        if (_currentConversation != null)
        {
            _currentConversation.History = [.. Messages];
            _currentConversation.UseStreamOutput = _getIsStreamOutput?.Invoke() ?? true;
            _currentConversation.MaxRounds = _getMaxRounds?.Invoke() ?? 0;
            var uiOptions = _getCurrentOptions?.Invoke();
            if (uiOptions != null)
            {
                _currentConversation.Options = uiOptions;
            }

            await _storageService.AddOrUpdateChatConversationAsync(_currentConversation);
            var item = History.FirstOrDefault(p => p.Id == _currentConversation.Id);
            item?.Update();
        }
    }

    [RelayCommand]
    private async Task SaveSystemInstructionAsync()
    {
        if (_currentConversation == null)
        {
            // 在没有消息记录的情况下，修改系统提示词没有意义，不做处理.
            return;
        }

        _currentConversation.SystemInstruction = SystemInstruction;
        await _storageService.AddOrUpdateChatConversationAsync(_currentConversation);
    }

    [RelayCommand]
    private async Task RemoveAllSessionsAsync()
    {
        foreach (var item in History)
        {
            await _storageService.RemoveChatConversationAsync(item.Conversation!.Id);
        }

        History.Clear();
        CheckHistoryEmpty();
    }

    private void SetCurrentConversation(ChatConversation? data)
    {
        _currentConversation = data;
        if (_currentConversation != null)
        {
            if (!IsGenerating)
            {
                Messages.Clear();
                SystemInstruction = _currentConversation.SystemInstruction;
                CurrentOptions = _currentConversation.Options;
                Title = _currentConversation.Title;
                foreach (var message in _currentConversation!.History ?? [])
                {
                    Messages.Add(message);
                }
            }

            foreach (var item in History)
            {
                item.IsSelected = item.Conversation == _currentConversation;
            }
        }
        else
        {
            SystemInstruction = string.Empty;
            Title = string.Empty;
            CurrentOptions = null;
            foreach (var item in History)
            {
                item.IsSelected = false;
            }

            ClearMessageCommand.Execute(default);
        }

        RequestReloadOptionsUI?.Invoke(this, EventArgs.Empty);
    }
}
