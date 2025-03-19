// Copyright (c) Richasy. All rights reserved.

using Richasy.WinUIKernel.AI.ViewModels;
using RodelAgent.UI.ViewModels.Core;
using RodelAgent.UI.ViewModels.Items;

namespace RodelAgent.UI.ViewModels.View;

public sealed partial class ChatPageViewModel
{
    private bool _isInitialized;

    private readonly ChatSessionViewModel _sessionViewModel;

    [ObservableProperty]
    public partial List<ChatServiceItemViewModel>? Services { get; set; }

    [ObservableProperty]
    public partial ChatServiceItemViewModel? SelectedService { get; set; }

    [ObservableProperty]
    public partial ChatAgentItemViewModel? SelectedAgent { get; set; }

    [ObservableProperty]
    public partial ChatGroupItemViewModel? SelectedGroup { get; set; }

    [ObservableProperty]
    public partial bool IsAgentSectionVisible { get; set; }

    [ObservableProperty]
    public partial bool IsToolSectionVisible { get; set; }

    [ObservableProperty]
    public partial bool IsNoService { get; private set; }

    [ObservableProperty]
    public partial bool IsInitializing { get; set; }

    [ObservableProperty]
    public partial bool IsAgentListVisible { get; set; }

    [ObservableProperty]
    public partial bool IsGroupListVisible { get; set; }

    [ObservableProperty]
    public partial bool IsServicesCollapsed { get; set; }

    [ObservableProperty]
    public partial bool IsAgentsCollapsed { get; set; }

    [ObservableProperty]
    public partial bool IsGroupsCollapsed { get; set; }

    /// <summary>
    /// 助理列表.
    /// </summary>
    public ObservableCollection<ChatAgentItemViewModel> Agents { get; } = [];

    /// <summary>
    /// 群组列表.
    /// </summary>
    public ObservableCollection<ChatGroupItemViewModel> Groups { get; } = [];

    /// <summary>
    /// MCP列表.
    /// </summary>
    public ObservableCollection<McpServerItemViewModel> Servers { get; } = [];
}
