// Copyright (c) Rodel. All rights reserved.

using RodelAgent.Interfaces;
using RodelAgent.UI.ViewModels.Components;
using RodelAgent.UI.ViewModels.Items;
using RodelAudio.Models.Client;

namespace RodelAgent.UI.ViewModels.Pages;

/// <summary>
/// 音频服务页面视图模型.
/// </summary>
public sealed partial class AudioServicePageViewModel
{
    private readonly IStorageService _storageService;
    private readonly ILogger<AudioServicePageViewModel> _logger;

    [ObservableProperty]
    private bool _isAvailableServicesEmpty;

    [ObservableProperty]
    private double _historyColumnWidth;

    [ObservableProperty]
    private bool _isHistoryEmpty;

    [ObservableProperty]
    private int _historyCount;

    /// <summary>
    /// 会话模型.
    /// </summary>
    public AudioSessionViewModel Session { get; }

    /// <summary>
    /// 可用的绘图服务.
    /// </summary>
    public ObservableCollection<AudioServiceItemViewModel> AvailableServices { get; } = new();

    /// <summary>
    /// 生成历史.
    /// </summary>
    public ObservableCollection<AudioSession> History { get; } = new();
}
