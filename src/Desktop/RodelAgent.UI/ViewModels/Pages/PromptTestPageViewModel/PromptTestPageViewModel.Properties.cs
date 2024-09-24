// Copyright (c) Rodel. All rights reserved.

using RodelAgent.Interfaces;
using RodelAgent.UI.ViewModels.Items;
using RodelChat.Interfaces.Client;
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

    // private List<ChatSession>? _sessions;
    private List<ChatMessage>? _predefinedMessages;

    [ObservableProperty]
    private double _extraColumnWidth;

    [ObservableProperty]
    private bool _isAvailableServicesEmpty;

    [ObservableProperty]
    private string? _messageJsonFilePath;

    [ObservableProperty]
    private string? _systemPromptFilePath;

    [ObservableProperty]
    private string? _systemPrompt;

    [ObservableProperty]
    private int _messageCount;

    [ObservableProperty]
    private bool _isVariableEmpty;

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

    /// <summary>
    /// 变量列表.
    /// </summary>
    public ObservableCollection<VariableItemViewModel> Variables { get; } = new();
}
