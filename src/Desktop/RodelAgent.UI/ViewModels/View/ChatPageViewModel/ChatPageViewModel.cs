// Copyright (c) Richasy. All rights reserved.

using Richasy.AgentKernel;
using Richasy.WinUIKernel.AI.ViewModels;
using RodelAgent.Models.Constants;
using RodelAgent.UI.Models.Constants;
using RodelAgent.UI.Pages;
using RodelAgent.UI.Toolkits;
using RodelAgent.UI.ViewModels.Core;

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
        if (Services == null)
        {
            this.Get<AppViewModel>().RequestReloadChatServices += (_, _) => ReloadAvailableServicesCommand.Execute(default);
            await ReloadAvailableServicesAsync();
        }
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
