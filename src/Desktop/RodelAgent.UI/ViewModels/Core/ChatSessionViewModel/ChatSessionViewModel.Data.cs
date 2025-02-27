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

        var options = _getCurrentOptions?.Invoke();

        var conversation = new ChatConversation
        {
            UseStreamOutput = options?.AdditionalProperties?.GetValueOrDefault("stream") as bool? ?? true,
            MaxRounds = options?.AdditionalProperties?.GetValueOrDefault("max_rounds") as int? ?? 0,
            History = [],
            Id = Guid.NewGuid().ToString("N"),
            Provider = CurrentProvider!.Value,
            Model = SelectedModel?.Id,
            SystemInstruction = SystemInstruction,
            Options = options,
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
            var uiOptions = _getCurrentOptions?.Invoke();
            if (uiOptions != null)
            {
                _currentConversation.Options = uiOptions;
                _currentConversation.UseStreamOutput = CurrentOptions?.AdditionalProperties?.GetValueOrDefault("stream") as bool? ?? true;
                _currentConversation.MaxRounds = CurrentOptions?.AdditionalProperties?.GetValueOrDefault("max_rounds") as int? ?? 0;
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
            CurrentOptions = null;
            foreach (var item in History)
            {
                item.IsSelected = false;
            }
        }

        RequestReloadOptionsUI?.Invoke(this, EventArgs.Empty);
    }
}
