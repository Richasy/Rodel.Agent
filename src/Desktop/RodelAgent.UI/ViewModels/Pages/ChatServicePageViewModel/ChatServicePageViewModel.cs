// Copyright (c) Rodel. All rights reserved.

using System.Collections.Specialized;
using RodelAgent.Interfaces;
using RodelAgent.UI.Models.Constants;
using RodelAgent.UI.Toolkits;
using RodelAgent.UI.ViewModels.Components;
using RodelChat.Interfaces.Client;

namespace RodelAgent.UI.ViewModels.Pages;

/// <summary>
/// 聊天服务页面视图模型.
/// </summary>
public sealed partial class ChatServicePageViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChatServicePageViewModel"/> class.
    /// </summary>
    public ChatServicePageViewModel(
        IChatParametersFactory chatParametersFactory,
        IStorageService storageService,
        IChatClient chatClient,
        ILogger<ChatServicePageViewModel> logger,
        ChatPresetModuleViewModel chatPresetModuleVM,
        GroupPresetModuleViewModel groupPresetModuleVM)
    {
        _chatParametersFactory = chatParametersFactory;
        _storageService = storageService;
        _chatClient = chatClient;
        _logger = logger;
        _chatPresetModuleVM = chatPresetModuleVM;
        _groupPresetModuleVM = groupPresetModuleVM;
        ServiceColumnWidth = SettingsToolkit.ReadLocalSetting(SettingNames.ChatServicePageServiceColumnWidth, 280d);
        ExtraColumnWidth = SettingsToolkit.ReadLocalSetting(SettingNames.ChatServicePageExtraColumnWidth, 240d);
        ExtraColumnVisible = SettingsToolkit.ReadLocalSetting(SettingNames.ChatServicePageExtraColumnVisible, true);
        ExtraRowHeight = SettingsToolkit.ReadLocalSetting(SettingNames.ChatServicePageExtraRowHeight, 400d);
        SessionPanelType = SettingsToolkit.ReadLocalSetting(SettingNames.ChatSessionPanelType, ChatSessionPanelType.SystemInstruction);
        GroupPanelType = SettingsToolkit.ReadLocalSetting(SettingNames.ChatGroupPanelType, ChatGroupPanelType.Agents);
        CheckSessionPanelType();
        CheckGroupPanelType();
        IsServiceSectionVisible = true;
        IsAvailableServicesEmpty = AvailableServices.Count == 0;
        IsAgentsEmpty = AgentPresets.Count == 0;
        IsSessionPresetsEmpty = SessionPresets.Count == 0;
        IsGroupsEmpty = GroupPresets.Count == 0;
        CheckHistorySessionStatus();
        HistoryChatSessions.CollectionChanged += OnHistorySessionsCountChanged;
        HistoryGroupSessions.CollectionChanged += OnHistorySessionsCountChanged;
        Plugins.CollectionChanged += OnPluginsCountChanged;
        CheckPluginsCount();

        AttachIsRunningToAsyncCommand(p => IsPluginLoading = p, ResetPluginsCommand);
    }

    private void CheckSessionPanelType()
    {
        IsSystemInstructionVisible = SessionPanelType == ChatSessionPanelType.SystemInstruction;
        IsSessionOptionsVisible = SessionPanelType == ChatSessionPanelType.SessionOptions;
    }

    private void CheckGroupPanelType()
    {
        IsAgentsSectionVisible = GroupPanelType == ChatGroupPanelType.Agents;
        IsGroupOptionsVisible = GroupPanelType == ChatGroupPanelType.GroupOptions;
    }

    private void OnHistorySessionsCountChanged(object sender, NotifyCollectionChangedEventArgs e)
        => CheckHistorySessionStatus();

    private void CheckHistorySessionStatus()
    {
        IsChatHistorySessionsEmpty = HistoryChatSessions.Count == 0;
        IsGroupHistorySessionsEmpty = HistoryGroupSessions.Count == 0;
    }

    partial void OnServiceColumnWidthChanged(double value)
    {
        if (value > 0)
        {
            SettingsToolkit.WriteLocalSetting(SettingNames.ChatServicePageServiceColumnWidth, value);
        }
    }

    partial void OnExtraColumnWidthChanged(double value)
    {
        if (value > 0)
        {
            SettingsToolkit.WriteLocalSetting(SettingNames.ChatServicePageExtraColumnWidth, value);
        }
    }

    partial void OnExtraRowHeightChanged(double value)
    {
        if (value > 0)
        {
            SettingsToolkit.WriteLocalSetting(SettingNames.ChatServicePageExtraRowHeight, value);
        }
    }

    partial void OnExtraColumnVisibleChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.ChatServicePageExtraColumnVisible, value);

    partial void OnIsPluginSectionVisibleChanged(bool value)
    {
        if (value)
        {
            IsServiceSectionVisible = false;
            ResetPluginsCommand.Execute(default);
        }
    }

    partial void OnIsServiceSectionVisibleChanged(bool value)
    {
        if (value)
        {
            IsPluginSectionVisible = false;
        }
    }

    partial void OnSessionPanelTypeChanged(ChatSessionPanelType value)
    {
        SettingsToolkit.WriteLocalSetting(SettingNames.ChatSessionPanelType, value);
        CheckSessionPanelType();
    }

    partial void OnGroupPanelTypeChanged(ChatGroupPanelType value)
    {
        SettingsToolkit.WriteLocalSetting(SettingNames.ChatGroupPanelType, value);
        CheckGroupPanelType();
    }
}
