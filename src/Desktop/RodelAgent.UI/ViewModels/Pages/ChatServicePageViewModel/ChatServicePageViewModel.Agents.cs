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
/// 聊天服务页面视图模型的助理部分.
/// </summary>
public sealed partial class ChatServicePageViewModel
{
    [RelayCommand]
    private async Task ResetAgentsAsync()
    {
        AgentPresets.Clear();
        var localPresets = await _storageService.GetChatAgentsAsync();
        foreach (var preset in localPresets)
        {
            AgentPresets.Add(new ChatPresetItemViewModel(preset));
        }

        if (SettingsToolkit.IsSettingKeyExist(SettingNames.LastSelectedAgent))
        {
            var lastSelectedAgent = SettingsToolkit.ReadLocalSetting(SettingNames.LastSelectedAgent, string.Empty);
            var agent = AgentPresets.FirstOrDefault(p => p.Data.Id == lastSelectedAgent);
            SetSelectedAgentCommand.Execute(agent);
        }

        IsAgentsEmpty = AgentPresets.Count == 0;
    }

    [RelayCommand]
    private async Task AddAgentAsync()
    {
        var lastProvider = AvailableServices.FirstOrDefault(p => p.IsSelected)?.ProviderType
            ?? AvailableServices.FirstOrDefault()?.ProviderType
            ?? ProviderType.OpenAI;
        var tempAgent = new ChatSessionPreset
        {
            Id = Guid.NewGuid().ToString("N"),
            Provider = lastProvider,
            Parameters = _chatParametersFactory.CreateChatParameters(lastProvider),
        };

        var vm = new ChatPresetItemViewModel(tempAgent);
        _chatPresetModuleVM.SetData(vm, ChatSessionPresetType.Agent);
        var dialog = new ChatPresetSettingsDialog();
        await dialog.ShowAsync();

        var preset = dialog.ViewModel.Data;
        if (preset is not null && !_chatPresetModuleVM.IsManualClose)
        {
            if (!AgentPresets.Any(p => p.Data.Id == preset.Data.Id))
            {
                AgentPresets.Add(new ChatPresetItemViewModel(preset.Data));
                await _storageService.AddOrUpdateChatAgentAsync(preset.Data);
            }
        }

        IsAgentsEmpty = AgentPresets.Count == 0;
    }

    [RelayCommand]
    private async Task CreateAgentCopyAsync(ChatPresetItemViewModel presetVM)
    {
        var tempAgent = presetVM.Data.Clone();
        tempAgent.Id = Guid.NewGuid().ToString("N");

        var vm = new ChatPresetItemViewModel(tempAgent);
        _chatPresetModuleVM.SetData(vm, ChatSessionPresetType.Agent);
        var dialog = new ChatPresetSettingsDialog();
        await dialog.ShowAsync();

        var preset = dialog.ViewModel.Data;
        if (preset is not null && !_chatPresetModuleVM.IsManualClose)
        {
            if (!AgentPresets.Any(p => p.Data.Id == preset.Data.Id))
            {
                AgentPresets.Add(new ChatPresetItemViewModel(preset.Data));
                await _storageService.AddOrUpdateChatAgentAsync(preset.Data);
            }
        }

        IsAgentsEmpty = AgentPresets.Count == 0;
    }

    [RelayCommand]
    private async Task SetSelectedAgentAsync(ChatPresetItemViewModel presetVM)
    {
        foreach (var item in AgentPresets)
        {
            item.IsSelected = presetVM != null && item.Equals(presetVM);
        }

        if (presetVM != null)
        {
            SetSelectedChatServiceCommand.Execute(default);
            SetSelectedSessionPresetCommand.Execute(default);
            SetSelectedGroupPresetCommand.Execute(default);
            HistoryChatSessions.Clear();
            var service = AvailableServices.FirstOrDefault(p => p.ProviderType == presetVM.Data.Provider);
            if (service == null)
            {
                await this.Get<AppViewModel>()
                        .ShowMessageDialogAsync(StringNames.CanNotLoadAgentWarning);
                return;
            }

            var sessions = await _storageService.GetChatSessionsAsync(presetVM.Data.Id);
            foreach (var session in sessions)
            {
                HistoryChatSessions.Add(new ChatSessionViewModel(session, _chatClient));
            }

            CheckHistorySessionStatus();
            _chatClient.LoadChatSessions(sessions);
            SettingsToolkit.WriteLocalSetting(SettingNames.LastSelectedAgent, presetVM.Data.Id);
            CreateNewSession();
            return;
        }

        SettingsToolkit.DeleteLocalSetting(SettingNames.LastSelectedAgent);
    }

    [RelayCommand]
    private async Task EditAgentAsync(ChatPresetItemViewModel presetVM)
    {
        if (presetVM == null)
        {
            return;
        }

        var newVM = new ChatPresetItemViewModel(presetVM.Data);
        _chatPresetModuleVM.SetData(newVM, ChatSessionPresetType.Agent);
        var dialog = new ChatPresetSettingsDialog();
        await dialog.ShowAsync();

        var preset = dialog.ViewModel.Data;
        if (preset is not null && !_chatPresetModuleVM.IsManualClose)
        {
            presetVM.Data = preset.Data;
            await _storageService.AddOrUpdateChatAgentAsync(preset.Data);
        }
    }

    [RelayCommand]
    private async Task DeleteAgentAsync(ChatPresetItemViewModel presetVM)
    {
        if (presetVM == null)
        {
            return;
        }

        if (presetVM.IsSelected && AvailableServices.Count > 0)
        {
            SetSelectedChatServiceCommand.Execute(AvailableServices.FirstOrDefault());
        }

        AgentPresets.Remove(presetVM);
        IsAgentsEmpty = AgentPresets.Count == 0;
        await _storageService.RemoveChatAgentAsync(presetVM.Data.Id);
    }
}
