// Copyright (c) Richasy. All rights reserved.

using Microsoft.Extensions.AI;
using Richasy.WinUIKernel.AI.ViewModels;
using RodelAgent.Interfaces;
using RodelAgent.Models.Feature;

namespace RodelAgent.UI.ViewModels.Core;

/// <summary>
/// 聊天预设视图模型.
/// </summary>
public sealed partial class ChatAgentConfigViewModel
{
    private readonly IStorageService _storageService;
    private Func<ChatOptions?>? _getCurrentOptions;
    private Func<bool>? _getIsStreamOutput;
    private Func<int>? _getMaxRounds;
    private Func<string>? _getEmoji;

    public event EventHandler RequestReloadOptionsUI;

    public ChatAgent? Agent { get; private set; }

    [ObservableProperty]
    public partial string? Name { get; set; }

    [ObservableProperty]
    public partial string? SystemInstruction { get; set; }

    [ObservableProperty]
    public partial int CurrentStep { get; set; }

    [ObservableProperty]
    public partial int StepCount { get; set; }

    [ObservableProperty]
    public partial bool IsModelSelectionStep { get; set; }

    [ObservableProperty]
    public partial bool IsPresetDetailStep { get; set; }

    [ObservableProperty]
    public partial bool IsLastStep { get; set; }

    [ObservableProperty]
    public partial bool IsPreviousStepShown { get; set; }

    [ObservableProperty]
    public partial bool IsNextButtonEnabled { get; set; }

    [ObservableProperty]
    public partial ChatServiceItemViewModel? SelectedService { get; set; }

    [ObservableProperty]
    public partial ChatModelItemViewModel? SelectedModel { get; set; }

    [ObservableProperty]
    public partial bool IsMessageEmpty { get; set; }

    [ObservableProperty]
    public partial bool IsModelsEmpty { get; set; }

    [ObservableProperty]
    public partial bool IsMaxRoundEnabled { get; set; }

    [ObservableProperty]
    public partial ChatOptions? CurrentOptions { get; set; }

    /// <summary>
    /// 关闭请求事件.
    /// </summary>
    public event EventHandler CloseRequested;

    /// <summary>
    /// 终止标记.
    /// </summary>
    public ObservableCollection<string> StopSequences { get; } = [];

    /// <summary>
    /// 过滤字符.
    /// </summary>
    public ObservableCollection<string> FilterCharacters { get; } = [];

    /// <summary>
    /// 可用服务.
    /// </summary>
    public ObservableCollection<ChatServiceItemViewModel> AvailableServices { get; } = [];

    /// <summary>
    /// 可用的模型.
    /// </summary>
    public ObservableCollection<ChatModelItemViewModel> Models { get; } = [];

    /// <summary>
    /// 预设消息.
    /// </summary>
    public ObservableCollection<ChatInteropMessage> Messages { get; } = [];

    /// <summary>
    /// 是否为手动退出.
    /// </summary>
    public bool IsManualClose { get; set; }
}
