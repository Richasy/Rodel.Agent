// Copyright (c) Richasy. All rights reserved.

using Richasy.AgentKernel;
using RodelAgent.Models.Feature;
using RodelAgent.UI.ViewModels.Items;
using SqlSugar;

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

    private async Task LoadConversationsWithAgentIdAsync(string agentId)
    {
        IsHistoryInitializing = true;
        History.Clear();
        CheckHistoryEmpty();
        var conversations = await _storageService.GetChatConversationsByAgentAsync(agentId);
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

    private async Task LoadConversationsWithGroupIdAsync(string groupId)
    {
        IsHistoryInitializing = true;
        History.Clear();
        CheckHistoryEmpty();
        var conversations = await _storageService.GetChatConversationsByGroupAsync(groupId);
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

        var options = _getSessionCurrentOptions?.Invoke() ?? default;
        var isStreamOutput = _getSessionIsStreamOutput?.Invoke() ?? true;
        var maxRounds = IsGroup ? _getGroupMaxRounds?.Invoke() ?? 1 : _getSessionMaxRounds?.Invoke() ?? 0;

        var conversation = new ChatConversation
        {
            UseStreamOutput = isStreamOutput,
            MaxRounds = maxRounds,
            History = [],
            Id = Guid.NewGuid().ToString("N"),
        };

        if (IsGroup)
        {
            conversation.GroupId = CurrentGroup!.Id;
            conversation.Agents = CurrentGroup.Agents!;
        }
        else
        {
            conversation.Provider = CurrentProvider!.Value;
            conversation.Model = SelectedModel?.Id;
            conversation.SystemInstruction = SystemInstruction;
            conversation.Options = options;
            conversation.AgentId = IsAgent ? CurrentAgent?.Id : null;
        }

        History.Insert(0, new ChatHistoryItemViewModel(conversation, RemoveHistoryAsync));
        SetCurrentConversation(conversation);
    }

    private async Task SaveCurrentMessagesAsync()
    {
        TryCreateConversation();
        if (_currentConversation != null)
        {
            if (IsGroup)
            {
                _currentConversation.MaxRounds = _getGroupMaxRounds?.Invoke() ?? 1;
            }
            else
            {
                _currentConversation.Provider = CurrentProvider!;
                _currentConversation.Model = SelectedModel?.Id;
                _currentConversation.UseStreamOutput = _getSessionIsStreamOutput?.Invoke() ?? true;
                _currentConversation.MaxRounds = _getSessionMaxRounds?.Invoke() ?? 0;
                var uiOptions = _getSessionCurrentOptions?.Invoke();
                if (uiOptions != null)
                {
                    _currentConversation.Options = uiOptions;
                }
            }

            _currentConversation.History = [.. Messages];
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
                if (IsAgent)
                {
                    if (_currentConversation.Provider != CurrentProvider)
                    {
                        var service = Services.FirstOrDefault(p => p.ProviderType == _currentConversation.Provider);
                        service ??= Services.FirstOrDefault(p => p.ProviderType == CurrentAgent?.Provider);
                        service ??= Services.FirstOrDefault();
                        ChangeServiceCommand.Execute(service);
                    }
                    else if (_currentConversation.Model != SelectedModel?.Id)
                    {
                        var model = Models.FirstOrDefault(p => p.Id == _currentConversation.Model);
                        model ??= Models.FirstOrDefault();
                        SelectModelCommand.Execute(model);
                    }
                }

                Messages.Clear();

                if (!IsGroup)
                {
                    SystemInstruction = _currentConversation.SystemInstruction;
                    CurrentOptions = _currentConversation.Options;
                }

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
            SystemInstruction = IsAgent && CurrentAgent != null
                ? CurrentAgent.SystemInstruction
                : string.Empty;
            Title = IsAgent && CurrentAgent != null
                ? CurrentAgent.Name
                : IsGroup && CurrentGroup != null
                    ? CurrentGroup.Name
                    : string.Empty;
            CurrentOptions = IsAgent && CurrentAgent != null
                ? CurrentAgent.Options
                : null;
            if (IsAgent && CurrentAgent != null)
            {
                SystemInstruction = CurrentAgent.SystemInstruction;
                Title = CurrentAgent.Name;
                CurrentOptions = CurrentAgent.Options;
                SelectedService = Services.FirstOrDefault(p => p.ProviderType == CurrentAgent.Provider);
            }
            else
            {
                SystemInstruction = string.Empty;
                Title = string.Empty;
                CurrentOptions = null;
            }

            foreach (var item in History)
            {
                item.IsSelected = false;
            }

            ClearMessageCommand.Execute(default);
        }

        RequestReloadOptionsUI?.Invoke(this, EventArgs.Empty);
    }
}
