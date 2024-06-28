// Copyright (c) Rodel. All rights reserved.

using RodelAgent.UI.Controls.Chat;
using RodelAgent.UI.Models.Constants;
using RodelAgent.UI.Toolkits;
using RodelAgent.UI.ViewModels.Components;
using RodelAgent.UI.ViewModels.Items;
using RodelChat.Models.Client;
using RodelChat.Models.Constants;

namespace RodelAgent.UI.ViewModels.Pages;

/// <summary>
/// 聊天服务页面视图模型关于会话的部分.
/// </summary>
public sealed partial class ChatServicePageViewModel
{
    [RelayCommand]
    private void CreateNewSession()
    {
        if (SettingsToolkit.IsSettingKeyExist(SettingNames.LastSelectedChatService))
        {
            var lastProvider = SettingsToolkit.ReadLocalSetting(SettingNames.LastSelectedChatService, ProviderType.OpenAI);
            var chatVM = AvailableServices.FirstOrDefault(p => p.ProviderType == lastProvider);
            if (chatVM == null)
            {
                // TODO: show tip
                return;
            }

            SetSelectedSession(default);
            CreateNewChatSessionInternal(chatVM);
        }
        else if (SettingsToolkit.IsSettingKeyExist(SettingNames.LastSelectedAgent))
        {
            var lastAgent = SettingsToolkit.ReadLocalSetting(SettingNames.LastSelectedAgent, string.Empty);
            var agentVM = AgentPresets.FirstOrDefault(p => p.Data.Id == lastAgent);
            if (agentVM == null)
            {
                return;
            }

            SetSelectedSession(default);
            CreateNewChatSessionInternal(agentVM);
        }
        else if (SettingsToolkit.IsSettingKeyExist(SettingNames.LastSelectedSessionPreset))
        {
            var lastSessionPreset = SettingsToolkit.ReadLocalSetting(SettingNames.LastSelectedSessionPreset, string.Empty);
            var sessionPresetVM = SessionPresets.FirstOrDefault(p => p.Data.Id == lastSessionPreset);
            if (sessionPresetVM == null)
            {
                return;
            }

            SetSelectedSession(default);
            CreateNewChatSessionInternal(sessionPresetVM);
        }
        else if (SettingsToolkit.IsSettingKeyExist(SettingNames.LastSelectedGroup))
        {
            var lastGroup = SettingsToolkit.ReadLocalSetting(SettingNames.LastSelectedGroup, string.Empty);
            var groupVM = GroupPresets.FirstOrDefault(p => p.Data.Id == lastGroup);
            if (groupVM == null)
            {
                return;
            }

            SetSelectedSession(default);
            CreateNewChatGroupInternal(groupVM);
        }
    }

    [RelayCommand]
    private async Task RemoveSessionAsync(ChatSessionViewModel sessionVM)
    {
        if (sessionVM == null)
        {
            return;
        }

        if (CurrentSession == sessionVM)
        {
            CreateNewSession();
        }

        sessionVM.CancelMessageCommand.Execute(default);
        await _storageService.RemoveChatSessionAsync(sessionVM.SessionId);
        HistoryChatSessions.Remove(sessionVM);
    }

    [RelayCommand]
    private async Task RemoveAllSessionsAsync()
    {
        foreach (var session in HistoryChatSessions)
        {
            session.IsSelected = false;
            session.CancelMessageCommand.Execute(default);
            await _storageService.RemoveChatSessionAsync(session.SessionId);
        }

        CurrentSession = default;
        HistoryChatSessions.Clear();
        CreateNewSessionCommand.Execute(default);
    }

    [RelayCommand]
    private void SetSelectedSession(ChatSessionViewModel sessionVM)
    {
        foreach (var item in HistoryChatSessions)
        {
            item.IsSelected = sessionVM != null && item.Equals(sessionVM);
        }

        if (sessionVM != null && CurrentSession != sessionVM)
        {
            CurrentSession = sessionVM;
        }

        CurrentSession?.CheckRegenerateButtonShownCommand.Execute(default);
        CurrentSession?.ResetPluginsCommand.Execute(default);
    }

    [RelayCommand]
    private void CheckCurrentSessionExist(ChatSessionViewModel vm)
    {
        if (CurrentSession != vm)
        {
            return;
        }

        if (string.IsNullOrEmpty(vm.Data.PresetId) && AvailableServices.FirstOrDefault(p => p.IsSelected)?.ProviderType != CurrentSession.Data.Provider)
        {
            return;
        }

        if (!string.IsNullOrEmpty(vm.Data.PresetId)
            && AgentPresets.FirstOrDefault(p => p.IsSelected)?.Data.Id != CurrentSession.Data.PresetId
            && SessionPresets.FirstOrDefault(p => p.IsSelected)?.Data.Id != CurrentSession.Data.PresetId)
        {
            return;
        }

        var sourceSession = HistoryChatSessions.FirstOrDefault(p => p.SessionId == CurrentSession.SessionId);
        if (sourceSession != null)
        {
            sourceSession.Title = CurrentSession.Title;
            sourceSession.Data.Title = CurrentSession.Title;
            return;
        }

        HistoryChatSessions.Insert(0, CurrentSession);
        SetSelectedSession(CurrentSession);
    }

    [RelayCommand]
    private async Task ResetSessionPresetsAsync()
    {
        SessionPresets.Clear();
        var localPresets = await _storageService.GetChatSessionPresetsAsync();
        foreach (var preset in localPresets)
        {
            SessionPresets.Add(new ChatPresetItemViewModel(preset));
        }

        if (SettingsToolkit.IsSettingKeyExist(SettingNames.LastSelectedSessionPreset))
        {
            var lastSelectedPreset = SettingsToolkit.ReadLocalSetting(SettingNames.LastSelectedSessionPreset, string.Empty);
            var preset = SessionPresets.FirstOrDefault(p => p.Data.Id == lastSelectedPreset);
            SetSelectedSessionPresetCommand.Execute(preset);
        }

        IsSessionPresetsEmpty = SessionPresets.Count == 0;
    }

    [RelayCommand]
    private async Task SaveAsSessionPresetAsync(ChatSession session)
    {
        var preset = session.Clone();
        preset.Name = session.Title;
        preset.Id = Guid.NewGuid().ToString("N");
        var vm = new ChatPresetItemViewModel(preset);
        _chatPresetModuleVM.SetData(vm, ChatSessionPresetType.Session);
        var dialog = new ChatPresetSettingsDialog();
        await dialog.ShowAsync();

        preset = dialog.ViewModel.Data.Data;
        if (preset is not null && !_chatPresetModuleVM.IsManualClose)
        {
            if (!SessionPresets.Any(p => p.Data.Equals(preset)))
            {
                SessionPresets.Add(new ChatPresetItemViewModel(preset));
                await _storageService.AddOrUpdateChatSessionPresetAsync(preset);
            }
        }

        IsSessionPresetsEmpty = SessionPresets.Count == 0;
    }

    [RelayCommand]
    private async Task EditSessionPresetAsync(ChatPresetItemViewModel presetVM)
    {
        if (presetVM == null)
        {
            return;
        }

        var newVM = new ChatPresetItemViewModel(presetVM.Data);
        _chatPresetModuleVM.SetData(newVM, ChatSessionPresetType.Session);
        var dialog = new ChatPresetSettingsDialog();
        await dialog.ShowAsync();

        var preset = dialog.ViewModel.Data;
        if (preset is not null && !_chatPresetModuleVM.IsManualClose)
        {
            presetVM.Data = preset.Data;
            await _storageService.AddOrUpdateChatSessionPresetAsync(preset.Data);
        }
    }

    [RelayCommand]
    private async Task DeleteSessionPresetAsync(ChatPresetItemViewModel presetVM)
    {
        if (presetVM == null)
        {
            return;
        }

        if (presetVM.IsSelected && AvailableServices.Count > 0)
        {
            SetSelectedChatServiceCommand.Execute(AvailableServices.FirstOrDefault());
        }

        SessionPresets.Remove(presetVM);
        IsSessionPresetsEmpty = SessionPresets.Count == 0;
        await _storageService.RemoveChatSessionPresetAsync(presetVM.Data.Id);
    }

    [RelayCommand]
    private async Task SetSelectedSessionPresetAsync(ChatPresetItemViewModel presetVM)
    {
        foreach (var item in SessionPresets)
        {
            item.IsSelected = presetVM != null && item.Equals(presetVM);
        }

        if (presetVM != null)
        {
            SetSelectedChatServiceCommand.Execute(default);
            SetSelectedAgentCommand.Execute(default);
            SetSelectedGroupPresetCommand.Execute(default);
            HistoryChatSessions.Clear();
            var service = AvailableServices.FirstOrDefault(p => p.ProviderType == presetVM.Data.Provider);
            if (service == null)
            {
                await GlobalDependencies.ServiceProvider.GetRequiredService<AppViewModel>()
                        .ShowMessageDialogAsync(StringNames.CanNotLoadSessionPresetWarning);
                return;
            }

            var sessions = await _storageService.GetChatSessionsAsync(presetVM.Data.Id);
            foreach (var session in sessions)
            {
                HistoryChatSessions.Add(new ChatSessionViewModel(session, _chatClient));
            }

            CheckHistorySessionStatus();
            _chatClient.LoadChatSessions(sessions);
            SettingsToolkit.WriteLocalSetting(SettingNames.LastSelectedSessionPreset, presetVM.Data.Id);
            CreateNewSession();
            return;
        }

        SettingsToolkit.DeleteLocalSetting(SettingNames.LastSelectedSessionPreset);
    }

    private void CreateNewChatSessionInternal(ChatServiceItemViewModel serviceVM)
    {
        ExitGroupChat();
        CurrentSession?.SaveSessionToDatabaseCommand.ExecuteAsync(default);
        var defaultModel = SettingsToolkit.ReadLocalSetting($"{serviceVM.ProviderType}DefaultChatModel", string.Empty);
        var hasModel = !string.IsNullOrEmpty(defaultModel) && (serviceVM.ServerModels.Any(p => p.Id == defaultModel) || serviceVM.CustomModels.Any(p => p.Id == defaultModel));
        if (!hasModel)
        {
            defaultModel = serviceVM.ServerModels.ToList().Concat(serviceVM.CustomModels.ToList()).FirstOrDefault()?.Id;
        }

        var newSession = _chatClient.CreateSession(serviceVM.ProviderType, modelId: defaultModel);
        CurrentSession = new ChatSessionViewModel(newSession, _chatClient);
        CurrentSession.ResetPluginsCommand.Execute(default);
    }

    private void CreateNewChatSessionInternal(ChatPresetItemViewModel preset)
    {
        ExitGroupChat();
        CurrentSession?.SaveSessionToDatabaseCommand.ExecuteAsync(default);
        var newSession = _chatClient.CreateSession(preset.Data);
        CurrentSession = new ChatSessionViewModel(newSession, _chatClient);
        CurrentSession.Title = preset.Data.Name;
        CurrentSession.ResetPluginsCommand.Execute(default);
    }

    private void CreateNewChatGroupInternal(GroupPresetItemViewModel preset)
    {
        CurrentSession?.SaveSessionToDatabaseCommand.ExecuteAsync(default);
        CurrentSession = default;
        CurrentGroup?.SaveSessionToDatabaseCommand.ExecuteAsync(default);
        var newSession = _chatClient.CreateSession(preset.Data);
        CurrentGroup = new ChatGroupViewModel(newSession, _chatClient);
        IsGroupChat = true;
    }

    private void ExitGroupChat()
    {
        IsGroupChat = false;
        CurrentGroup?.SaveSessionToDatabaseCommand.ExecuteAsync(default);
        CurrentGroup = default;
    }
}
