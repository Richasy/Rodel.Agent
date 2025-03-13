// Copyright (c) Richasy. All rights reserved.

using Richasy.AgentKernel;
using Richasy.WinUIKernel.AI.ViewModels;
using RodelAgent.Interfaces;
using RodelAgent.Models.Constants;
using RodelAgent.Models.Feature;
using RodelAgent.UI.Controls.Chat;
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
        Agents.CollectionChanged += (_, _) => CheckAgentsVisible();
        Groups.CollectionChanged += (_, _) => CheckGroupsVisible();
    }

    /// <inheritdoc/>
    protected override string GetPageKey() => nameof(ChatPage);

    [RelayCommand]
    private async Task InitializeAsync()
    {
        IsInitializing = true;
        IsServicesCollapsed = SettingsToolkit.ReadLocalSetting(SettingNames.IsChatServicesCollapsed, false);
        IsAgentsCollapsed = SettingsToolkit.ReadLocalSetting(SettingNames.IsChatAgentsCollapsed, false);
        IsGroupsCollapsed = SettingsToolkit.ReadLocalSetting(SettingNames.IsChatGroupsCollapsed, false);
        if (Services == null)
        {
            this.Get<AppViewModel>().RequestReloadChatServices += (_, _) => ReloadAvailableServicesCommand.Execute(default);
            await ReloadAvailableServicesAsync();
        }

        if (!_isInitialized)
        {
            await ReloadAvailableAgentsCommand.ExecuteAsync(default);
            ReloadAvailableGroupsCommand.Execute(default);
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
    private async Task ReloadAvailableAgentsAsync()
    {
        Agents.Clear();
        var agents = await this.Get<IStorageService>().GetChatAgentsAsync();
        if (agents?.Count > 0)
        {
            foreach (var agent in agents)
            {
                Agents.Add(new(agent));
            }
        }

        CheckAgentsVisible();

        var lastSelectedSection = SettingsToolkit.ReadLocalSetting(SettingNames.LastSelectedAgentSection, AgentSectionType.Service);
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
    }

    [RelayCommand]
    private async Task ReloadAvailableGroupsAsync()
    {
        Groups.Clear();
        var groups = await this.Get<IStorageService>().GetChatGroupsAsync();
        if (groups?.Count > 0)
        {
            foreach (var group in groups)
            {
                Groups.Add(new(group));
            }
        }

        CheckGroupsVisible();
        var lastSelectedSection = SettingsToolkit.ReadLocalSetting(SettingNames.LastSelectedAgentSection, AgentSectionType.Service);
        if (lastSelectedSection == AgentSectionType.Group)
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

        DeselectAllAgents();
        DeselectAllGroups();

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

        DeselectAllServices();
        DeselectAllGroups();

        SelectedAgent = agent;

        if (agent != null)
        {
            SettingsToolkit.WriteLocalSetting(SettingNames.LastSelectedAgentSection, AgentSectionType.Agent);
            SettingsToolkit.WriteLocalSetting(SettingNames.LastSelectedAgent, agent.Data.Id);
            _sessionViewModel.InitializeWithAgentCommand.Execute(agent);
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

        DeselectAllServices();
        DeselectAllAgents();

        if (group != null)
        {
            SettingsToolkit.WriteLocalSetting(SettingNames.LastSelectedAgentSection, AgentSectionType.Group);
            SettingsToolkit.WriteLocalSetting(SettingNames.LastSelectedGroup, group.Data.Id);
            _sessionViewModel.InitializeWithGroupCommand.Execute(group);
        }
    }

    [RelayCommand]
    private async Task CreateAgentAsync()
    {
        var lastProvider = Services?.FirstOrDefault(p => p.IsSelected)?.ProviderType ?? ChatProviderType.OpenAI;
        var tempAgent = new ChatAgent
        {
            Id = Guid.NewGuid().ToString("N"),
            Provider = lastProvider,
            UseStreamOutput = true,
            MaxRounds = 0,
        };

        var vm = new ChatAgentItemViewModel(tempAgent);
        this.Get<ChatAgentConfigViewModel>().SetData(vm);
        var dialog = new ChatAgentConfigDialog();
        await dialog.ShowAsync();
    }

    [RelayCommand]
    private async Task CreateGroupAsync()
    {
        var tempGroup = new ChatGroup
        {
            Id = Guid.NewGuid().ToString("N"),
            Agents = [],
        };

        var vm = new ChatGroupItemViewModel(tempGroup);
        this.Get<ChatGroupConfigViewModel>().SetData(vm);
        var dialog = new ChatGroupConfigDialog();
        await dialog.ShowAsync();
    }

    private void CheckAgentsVisible()
        => IsAgentListVisible = Agents.Count > 0;

    private void CheckGroupsVisible()
        => IsGroupListVisible = Groups.Count > 0;

    private void DeselectAllServices()
    {
        foreach (var item in Services!)
        {
            item.IsSelected = false;
        }
    }

    private void DeselectAllAgents()
    {
        foreach (var item in Agents)
        {
            item.IsSelected = false;
        }
    }

    private void DeselectAllGroups()
    {
        foreach (var item in Groups)
        {
            item.IsSelected = false;
        }
    }

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

    partial void OnIsServicesCollapsedChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.IsChatServicesCollapsed, value);

    partial void OnIsAgentsCollapsedChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.IsChatAgentsCollapsed, value);

    partial void OnIsGroupsCollapsedChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.IsChatGroupsCollapsed, value);
}
