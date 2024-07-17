// Copyright (c) Rodel. All rights reserved.

using RodelAgent.Interfaces;
using RodelAgent.UI.ViewModels.Items;
using RodelChat.Interfaces.Client;
using RodelChat.Models.Constants;

namespace RodelAgent.UI.ViewModels.Components;

/// <summary>
/// 聊天预设模块视图模型.
/// </summary>
public sealed partial class ChatPresetModuleViewModel
{
    private readonly IStorageService _storageService;
    private readonly IChatParametersFactory _chatParametersFactory;

    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private string _instruction;

    [ObservableProperty]
    private int _currentStep;

    [ObservableProperty]
    private int _stepCount;

    [ObservableProperty]
    private bool _isModelSelectionStep;

    [ObservableProperty]
    private bool _isPresetDetailStep;

    [ObservableProperty]
    private bool _isLastStep;

    [ObservableProperty]
    private bool _isPreviousStepShown;

    [ObservableProperty]
    private bool _isNextButtonEnabled;

    [ObservableProperty]
    private bool _isAgentPreset;

    [ObservableProperty]
    private bool _isSessionPreset;

    [ObservableProperty]
    private ChatSessionPresetType _presetType;

    [ObservableProperty]
    private ChatServiceItemViewModel _selectedService;

    [ObservableProperty]
    private ChatModelItemViewModel _selectedModel;

    [ObservableProperty]
    private bool _isMessageEmpty;

    [ObservableProperty]
    private bool _isModelsEmpty;

    [ObservableProperty]
    private bool _isMaxRoundEnabled;

    /// <summary>
    /// 关闭请求事件.
    /// </summary>
    public event EventHandler CloseRequested;

    /// <summary>
    /// 终止标记.
    /// </summary>
    public ObservableCollection<string> StopSequences { get; } = new();

    /// <summary>
    /// 过滤字符.
    /// </summary>
    public ObservableCollection<string> FilterCharacters { get; } = new();

    /// <summary>
    /// 可用服务.
    /// </summary>
    public ObservableCollection<ChatServiceItemViewModel> AvailableServices { get; } = new();

    /// <summary>
    /// 可用的模型.
    /// </summary>
    public ObservableCollection<ChatModelItemViewModel> Models { get; } = new();

    /// <summary>
    /// 预设消息.
    /// </summary>
    public ObservableCollection<ChatMessageItemViewModel> Messages { get; } = new();

    /// <summary>
    /// 是否为手动退出.
    /// </summary>
    public bool IsManualClose { get; set; }
}
