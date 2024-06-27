// Copyright (c) Rodel. All rights reserved.

using RodelAgent.UI.Controls.Chat;
using RodelAgent.UI.Models.Constants;
using RodelAgent.UI.Toolkits;
using RodelAgent.UI.ViewModels.Components;
using RodelAgent.UI.ViewModels.Items;
using RodelChat.Models.Client;

namespace RodelAgent.UI.ViewModels.Pages;

/// <summary>
/// 聊天服务页面视图模型的群组部分.
/// </summary>
public sealed partial class ChatServicePageViewModel
{
    [RelayCommand]
    private async Task ResetGroupsAsync()
    {
        GroupPresets.Clear();
        var localPresets = await _storageService.GetChatGroupPresetsAsync();
        foreach (var preset in localPresets)
        {
            GroupPresets.Add(new GroupPresetItemViewModel(preset));
        }

        if (SettingsToolkit.IsSettingKeyExist(SettingNames.LastSelectedGroup))
        {
            var lastSelectedGroup = SettingsToolkit.ReadLocalSetting(SettingNames.LastSelectedGroup, string.Empty);
            var group = GroupPresets.FirstOrDefault(p => p.Data.Id == lastSelectedGroup);
            SetSelectedGroupPresetCommand.Execute(group);
        }

        IsGroupsEmpty = GroupPresets.Count == 0;
    }

    [RelayCommand]
    private async Task AddGroupAsync()
    {
        if (AgentPresets.Count < 2)
        {
            var appVM = GlobalDependencies.ServiceProvider.GetService<AppViewModel>();
            await appVM.ShowMessageDialogAsync(StringNames.CanNotCreateGroupWithAgents);
            return;
        }

        var tempGroup = new ChatGroupPreset
        {
            Id = Guid.NewGuid().ToString("N"),
            Agents = new List<string>(),
            MaxRounds = 1,
        };

        var vm = new GroupPresetItemViewModel(tempGroup);
        await _groupPresetModuleVM.SetDataAsync(vm);
        var dialog = new GroupPresetSettingsDialog();
        await dialog.ShowAsync();

        var preset = dialog.ViewModel.Data;
        if (preset is not null && !_groupPresetModuleVM.IsManualClose)
        {
            if (!GroupPresets.Any(p => p.Data.Id == preset.Data.Id))
            {
                GroupPresets.Add(new GroupPresetItemViewModel(preset.Data));
                await _storageService.AddOrUpdateChatGroupPresetAsync(preset.Data);
            }
        }

        IsGroupsEmpty = GroupPresets.Count == 0;
    }

    [RelayCommand]
    private async Task SetSelectedGroupPresetAsync(GroupPresetItemViewModel presetVM)
    {
        foreach (var item in GroupPresets)
        {
            item.IsSelected = presetVM != null && item.Equals(presetVM);
        }

        if (presetVM != null)
        {
            SetSelectedChatServiceCommand.Execute(default);
            SetSelectedSessionPresetCommand.Execute(default);
            SetSelectedAgentCommand.Execute(default);
            HistoryGroupSessions.Clear();

            var sessions = await _storageService.GetChatGroupSessionsAsync(presetVM.Data.Id);
            foreach (var session in sessions)
            {
                HistoryGroupSessions.Add(new ChatGroupViewModel(session, _chatClient));
            }

            CheckHistorySessionStatus();
            _chatClient.LoadGroupSessions(sessions);
            SettingsToolkit.WriteLocalSetting(SettingNames.LastSelectedGroup, presetVM.Data.Id);
            CreateNewSession();
            return;
        }

        SettingsToolkit.DeleteLocalSetting(SettingNames.LastSelectedGroup);
    }

    [RelayCommand]
    private async Task EditGroupAsync(GroupPresetItemViewModel presetVM)
    {
        if (presetVM == null)
        {
            return;
        }

        var newVM = new GroupPresetItemViewModel(presetVM.Data);
        await _groupPresetModuleVM.SetDataAsync(newVM);
        var dialog = new GroupPresetSettingsDialog();
        await dialog.ShowAsync();

        var preset = dialog.ViewModel.Data;
        if (preset is not null && !_groupPresetModuleVM.IsManualClose)
        {
            presetVM.Data = preset.Data;
            await _storageService.AddOrUpdateChatGroupPresetAsync(preset.Data);
        }
    }

    [RelayCommand]
    private async Task DeleteGroupAsync(GroupPresetItemViewModel presetVM)
    {
        if (presetVM == null)
        {
            return;
        }

        if (presetVM.IsSelected && AvailableServices.Count > 0)
        {
            SetSelectedChatServiceCommand.Execute(AvailableServices.FirstOrDefault());
        }

        GroupPresets.Remove(presetVM);
        IsGroupsEmpty = GroupPresets.Count == 0;
        await _storageService.RemoveChatGroupPresetAsync(presetVM.Data.Id);
    }

    [RelayCommand]
    private void CheckCurrentGroupExist(ChatGroupViewModel vm)
    {
        if (CurrentGroup != vm)
        {
            return;
        }

        if (!string.IsNullOrEmpty(vm.Data.PresetId)
            && GroupPresets.FirstOrDefault(p => p.IsSelected)?.Data.Id != CurrentGroup.Data.PresetId)
        {
            return;
        }

        var sourceSession = HistoryGroupSessions.FirstOrDefault(p => p.SessionId == CurrentGroup.SessionId);
        if (sourceSession != null)
        {
            sourceSession.Title = CurrentGroup.Title;
            sourceSession.Data.Title = CurrentGroup.Title;
            return;
        }

        HistoryGroupSessions.Insert(0, CurrentGroup);
        SetSelectedGroupSession(CurrentGroup);
    }

    [RelayCommand]
    private void SetSelectedGroupSession(ChatGroupViewModel groupVM)
    {
        foreach (var item in HistoryGroupSessions)
        {
            item.IsSelected = groupVM != null && item.Equals(groupVM);
        }

        if (groupVM != null && CurrentGroup != groupVM)
        {
            CurrentGroup = groupVM;
        }
    }

    [RelayCommand]
    private async Task RemoveGroupAsync(ChatGroupViewModel groupVM)
    {
        if (groupVM == null)
        {
            return;
        }

        if (CurrentGroup == groupVM)
        {
            CreateNewSession();
        }

        groupVM.CancelMessageCommand.Execute(default);
        await _storageService.RemoveChatGroupSessionAsync(groupVM.SessionId);
        HistoryGroupSessions.Remove(groupVM);
    }

    [RelayCommand]
    private async Task RemoveAllGroupsAsync()
    {
        foreach (var session in HistoryGroupSessions)
        {
            session.IsSelected = false;
            session.CancelMessageCommand.Execute(default);
            await _storageService.RemoveChatGroupSessionAsync(session.Data.Id);
        }

        CurrentGroup = default;
        HistoryGroupSessions.Clear();
        CreateNewSessionCommand.Execute(default);
    }

    [RelayCommand]
    private void ReloadGroupAgents(string agentId)
    {
        foreach (var group in HistoryGroupSessions)
        {
            if (group.Agents.Any(p => p.Data.Id == agentId))
            {
                group.InitializeAgentsCommand.Execute(default);
            }
        }
    }
}
