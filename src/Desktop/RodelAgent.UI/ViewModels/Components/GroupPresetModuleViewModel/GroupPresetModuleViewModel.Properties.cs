// Copyright (c) Rodel. All rights reserved.

using RodelAgent.Interfaces;
using RodelAgent.UI.ViewModels.Items;

namespace RodelAgent.UI.ViewModels.Components;

/// <summary>
/// 群组预设模块视图模型.
/// </summary>
public sealed partial class GroupPresetModuleViewModel
{
    private readonly IStorageService _storageService;

    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private int _maxRounds;

    [ObservableProperty]
    private bool _isNoAgentSelected;

    [ObservableProperty]
    private bool _isAgentsEmpty;

    /// <summary>
    /// 关闭请求事件.
    /// </summary>
    public event EventHandler CloseRequested;

    /// <summary>
    /// 全部助理.
    /// </summary>
    public ObservableCollection<ChatPresetItemViewModel> TotalAgents { get; } = new();

    /// <summary>
    /// 已选助理.
    /// </summary>
    public ObservableCollection<ChatPresetItemViewModel> SelectedAgents { get; } = new();

    /// <summary>
    /// 终止文本.
    /// </summary>
    public ObservableCollection<string> TerminateText { get; } = new();

    /// <summary>
    /// 是否为手动退出.
    /// </summary>
    public bool IsManualClose { get; set; }
}
