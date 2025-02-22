// Copyright (c) Richasy. All rights reserved.

using Richasy.AgentKernel;
using Richasy.AgentKernel.Draw;
using Richasy.WinUIKernel.AI.ViewModels;
using RodelAgent.UI.ViewModels.Items;

namespace RodelAgent.UI.ViewModels.View;

/// <summary>
/// 绘图页面视图模型.
/// </summary>
public sealed partial class DrawPageViewModel
{
    private IDrawService _drawService;
    private CancellationTokenSource? _drawCts;

    [ObservableProperty]
    public partial List<DrawServiceItemViewModel> Services { get; set; }

    [ObservableProperty]
    public partial DrawServiceItemViewModel SelectedService { get; set; }

    [ObservableProperty]
    public partial DrawSizeItemViewModel? SelectedSize { get; set; }

    [ObservableProperty]
    public partial DrawModelItemViewModel? SelectedModel { get; set; }

    [ObservableProperty]
    public partial bool IsNoService { get; private set; }

    [ObservableProperty]
    public partial bool IsEnterSend { get; set; }

    [ObservableProperty]
    public partial int HistoryCount { get; set; }

    [ObservableProperty]
    public partial bool IsHistoryEmpty { get; set; }

    [ObservableProperty]
    public partial bool IsDrawing { get; set; }

    [ObservableProperty]
    public partial string Prompt { get; set; }

    [ObservableProperty]
    public partial Uri? Image { get; set; }

    [ObservableProperty]
    public partial DrawProviderType PresenterProvider { get; set; }

    [ObservableProperty]
    public partial string PresenterTime { get; set; }

    [ObservableProperty]
    public partial double PresenterProportion { get; set; }

    /// <summary>
    /// 尺寸列表.
    /// </summary>
    public ObservableCollection<DrawSizeItemViewModel> Sizes { get; } = [];

    /// <summary>
    /// 模型列表.
    /// </summary>
    public ObservableCollection<DrawModelItemViewModel> Models { get; } = [];

    /// <summary>
    /// 历史记录.
    /// </summary>
    public ObservableCollection<DrawRecordItemViewModel> History { get; } = [];
}
