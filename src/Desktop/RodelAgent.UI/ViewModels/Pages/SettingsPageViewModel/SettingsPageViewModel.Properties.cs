// Copyright (c) Rodel. All rights reserved.

using RodelAgent.Interfaces;
using RodelAgent.UI.ViewModels.Items;

namespace RodelAgent.UI.ViewModels.Pages;

/// <summary>
/// 设置页面视图模型.
/// </summary>
public sealed partial class SettingsPageViewModel
{
    private const int BuildYear = 2024;
    private readonly ILogger<SettingsPageViewModel> _logger;
    private readonly IStorageService _storageService;

    private bool _shouldSaveChatServices;
    private bool _shouldSaveTranslateServices;
    private bool _shouldSaveDrawServices;
    private bool _shouldSaveAudioServices;

    [ObservableProperty]
    private ElementTheme _appTheme;

    [ObservableProperty]
    private string _appThemeText;

    [ObservableProperty]
    private bool _useStreamOutput;

    [ObservableProperty]
    private string _appVersion;

    [ObservableProperty]
    private string _copyright;

    [ObservableProperty]
    private string _workingDirectory;

    [ObservableProperty]
    private bool _shouldRecordTranslate;

    [ObservableProperty]
    private string _appLanguage;

    [ObservableProperty]
    private bool _hiddenWhenCloseWindow;

    /// <summary>
    /// 在线聊天服务.
    /// </summary>
    public ObservableCollection<ChatServiceItemViewModel> OnlineChatServices { get; } = new();

    /// <summary>
    /// 翻译服务.
    /// </summary>
    public ObservableCollection<TranslateServiceItemViewModel> OnlineTranslateServices { get; } = new();

    /// <summary>
    /// 绘图服务.
    /// </summary>
    public ObservableCollection<DrawServiceItemViewModel> OnlineDrawServices { get; } = new();

    /// <summary>
    /// 在线音频服务.
    /// </summary>
    public ObservableCollection<AudioServiceItemViewModel> OnlineAudioServices { get; } = new();
}
