// Copyright (c) Rodel. All rights reserved.

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
/// 聊天会话视图模型.
/// </summary>
public sealed partial class ChatSessionViewModel : ViewModelBase<ChatSession>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChatSessionViewModel"/> class.
    /// </summary>
    public ChatSessionViewModel(
        ChatSession data,
        IChatClient chatClient)
        : base(data)
    {
        _chatClient = chatClient;
        _dispatcherQueue = GlobalDependencies.ServiceProvider.GetService<DispatcherQueue>();
        _logger = GlobalDependencies.ServiceProvider.GetService<ILogger<ChatSessionViewModel>>();
        _storageService = GlobalDependencies.ServiceProvider.GetService<IStorageService>();
        IsEnterSend = SettingsToolkit.ReadLocalSetting(SettingNames.ChatServicePageIsEnterSend, true);
        Messages.CollectionChanged += OnMessageCountChanged;
        PresetViewModel = new(data);
        Initialize(data);
    }

    private void Initialize(ChatSession data)
    {
        Title = data.Title ?? ResourceToolkit.GetLocalizedString(StringNames.RandomChat);
        MaxRounds = data.MaxRounds;
        Model = data.Model;
        if (!string.IsNullOrEmpty(data.PresetId))
        {
            var chatPageVM = GlobalDependencies.ServiceProvider.GetService<ChatServicePageViewModel>();
            IsAgentPreset = chatPageVM.AgentPresets.Any(p => p.Data.Id == data.PresetId);
            IsSessionPreset = chatPageVM.SessionPresets.Any(p => p.Data.Id == data.PresetId);
        }

        IsNormalSession = string.IsNullOrEmpty(data.PresetId);
        if (data.Messages != null && data.Messages.Count > 0)
        {
            foreach (var message in data.Messages)
            {
                if (message.Role == MessageRole.System)
                {
                    continue;
                }

                var vm = new ChatMessageItemViewModel(message, EditMessageAsync, DeleteMessageAsync);
                Messages.Add(vm);
            }
        }

        InitializeModels();
        CheckChatEmpty();
        CheckLastMessageTime();
        CheckRegenerateButtonShown();
        RequestFocusInput?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// 在进入视图开始显示时执行.
    /// </summary>
    [RelayCommand]
    private void EnterView()
        => CalcTotalTokenCountCommand.Execute(default);

    [RelayCommand]
    private void NewSession()
    {
        var pageVM = GlobalDependencies.ServiceProvider.GetService<ChatServicePageViewModel>();
        pageVM.CreateNewSessionCommand.Execute(default);
        RequestFocusInput?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private void SaveAsPreset()
    {
        this.Get<ChatServicePageViewModel>()
            .SaveAsSessionPresetCommand.Execute(Data);
    }

    [RelayCommand]
    private async Task SaveSessionToDatabaseAsync(bool force = false)
    {
        if (!force && (Data.Messages == null || Data.Messages.Count == 0))
        {
            return;
        }

        await _storageService.AddOrUpdateChatSessionAsync(Data);
        CheckRegenerateButtonShown();
        GlobalDependencies.ServiceProvider.GetService<ChatServicePageViewModel>().CheckCurrentSessionExistCommand.Execute(this);
    }

    [RelayCommand]
    private async Task ChangeTitleAsync(string title)
    {
        Title = string.IsNullOrEmpty(title) ? ResourceToolkit.GetLocalizedString(StringNames.RandomChat) : title;
        Data.Title = Title;
        await SaveSessionToDatabaseAsync(true);
    }

    [RelayCommand]
    private async Task ChangeModelAsync(ChatModelItemViewModel model)
    {
        TotalTokenCount = 0;
        if (model is null)
        {
            return;
        }

        Data.Model = model.Id;
        Model = model.Name;
        TotalTokenCount = Convert.ToInt32(model.ContextLength);
        foreach (var item in Models)
        {
            item.IsSelected = item.Equals(model);
        }

        var pageVM = GlobalDependencies.ServiceProvider.GetService<ChatServicePageViewModel>();
        if (pageVM.HistoryChatSessions.Any(p => p.SessionId == SessionId))
        {
            await SaveSessionToDatabaseAsync(true);
        }

        SettingsToolkit.WriteLocalSetting($"{Data.Provider}DefaultChatModel", model.Id);
        RequestFocusInput?.Invoke(this, EventArgs.Empty);
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
    private void CheckRegenerateButtonShown()
        => IsRegenerateButtonShown = !IsChatEmpty && !IsResponding && Messages.Last().IsAssistant;

    [RelayCommand]
    private async Task CalcTotalTokenCountAsync()
    {
        var pageVM = GlobalDependencies.ServiceProvider.GetService<ChatServicePageViewModel>();
        if (pageVM.CurrentSession != this)
        {
            return;
        }

        await CalcBaseTokenCountAsync();
        await CalcUserInputTokenCountAsync();
    }

    private void InitializeModels()
    {
        Models.Clear();
        var pageVM = GlobalDependencies.ServiceProvider.GetService<ChatServicePageViewModel>();
        var service = pageVM.AvailableServices.FirstOrDefault(p => p.ProviderType == Data.Provider);
        if (service == null)
        {
            return;
        }

        var models = service.ServerModels.Concat(service.CustomModels).OrderByDescending(p => p.Data.IsCustomModel).ToList();
        foreach (var item in models)
        {
            Models.Add(new ChatModelItemViewModel(item.Data));
        }

        var selectedModel = Models.FirstOrDefault(p => p.Id == Data.Model);
        if (selectedModel == null)
        {
            var defaultModel = SettingsToolkit.ReadLocalSetting($"{Data.Provider}DefaultChatModel", string.Empty);
            if (!string.IsNullOrEmpty(defaultModel))
            {
                selectedModel = Models.FirstOrDefault(p => p.Id == defaultModel);
            }

            selectedModel ??= Models.FirstOrDefault();
        }

        if (selectedModel is not null)
        {
            selectedModel.IsSelected = true;
            Model = selectedModel.Name;
            TotalTokenCount = Convert.ToInt32(selectedModel.ContextLength);
        }
    }

    private void CheckChatEmpty()
        => IsChatEmpty = Messages.Count == 0;

    private void CheckLastMessageTime()
    {
        var lastMsg = Messages.LastOrDefault();
        LastMessageTime = lastMsg is not null ? lastMsg.Data.Time?.ToLocalTime().ToString("MM/dd") ?? string.Empty : string.Empty;
    }

    private void CheckCurrentModelStatus()
    {
        var model = Models.FirstOrDefault(p => p.Id == Data.Model);
        if (model is null)
        {
            return;
        }

        IsSupportVision = model.IsSupportVision;
        IsSupportTool = model.IsSupportTool;
    }

    private void OnMessageCountChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            RequestScrollToBottom?.Invoke(this, EventArgs.Empty);
        }

        CheckChatEmpty();
        CheckRegenerateButtonShown();
        CheckLastMessageTime();
        CalcTotalTokenCountCommand.Execute(default);
    }

    partial void OnIsEnterSendChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.ChatServicePageIsEnterSend, value);

    partial void OnModelChanged(string value)
        => CheckCurrentModelStatus();

    partial void OnUserInputChanged(string value)
        => _lastInputTime = DateTimeOffset.Now;
}
