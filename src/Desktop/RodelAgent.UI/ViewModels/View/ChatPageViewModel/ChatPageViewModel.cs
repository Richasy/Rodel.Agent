// Copyright (c) Richasy. All rights reserved.

using Richasy.AgentKernel;
using Richasy.WinUIKernel.AI.ViewModels;
using RodelAgent.Interfaces;
using RodelAgent.Models.Constants;
using RodelAgent.UI.Models.Constants;
using RodelAgent.UI.Pages;
using RodelAgent.UI.Toolkits;
using RodelAgent.UI.ViewModels.Core;
using RodelAgent.UI.ViewModels.Items;

namespace RodelAgent.UI.ViewModels.View;

/// <summary>
/// 对话页面视图模型.
/// </summary>
public sealed partial class ChatPageViewModel : LayoutPageViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChatPageViewModel"/> class.
    /// </summary>
    public ChatPageViewModel(ChatSessionViewModel sessionVM)
    {
        _sessionViewModel = sessionVM;
        IsAgentSectionVisible = true;
        IsToolSectionVisible = false;
    }

    /// <inheritdoc/>
    protected override string GetPageKey() => nameof(ChatPage);

    [RelayCommand]
    private async Task InitializeAsync()
    {
        IsInitializing = true;
        if (Services == null)
        {
            this.Get<AppViewModel>().RequestReloadChatServices += (_, _) => ReloadAvailableServicesCommand.Execute(default);
            await ReloadAvailableServicesAsync();
        }

        if (!_isInitialized)
        {
            var lastSelectedSection = SettingsToolkit.ReadLocalSetting(SettingNames.LastSelectedAgentSection, AgentSectionType.Service);
            var agents = await this.Get<IStorageService>().GetChatAgentsAsync();
            if (agents?.Count > 0)
            {
                foreach (var agent in agents)
                {
                    Agents.Add(new(agent));
                }
            }

            var groups = await this.Get<IStorageService>().GetChatGroupsAsync();
            if (groups?.Count > 0)
            {
                foreach (var group in groups)
                {
                    Groups.Add(new(group));
                }
            }

            CheckAgentsVisible();
            CheckGroupsVisible();
            if (lastSelectedSection == AgentSectionType.Agent)
            {
                var lastSelectedAgentId = SettingsToolkit.ReadLocalSetting(SettingNames.LastSelectedAgent, string.Empty);
                var agent = Agents.FirstOrDefault(p => p.Data.Id == lastSelectedAgentId);
                if (agent != null)
                {
                    SelectAgentCommand.Execute(agent);
                }
                else
                {
                    SelectServiceCommand.Execute(Services?.FirstOrDefault());
                }
            }
            else if (lastSelectedSection == AgentSectionType.Group)
            {
                var lastSelectedGroupId = SettingsToolkit.ReadLocalSetting(SettingNames.LastSelectedGroup, string.Empty);
                var group = Groups.FirstOrDefault(p => p.Data.Id == lastSelectedGroupId);
                if (group != null)
                {
                    SelectGroupCommand.Execute(group);
                }
                else
                {
                    SelectServiceCommand.Execute(Services?.FirstOrDefault());
                }
            }
        }

        IsInitializing = false;
        _isInitialized = true;
    }

    [RelayCommand]
    private async Task ReloadAvailableServicesAsync()
    {
        var providers = Enum.GetValues<ChatProviderType>();
        var services = new List<ChatServiceItemViewModel>();
        var chatConfigManager = this.Get<IChatConfigManager>();
        foreach (var p in providers)
        {
            var config = await chatConfigManager.GetChatConfigAsync(p);
            if (config?.IsValid() == true)
            {
                services.Add(new ChatServiceItemViewModel(p));
            }
        }

        Services = services;
        IsNoService = services.Count == 0;

        if (IsNoService)
        {
            return;
        }

        var lastSelectedSection = SettingsToolkit.ReadLocalSetting(SettingNames.LastSelectedAgentSection, AgentSectionType.Service);
        if (lastSelectedSection == AgentSectionType.Service)
        {
            var lastSelectedService = SettingsToolkit.ReadLocalSetting(SettingNames.LastSelectedChatService, ChatProviderType.OpenAI);
            var service = Services.Find(p => p.ProviderType == lastSelectedService) ?? Services[0];
            SelectServiceCommand.Execute(service);
        }
    }

    [RelayCommand]
    private void SelectService(ChatServiceItemViewModel? service)
    {
        if (IsNoService)
        {
            return;
        }

        foreach (var item in Services!)
        {
            item.IsSelected = service != null && item.ProviderType == service.ProviderType;
        }

        SelectedService = service;
        if (service != null)
        {
            SettingsToolkit.WriteLocalSetting(SettingNames.LastSelectedAgentSection, AgentSectionType.Service);
            SettingsToolkit.WriteLocalSetting(SettingNames.LastSelectedChatService, service.ProviderType);
            _sessionViewModel.InitializeWithServiceCommand.Execute(service);
        }
    }

    [RelayCommand]
    private void SelectAgent(ChatAgentItemViewModel? agent)
    {
        foreach (var item in Agents)
        {
            item.IsSelected = item.Data.Id == agent?.Data.Id;
        }

        SelectedAgent = agent;

        if (agent != null)
        {
            SettingsToolkit.WriteLocalSetting(SettingNames.LastSelectedAgentSection, AgentSectionType.Agent);
            SettingsToolkit.WriteLocalSetting(SettingNames.LastSelectedAgent, agent.Data.Id);
            // TODO: Initialize session with agent.
        }
    }

    [RelayCommand]
    private void SelectGroup(ChatGroupItemViewModel? group)
    {
        foreach (var item in Groups)
        {
            item.IsSelected = item.Data.Id == group?.Data.Id;
        }

        SelectedGroup = group;

        if (group != null)
        {
            SettingsToolkit.WriteLocalSetting(SettingNames.LastSelectedAgentSection, AgentSectionType.Group);
            SettingsToolkit.WriteLocalSetting(SettingNames.LastSelectedGroup, group.Data.Id);

            // TODO: Initialize session with group.
        }
    }

    private void CheckAgentsVisible()
        => IsAgentSectionVisible = Agents.Count > 0;

    private void CheckGroupsVisible()
        => IsGroupListVisible = Groups.Count > 0;

    partial void OnIsAgentSectionVisibleChanged(bool value)
    {
        if (value)
        {
            IsToolSectionVisible = false;
        }
    }

    partial void OnIsToolSectionVisibleChanged(bool value)
    {
        if (value)
        {
            IsAgentSectionVisible = false;
        }
    }
}
