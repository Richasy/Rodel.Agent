// Copyright (c) Richasy. All rights reserved.

using RodelAgent.Interfaces;
using RodelAgent.Models.Feature;
using RodelAgent.UI.ViewModels.Items;

namespace RodelAgent.UI.ViewModels.Core;

/// <summary>
/// 聊天群组配置视图模型.
/// </summary>
public sealed partial class ChatGroupConfigViewModel
{
    private readonly IStorageService _storageService;
    private Func<string>? _getEmoji;

    public ChatGroup? Group { get; private set; }

    [ObservableProperty]
    public partial string? Name { get; set; }

    [ObservableProperty]
    public partial int MaxRounds { get; set; }

    [ObservableProperty]
    public partial bool IsNoAgentSelected { get; set; }

    [ObservableProperty]
    public partial bool IsAgentsEmpty { get; set; }

    /// <summary>
    /// 关闭请求事件.
    /// </summary>
    public event EventHandler CloseRequested;

    public ObservableCollection<ChatAgentItemViewModel> TotalAgents { get; } = [];

    public ObservableCollection<ChatAgentItemViewModel> SelectedAgents { get; } = [];

    public ObservableCollection<string> TerminateSequence { get; } = [];

    public bool IsManualClose { get; set; }
}
