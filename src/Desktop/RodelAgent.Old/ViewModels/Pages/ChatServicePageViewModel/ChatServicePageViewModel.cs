// Copyright (c) Richasy. All rights reserved.

using RodelAgent.Interfaces;
using RodelAgent.UI.Models.Constants;
using RodelAgent.UI.Pages;
using RodelAgent.UI.Toolkits;
using RodelAgent.UI.ViewModels.Components;
using System.Collections.Specialized;

namespace RodelAgent.UI.ViewModels.Pages;

/// <summary>
/// 聊天服务页面视图模型.
/// </summary>
public sealed partial class ChatServicePageViewModel : LayoutPageViewModelBase
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
        ExtraColumnWidth = SettingsToolkit.ReadLocalSetting(SettingNames.ChatServicePageExtraColumnWidth, 240d);
        ExtraColumnVisible = SettingsToolkit.ReadLocalSetting(SettingNames.ChatServicePageExtraColumnVisible, true);
        ExtraRowHeight = SettingsToolkit.ReadLocalSetting(SettingNames.ChatServicePageExtraRowHeight, 400d);
        SessionPanelType = SettingsToolkit.ReadLocalSetting(SettingNames.ChatSessionPanelType, ChatSessionPanelType.SystemInstruction);
        GroupPanelType = SettingsToolkit.ReadLocalSetting(SettingNames.ChatGroupPanelType, ChatGroupPanelType.Agents);
        IsExtraColumnManualHide = SettingsToolkit.ReadLocalSetting(SettingNames.IsChatServicePageExtraColumnManualHide, false);
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

        if (_tokenTimer is null)
        {
            _tokenTimer = new DispatcherTimer();
            _tokenTimer.Interval = TimeSpan.FromMilliseconds(400);
            _tokenTimer.Tick += OnTokenTimerTick;
            _tokenTimer.Start();
        }
    }

    /// <inheritdoc/>
    protected override string GetPageKey()
        => nameof(ChatServicePage);

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

    partial void OnCurrentSessionChanged(ChatSessionViewModel value)
    {
        if (value is null)
        {
            return;
        }

        value.EnterViewCommand.Execute(default);
    }

    partial void OnIsExtraColumnManualHideChanged(bool value)
    {
        SettingsToolkit.WriteLocalSetting(SettingNames.IsChatServicePageExtraColumnManualHide, value);
        ExtraColumnWidth = value ? 0 : SettingsToolkit.ReadLocalSetting(SettingNames.ChatServicePageExtraColumnWidth, 240d);
    }
}
