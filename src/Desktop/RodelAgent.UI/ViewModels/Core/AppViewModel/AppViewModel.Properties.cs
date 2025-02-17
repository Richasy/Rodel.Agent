// Copyright (c) Richasy. All rights reserved.

namespace RodelAgent.UI.ViewModels.Core;

/// <summary>
/// 应用视图模型.
/// </summary>
public sealed partial class AppViewModel
{
    [ObservableProperty]
    public partial Window ActivatedWindow { get; set; }

    /// <summary>
    /// 是否支持托盘.
    /// </summary>
    public bool IsTraySupport { get; set; }

    /// <summary>
    /// 是否从任务栏关闭.
    /// </summary>
    public bool ExitFromTray { get; set; }

    /// <summary>
    /// 显示的窗口列表.
    /// </summary>
    public List<Window> Windows { get; } = [];
}
