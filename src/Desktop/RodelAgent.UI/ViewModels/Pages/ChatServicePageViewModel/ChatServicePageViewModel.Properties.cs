// Copyright (c) Rodel. All rights reserved.

using RodelAgent.Interfaces;
using RodelAgent.UI.Models.Constants;
using RodelAgent.UI.ViewModels.Components;
using RodelAgent.UI.ViewModels.Items;
using RodelChat.Interfaces.Client;

namespace RodelAgent.UI.ViewModels.Pages;

/// <summary>
/// 聊天服务页面视图模型.
/// </summary>
public sealed partial class ChatServicePageViewModel
{
    private const string LOCAL_MODELS_FILE = "LocalModels.json";
    private readonly IChatParametersFactory _chatParametersFactory;
    private readonly IStorageService _storageService;
    private readonly IChatClient _chatClient;
    private readonly ILogger<ChatServicePageViewModel> _logger;
    private readonly ChatPresetModuleViewModel _presetModuleVM;
    private bool _isPluginInitialized;

    [ObservableProperty]
    private double _serviceColumnWidth;

    [ObservableProperty]
    private double _extraColumnWidth;

    [ObservableProperty]
    private bool _extraColumnVisible;

    [ObservableProperty]
    private double _extraRowHeight;

    [ObservableProperty]
    private bool _isAvailableServicesEmpty;

    [ObservableProperty]
    private bool _isHistorySessionsEmpty;

    [ObservableProperty]
    private bool _isServiceSectionVisible;

    [ObservableProperty]
    private bool _isPluginSectionVisible;

    [ObservableProperty]
    private bool _isSystemInstructionVisible;

    [ObservableProperty]
    private bool _isSessionOptionsVisible;

    [ObservableProperty]
    private bool _isLocalModelsEmpty;

    [ObservableProperty]
    private bool _isAgentsEmpty;

    [ObservableProperty]
    private bool _isSessionPresetsEmpty;

    [ObservableProperty]
    private bool _isPluginLoading;

    [ObservableProperty]
    private bool _isPluginEmpty;

    [ObservableProperty]
    private ChatSessionPanelType _panelType;

    [ObservableProperty]
    private ChatSessionViewModel _currentSession;

    [ObservableProperty]
    private bool _isDeletingPluginsNotEmpty;

    /// <summary>
    /// 可用的聊天服务.
    /// </summary>
    public ObservableCollection<ChatServiceItemViewModel> AvailableServices { get; } = new();

    /// <summary>
    /// 历史会话.
    /// </summary>
    public ObservableCollection<ChatSessionViewModel> HistorySessions { get; } = new();

    /// <summary>
    /// 本地模型.
    /// </summary>
    public ObservableCollection<ChatModelItemViewModel> LocalModels { get; } = new();

    /// <summary>
    /// 会话预设.
    /// </summary>
    public ObservableCollection<ChatPresetItemViewModel> SessionPresets { get; } = new();

    /// <summary>
    /// 本地模型预设.
    /// </summary>
    public ObservableCollection<ChatPresetItemViewModel> LocalModelPresets { get; } = new();

    /// <summary>
    /// 助理列表.
    /// </summary>
    public ObservableCollection<ChatPresetItemViewModel> AgentPresets { get; } = new();

    /// <summary>
    /// 聊天插件.
    /// </summary>
    public ObservableCollection<ChatPluginItemViewModel> Plugins { get; } = new();
}
