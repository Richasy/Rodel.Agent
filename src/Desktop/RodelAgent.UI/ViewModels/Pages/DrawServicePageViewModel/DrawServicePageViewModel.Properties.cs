// Copyright (c) Rodel. All rights reserved.

using RodelAgent.Interfaces;
using RodelAgent.UI.ViewModels.Components;
using RodelAgent.UI.ViewModels.Items;
using RodelDraw.Models.Client;

namespace RodelAgent.UI.ViewModels.Pages;

/// <summary>
/// 绘图服务页面视图模型.
/// </summary>
public sealed partial class DrawServicePageViewModel
{
    private readonly IStorageService _storageService;
    private readonly ILogger<DrawServicePageViewModel> _logger;

    [ObservableProperty]
    private bool _isAvailableServicesEmpty;

    [ObservableProperty]
    private bool _isHistoryEmpty;

    [ObservableProperty]
    private int _historyCount;

    /// <summary>
    /// 会话模型.
    /// </summary>
    public DrawSessionViewModel Session { get; }

    /// <summary>
    /// 可用的绘图服务.
    /// </summary>
    public ObservableCollection<DrawServiceItemViewModel> AvailableServices { get; } = new();

    /// <summary>
    /// 生成历史.
    /// </summary>
    public ObservableCollection<DrawSession> History { get; } = new();
}
