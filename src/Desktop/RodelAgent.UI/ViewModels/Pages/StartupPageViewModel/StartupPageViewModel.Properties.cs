// Copyright (c) Rodel. All rights reserved.

using RodelAgent.Interfaces;
using RodelAgent.UI.Models.Args;
using RodelAgent.UI.ViewModels.Items;

namespace RodelAgent.UI.ViewModels.Pages;

/// <summary>
/// 启动页面视图模型.
/// </summary>
public sealed partial class StartupPageViewModel
{
    private readonly AppViewModel _appViewModel;
    private readonly ILogger<StartupPageViewModel> _logger;
    private readonly IStorageService _storageService;

    [ObservableProperty]
    private int _currentStep;

    [ObservableProperty]
    private int _stepCount;

    [ObservableProperty]
    private bool _isWelcomeStep;

    [ObservableProperty]
    private bool _isOnlineChatStep;

    [ObservableProperty]
    private bool _isOnlineTranslateStep;

    [ObservableProperty]
    private bool _isOnlineDrawStep;

    [ObservableProperty]
    private bool _isOnlineAudioStep;

    [ObservableProperty]
    private bool _isLastStep;

    [ObservableProperty]
    private bool _isPreviousStepShown;

    [ObservableProperty]
    private string _selectedFolder;

    [ObservableProperty]
    private bool _isOnlineChatInitializing;

    [ObservableProperty]
    private bool _isOnlineTranslateInitializing;

    [ObservableProperty]
    private bool _isOnlineDrawInitializing;

    [ObservableProperty]
    private bool _isOnlineAudioInitializing;

    [ObservableProperty]
    private bool _isMigrating;

    /// <summary>
    /// 侧页面导航请求.
    /// </summary>
    public event EventHandler<AppNavigationEventArgs> SideNavigationRequested;

    /// <summary>
    /// 在线聊天服务.
    /// </summary>
    public ObservableCollection<ChatServiceItemViewModel> OnlineChatServices { get; } = new();

    /// <summary>
    /// 在线音频服务.
    /// </summary>
    public ObservableCollection<AudioServiceItemViewModel> OnlineAudioServices { get; } = new();

    /// <summary>
    /// 在线绘图服务.
    /// </summary>
    public ObservableCollection<DrawServiceItemViewModel> OnlineDrawServices { get; } = new();

    /// <summary>
    /// 在线翻译服务.
    /// </summary>
    public ObservableCollection<TranslateServiceItemViewModel> OnlineTranslateServices { get; } = new();
}
