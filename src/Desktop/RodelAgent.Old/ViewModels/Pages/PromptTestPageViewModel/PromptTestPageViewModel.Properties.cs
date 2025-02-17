// Copyright (c) Richasy. All rights reserved.

using RodelAgent.Interfaces;
using RodelAgent.UI.ViewModels.Items;
using RodelChat.Models.Client;

namespace RodelAgent.UI.ViewModels.Pages;

/// <summary>
/// 提示词测试页面视图模型.
/// </summary>
public sealed partial class PromptTestPageViewModel
{
    private readonly ILogger<PromptTestPageViewModel> _logger;
    private readonly IChatParametersFactory _chatParametersFactory;
    private readonly IStorageService _storageService;
    private readonly IChatClient _chatClient;

    private List<ChatMessage>? _predefinedMessages;
    private Dictionary<string, List<string>>? _variables;
    private List<string>? _inputs;
    private string _defaultInputVariable = string.Empty;
    private string? _lastContext;
    private DispatcherTimer? _generateTimer;

    [ObservableProperty]
    private double _extraColumnWidth;

    [ObservableProperty]
    private bool _isAvailableServicesEmpty;

    [ObservableProperty]
    private string? _messageJsonFilePath;

    [ObservableProperty]
    private PromptTestSystemPromptItemViewModel? _currentSystemPrompt;

    [ObservableProperty]
    private int _messageCount;

    [ObservableProperty]
    private bool _isVariableEmpty;

    [ObservableProperty]
    private int _presetVariablesCount;

    [ObservableProperty]
    private string? _presetVariablesFilePath;

    [ObservableProperty]
    private int _inputsCount;

    [ObservableProperty]
    private string? _inputFilePath;

    [ObservableProperty]
    private ChatPresetItemViewModel? _preset;

    [ObservableProperty]
    private ChatServiceItemViewModel _selectedService;

    [ObservableProperty]
    private ChatModelItemViewModel _selectedModel;

    [ObservableProperty]
    private List<ChatServiceItemViewModel> _availableServices;

    [ObservableProperty]
    private List<ChatModelItemViewModel> _availableModels;

    [ObservableProperty]
    private List<PromptTestSystemPromptItemViewModel> _systemPrompts;

    [ObservableProperty]
    private List<PromptTestSessionItemViewModel>? _sessions;

    [ObservableProperty]
    private string _userPromptTemplate;

    [ObservableProperty]
    private bool _isGenerating;

    [ObservableProperty]
    private bool _isFinished;

    [ObservableProperty]
    private bool _isSessionsEmpty;

    /// <summary>
    /// 系统提示初始化完成事件.
    /// </summary>
    public event EventHandler SystemPromptInitialzied;

    /// <summary>
    /// 变量列表.
    /// </summary>
    public ObservableCollection<VariableItemViewModel> Variables { get; } = new();
}
