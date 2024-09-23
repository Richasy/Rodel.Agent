// Copyright (c) Rodel. All rights reserved.

using System.Threading;
using Microsoft.UI.Dispatching;
using RodelAgent.Interfaces;
using RodelAgent.UI.ViewModels.Items;
using RodelChat.Interfaces.Client;

namespace RodelAgent.UI.ViewModels.Components;

/// <summary>
/// 聊天会话视图模型.
/// </summary>
public sealed partial class ChatSessionViewModel
{
    private readonly DispatcherQueue _dispatcherQueue;
    private readonly IChatClient _chatClient;
    private readonly IStorageService _storageService;
    private readonly ILogger<ChatSessionViewModel> _logger;
    private CancellationTokenSource _cancellationTokenSource;

    private int _baseTokenCount;

    [ObservableProperty]
    private ChatServiceItemViewModel _chatService;

    [ObservableProperty]
    private string _title;

    [ObservableProperty]
    private string _model;

    [ObservableProperty]
    private string _userInput;

    [ObservableProperty]
    private string _tempMessage;

    [ObservableProperty]
    private string _errorText;

    [ObservableProperty]
    private bool _isResponding;

    [ObservableProperty]
    private bool _isChatEmpty;

    [ObservableProperty]
    private bool _isEnterSend;

    [ObservableProperty]
    private bool _isRegenerateButtonShown;

    [ObservableProperty]
    private string _lastMessageTime;

    [ObservableProperty]
    private bool _isSelected;

    [ObservableProperty]
    private bool _isSupportTool;

    [ObservableProperty]
    private bool _isSupportVision;

    [ObservableProperty]
    private int _maxRounds;

    [ObservableProperty]
    private bool _isSessionPreset;

    [ObservableProperty]
    private bool _isAgentPreset;

    [ObservableProperty]
    private bool _isNormalSession;

    [ObservableProperty]
    private string _generatingTipText;

    [ObservableProperty]
    private int _totalTokenUsage;

    [ObservableProperty]
    private int _remainderTokenCount;

    [ObservableProperty]
    private int _systemTokenCount;

    [ObservableProperty]
    private int _userInputWordCount;

    [ObservableProperty]
    private int _userInputTokenCount;

    [ObservableProperty]
    private int _totalTokenCount;

    /// <summary>
    /// 请求滚动到底部.
    /// </summary>
    public event EventHandler RequestScrollToBottom;

    /// <summary>
    /// 请求聚焦于输入框.
    /// </summary>
    public event EventHandler RequestFocusInput;

    /// <summary>
    /// 会话标识符.
    /// </summary>
    public string SessionId => Data?.Id ?? string.Empty;

    /// <summary>
    /// 会话预设模型（仅用于绑定）.
    /// </summary>
    public ChatPresetItemViewModel PresetViewModel { get; }

    /// <summary>
    /// 消息列表.
    /// </summary>
    public ObservableCollection<ChatMessageItemViewModel> Messages { get; } = new();

    /// <summary>
    /// 聊天模型列表.
    /// </summary>
    public ObservableCollection<ChatModelItemViewModel> Models { get; } = new();

    /// <summary>
    /// 插件列表.
    /// </summary>
    public ObservableCollection<ChatPluginItemViewModel> Plugins { get; } = new();
}
