// Copyright (c) Richasy. All rights reserved.

using Microsoft.Extensions.AI;
using Richasy.AgentKernel;
using Richasy.AgentKernel.Chat;
using Richasy.WinUIKernel.AI.ViewModels;
using RodelAgent.Interfaces;
using RodelAgent.Models.Constants;
using RodelAgent.Models.Feature;
using RodelAgent.UI.ViewModels.Items;

namespace RodelAgent.UI.ViewModels.Core;

/// <summary>
/// Chat session view model.
/// </summary>
public sealed partial class ChatSessionViewModel
{
    private readonly ILogger<ChatSessionViewModel> _logger;
    private readonly IStorageService _storageService;
    private DispatcherTimer? _tokenTimer;
    private IChatService? _chatService;
    private CancellationTokenSource? _cancellationTokenSource;
    private WebView2? _webView;
    private ChatConversation? _currentConversation;
    private Func<ChatOptions?>? _getSessionCurrentOptions;
    private Func<bool>? _getSessionIsStreamOutput;
    private Func<int>? _getSessionMaxRounds;
    private Func<int>? _getGroupMaxRounds;
    private int _currentAgentIndex;
    private DateTimeOffset? _lastInputTime;

    public event EventHandler RequestFocusInput;

    public event EventHandler RequestCloseFlyout;

    public event EventHandler RequestReloadOptionsUI;

    [ObservableProperty]
    public partial ChatProviderType? CurrentProvider { get; set; }

    [ObservableProperty]
    public partial ChatServiceItemViewModel? SelectedService { get; set; }

    [ObservableProperty]
    public partial ChatAgent? CurrentAgent { get; set; }

    [ObservableProperty]
    public partial ChatGroup? CurrentGroup { get; set; }

    [ObservableProperty]
    public partial ChatOptions? CurrentOptions { get; set; }

    [ObservableProperty]
    public partial string? Title { get; set; }

    [ObservableProperty]
    public partial string? Subtitle { get; set; }

    [ObservableProperty]
    public partial ChatModelItemViewModel? SelectedModel { get; set; }

    [ObservableProperty]
    public partial bool IsService { get; set; }

    [ObservableProperty]
    public partial bool IsAgent { get; set; }

    [ObservableProperty]
    public partial bool IsGroup { get; set; }

    [ObservableProperty]
    public partial AgentSectionType SectionType { get; set; }

    [ObservableProperty]
    public partial bool IsGenerating { get; set; }

    [ObservableProperty]
    public partial string? UserInput { get; set; }

    [ObservableProperty]
    public partial bool IsRegenerateButtonShown { get; set; }

    [ObservableProperty]
    public partial bool IsChatEmpty { get; set; }

    [ObservableProperty]
    public partial string? GeneratingTipText { get; set; }

    [ObservableProperty]
    public partial bool IsEnterSend { get; set; }

    [ObservableProperty]
    public partial bool IsWebInitializing { get; set; }

    [ObservableProperty]
    public partial bool IsWebInitialized { get; set; }

    [ObservableProperty]
    public partial bool IsHistoryInitializing { get; set; }

    [ObservableProperty]
    public partial bool IsHistoryEmpty { get; set; }

    [ObservableProperty]
    public partial double HistoryHeight { get; set; }

    [ObservableProperty]
    public partial bool IsInstructionVisible { get; set; }

    [ObservableProperty]
    public partial bool IsSessionOptionsVisible { get; set; }

    [ObservableProperty]
    public partial bool IsGroupOptionsVisible { get; set; }

    [ObservableProperty]
    public partial bool IsAgentsVisible { get; set; }

    [ObservableProperty]
    public partial string? SystemInstruction { get; set; }

    [ObservableProperty]
    public partial int TotalTokenUsage { get; set; }

    [ObservableProperty]
    public partial int InstructionTokenCount { get; set; }

    [ObservableProperty]
    public partial int UserInputWordCount { get; set; }

    [ObservableProperty]
    public partial int UserInputTokenCount { get; set; }

    [ObservableProperty]
    public partial int HistoryTokenCount { get; set; }

    [ObservableProperty]
    public partial int TotalTokenCount { get; set; }

    public ObservableCollection<ChatAgentItemViewModel> Agents { get; } = [];

    public ObservableCollection<ChatServiceItemViewModel> Services { get; } = [];

    public ObservableCollection<ChatModelItemViewModel> Models { get; } = [];

    public ObservableCollection<ChatInteropMessage> Messages { get; } = [];

    public ObservableCollection<ChatHistoryItemViewModel> History { get; } = [];

    public ObservableCollection<AIToolsetItemViewModel> Tools { get; } = [];
}
