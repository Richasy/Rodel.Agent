// Copyright (c) Richasy. All rights reserved.

using Microsoft.Extensions.AI;
using Richasy.AgentKernel;
using Richasy.AgentKernel.Chat;
using Richasy.WinUIKernel.AI.ViewModels;
using RodelAgent.Models.Constants;

namespace RodelAgent.UI.ViewModels.Core;

/// <summary>
/// Chat session view model.
/// </summary>
public sealed partial class ChatSessionViewModel
{
    private readonly ILogger<ChatSessionViewModel> _logger;
    private readonly List<ChatMessage> _currentHistory = [];
    private IChatService? _chatService;
    private CancellationTokenSource? _cancellationTokenSource;
    private WebView2? _webView;

    public event EventHandler RequestFocusInput;

    public event EventHandler RequestCloseFlyout;

    [ObservableProperty]
    public partial ChatProviderType? CurrentProvider { get; set; }

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

    public ObservableCollection<ChatModelItemViewModel> Models { get; } = [];
}
