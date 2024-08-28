﻿// Copyright (c) Rodel. All rights reserved.

using System.Collections.Specialized;
using Microsoft.UI.Dispatching;
using RodelAgent.Interfaces;
using RodelAgent.UI.Models.Constants;
using RodelAgent.UI.Toolkits;
using RodelAgent.UI.ViewModels.Items;
using RodelAgent.UI.ViewModels.Pages;
using RodelChat.Interfaces.Client;
using RodelChat.Models.Client;
using RodelChat.Models.Constants;

namespace RodelAgent.UI.ViewModels.Components;

/// <summary>
/// 聊天群组视图模型.
/// </summary>
public sealed partial class ChatGroupViewModel : ViewModelBase<ChatGroup>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChatGroupViewModel"/> class.
    /// </summary>
    public ChatGroupViewModel(ChatGroup data, IChatClient chatClient)
        : base(data)
    {
        _chatClient = chatClient;
        _dispatcherQueue = GlobalDependencies.ServiceProvider.GetService<DispatcherQueue>();
        _logger = GlobalDependencies.ServiceProvider.GetService<ILogger<ChatSessionViewModel>>();
        _storageService = GlobalDependencies.ServiceProvider.GetService<IStorageService>();
        IsEnterSend = SettingsToolkit.ReadLocalSetting(SettingNames.ChatServicePageIsEnterSend, true);
        Messages.CollectionChanged += OnMessageCountChanged;
        InitializeCommand.Execute(data);
    }

    [RelayCommand]
    private async Task InitializeAsync(ChatGroup data)
    {
        GroupName = data.Name;
        Title = data.Title ?? ResourceToolkit.GetLocalizedString(StringNames.RandomChat);
        MaxRounds = data.MaxRounds;
        await InitializeAgentsCommand.ExecuteAsync(default);

        if (data.TerminateText != null && data.TerminateText.Count > 0)
        {
            foreach (var item in data.TerminateText)
            {
                TerminateText.Add(item);
            }
        }

        if (data.Messages != null && data.Messages.Count > 0)
        {
            foreach (var message in data.Messages)
            {
                if (message.Role == MessageRole.System)
                {
                    continue;
                }

                var vm = new ChatMessageItemViewModel(message, EditMessageAsync, DeleteMessageAsync);

                var agent = Agents.FirstOrDefault(p => p.Data.Id == message.AuthorId);
                if (agent is not null && agent.Name != vm.Author)
                {
                    vm.Author = agent.Name;
                }

                Messages.Add(vm);
            }
        }

        CheckChatEmpty();
        CheckLastMessageTime();
        RequestFocusInput?.Invoke(this, EventArgs.Empty);
        RequestScrollToBottom?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private void NewSession()
    {
        var pageVM = GlobalDependencies.ServiceProvider.GetService<ChatServicePageViewModel>();
        pageVM.CreateNewSessionCommand.Execute(default);
        RequestFocusInput?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private async Task SaveSessionToDatabaseAsync(bool force = false)
    {
        if (!force && (Data.Messages == null || Data.Messages.Count == 0))
        {
            return;
        }

        await _storageService.AddOrUpdateChatGroupSessionAsync(Data);
        GlobalDependencies.ServiceProvider.GetService<ChatServicePageViewModel>().CheckCurrentGroupExistCommand.Execute(this);
    }

    [RelayCommand]
    private async Task ChangeTitleAsync(string title)
    {
        Title = string.IsNullOrEmpty(title) ? ResourceToolkit.GetLocalizedString(StringNames.RandomChat) : title;
        Data.Title = Title;
        await SaveSessionToDatabaseAsync(true);
    }

    [RelayCommand]
    private void CheckMaxRounds()
    {
        if (MaxRounds != Data.MaxRounds)
        {
            Data.MaxRounds = MaxRounds;

            var pageVM = GlobalDependencies.ServiceProvider.GetService<ChatServicePageViewModel>();
            if (pageVM.HistoryChatSessions.Any(p => p.SessionId == SessionId))
            {
                SaveSessionToDatabaseCommand.Execute(default);
            }
        }
    }

    [RelayCommand]
    private void CheckTerminateText()
    {
        if (TerminateText != null)
        {
            Data.TerminateText = new List<string>(TerminateText);
            SaveSessionToDatabaseCommand.Execute(default);
        }
    }

    [RelayCommand]
    private async Task InitializeAgentsAsync()
    {
        Agents.Clear();
        var storageService = this.Get<IStorageService>();
        var agents = await storageService.GetChatAgentsAsync();
        foreach (var agentId in Data.Agents)
        {
            var agent = agents.FirstOrDefault(p => p.Id == agentId);
            if (agent is null)
            {
                continue;
            }

            var a = new ChatPresetItemViewModel(agent);
            Agents.Add(a);
        }

        UpdateAgentSelection();
    }

    private void CheckChatEmpty()
        => IsChatEmpty = Messages.Count == 0;

    private void CheckLastMessageTime()
    {
        var lastMsg = Messages.LastOrDefault();
        LastMessageTime = lastMsg is not null ? lastMsg.Data.Time?.ToLocalTime().ToString("MM/dd") ?? string.Empty : string.Empty;
    }

    private void OnMessageCountChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            RequestScrollToBottom?.Invoke(this, EventArgs.Empty);
        }

        CheckChatEmpty();
        CheckLastMessageTime();
    }

    partial void OnIsEnterSendChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.ChatServicePageIsEnterSend, value);
}
