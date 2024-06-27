// Copyright (c) Rodel. All rights reserved.

using System.Threading;
using Microsoft.UI.Dispatching;
using RodelAgent.Interfaces;
using RodelAgent.UI.ViewModels.Items;
using RodelChat.Interfaces.Client;

namespace RodelAgent.UI.ViewModels.Components;

/// <summary>
/// 聊天群组视图模型.
/// </summary>
public sealed partial class ChatGroupViewModel
{
    private readonly DispatcherQueue _dispatcherQueue;
    private readonly IChatClient _chatClient;
    private readonly IStorageService _storageService;
    private readonly ILogger<ChatSessionViewModel> _logger;
    private CancellationTokenSource _cancellationTokenSource;
    private int _currentGeneratingIndex = -1;

    [ObservableProperty]
    private string _title;

    [ObservableProperty]
    private string _groupName;

    [ObservableProperty]
    private string _userInput;

    [ObservableProperty]
    private string _errorText;

    [ObservableProperty]
    private bool _isResponding;

    [ObservableProperty]
    private bool _isChatEmpty;

    [ObservableProperty]
    private bool _isEnterSend;

    [ObservableProperty]
    private string _lastMessageTime;

    [ObservableProperty]
    private bool _isSelected;

    [ObservableProperty]
    private int _maxRounds;

    [ObservableProperty]
    private string _generatingTipText;

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
    /// 消息列表.
    /// </summary>
    public ObservableCollection<ChatMessageItemViewModel> Messages { get; } = new();

    /// <summary>
    /// 助理列表.
    /// </summary>
    public ObservableCollection<ChatPresetItemViewModel> Agents { get; } = new();

    /// <summary>
    /// 终止文本列表.
    /// </summary>
    public ObservableCollection<string> TerminateText { get; } = new();
}
