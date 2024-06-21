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
        ChatPresetModuleViewModel presetModuleVM)
    {
        _chatParametersFactory = chatParametersFactory;
        _storageService = storageService;
        _chatClient = chatClient;
        _logger = logger;
        _presetModuleVM = presetModuleVM;
        ServiceColumnWidth = SettingsToolkit.ReadLocalSetting(SettingNames.ChatServicePageServiceColumnWidth, 280d);
        ExtraColumnWidth = SettingsToolkit.ReadLocalSetting(SettingNames.ChatServicePageExtraColumnWidth, 240d);
        ExtraColumnVisible = SettingsToolkit.ReadLocalSetting(SettingNames.ChatServicePageExtraColumnVisible, true);
        ExtraRowHeight = SettingsToolkit.ReadLocalSetting(SettingNames.ChatServicePageExtraRowHeight, 400d);
        PanelType = SettingsToolkit.ReadLocalSetting(SettingNames.ChatSessionPanelType, ChatSessionPanelType.SystemInstruction);
        CheckPanelType();
        IsServiceSectionVisible = true;
        IsAvailableServicesEmpty = AvailableServices.Count == 0;
        IsLocalModelsEmpty = LocalModels.Count == 0;
        IsAgentsEmpty = AgentPresets.Count == 0;
        IsSessionPresetsEmpty = SessionPresets.Count == 0;
        CheckHistorySessionStatus();
        HistorySessions.CollectionChanged += OnHistorySessionsCountChanged;
        Plugins.CollectionChanged += OnPluginsCountChanged;
        CheckPluginsCount();

        AttachIsRunningToAsyncCommand(p => IsPluginLoading = p, ResetPluginsCommand);
    }

    private void CheckPanelType()
    {
        IsSystemInstructionVisible = PanelType == ChatSessionPanelType.SystemInstruction;
        IsSessionOptionsVisible = PanelType == ChatSessionPanelType.SessionOptions;
    }

    private void OnHistorySessionsCountChanged(object sender, NotifyCollectionChangedEventArgs e)
        => CheckHistorySessionStatus();

    private void CheckHistorySessionStatus()
        => IsHistorySessionsEmpty = HistorySessions.Count == 0;

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

    partial void OnPanelTypeChanged(ChatSessionPanelType value)
    {
        SettingsToolkit.WriteLocalSetting(SettingNames.ChatSessionPanelType, value);
        CheckPanelType();
    }
}
