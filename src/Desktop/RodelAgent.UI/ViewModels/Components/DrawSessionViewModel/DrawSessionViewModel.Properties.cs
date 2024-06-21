// Copyright (c) Rodel. All rights reserved.

using System.Threading;
using RodelAgent.Interfaces;
using RodelAgent.UI.ViewModels.Items;
using RodelDraw.Interfaces.Client;
using RodelDraw.Models.Client;

namespace RodelAgent.UI.ViewModels.Components;

/// <summary>
/// 绘图会话视图模型.
/// </summary>
public sealed partial class DrawSessionViewModel
{
    private readonly IDrawClient _drawClient;
    private readonly ILogger<DrawSessionViewModel> _logger;
    private readonly IStorageService _storageService;
    private CancellationTokenSource _cancellationTokenSource;

    [ObservableProperty]
    private string _size;

    [ObservableProperty]
    private string _prompt;

    [ObservableProperty]
    private string _negativePrompt;

    [ObservableProperty]
    private bool _isNegativePromptVisible;

    [ObservableProperty]
    private bool _isDrawing;

    [ObservableProperty]
    private string _imagePath;

    [ObservableProperty]
    private DrawServiceItemViewModel _drawService;

    [ObservableProperty]
    private bool _isEnterSend;

    [ObservableProperty]
    private string _lastGenerateTime;

    [ObservableProperty]
    private bool _isDrawParametersShown;

    [ObservableProperty]
    private bool _isDrawHistoryShown;

    [ObservableProperty]
    private string _errorText;

    /// <summary>
    /// 当前加载的绘图会话改变事件.
    /// </summary>
    public event EventHandler<DrawSession> DataChanged;

    /// <summary>
    /// 支持的绘图尺寸.
    /// </summary>
    public ObservableCollection<string> Sizes { get; } = new();

    /// <summary>
    /// 绘图模型列表.
    /// </summary>
    public ObservableCollection<DrawModelItemViewModel> Models { get; } = new();
}
