// Copyright (c) Rodel. All rights reserved.

using RodelAgent.Interfaces;
using RodelAgent.UI.ViewModels.Components;
using RodelAgent.UI.ViewModels.Items;
using RodelTranslate.Models.Client;

namespace RodelAgent.UI.ViewModels.Pages;

/// <summary>
/// 翻译页面视图模型.
/// </summary>
public sealed partial class TranslateServicePageViewModel
{
    private readonly IStorageService _storageService;
    private readonly ILogger<TranslateServicePageViewModel> _logger;

    [ObservableProperty]
    private bool _isAvailableServicesEmpty;

    [ObservableProperty]
    private bool _isHistoryShown;

    [ObservableProperty]
    private bool _isHistoryEmpty;

    /// <summary>
    /// 会话模型.
    /// </summary>
    public TranslateSessionViewModel Session { get; }

    /// <summary>
    /// 可用的翻译服务.
    /// </summary>
    public ObservableCollection<TranslateServiceItemViewModel> AvailableServices { get; } = new();

    /// <summary>
    /// 翻译会话.
    /// </summary>
    public ObservableCollection<TranslateSession> History { get; } = new();
}
