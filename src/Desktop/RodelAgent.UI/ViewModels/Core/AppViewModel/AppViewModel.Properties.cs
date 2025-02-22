// Copyright (c) Richasy. All rights reserved.

namespace RodelAgent.UI.ViewModels.Core;

/// <summary>
/// 应用视图模型.
/// </summary>
public sealed partial class AppViewModel
{
    [ObservableProperty]
    public partial Window ActivatedWindow { get; set; }

    [ObservableProperty]
    public partial bool IsUpdateShown { get; set; }

    [ObservableProperty]
    public partial bool IsClosing { get; set; }

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

    /// <summary>
    /// 请求重载对话服务.
    /// </summary>
    public event EventHandler RequestReloadChatServices;

    /// <summary>
    /// 请求重载绘图服务.
    /// </summary>
    public event EventHandler RequestReloadDrawServices;

    /// <summary>
    /// 请求重载语音服务.
    /// </summary>
    public event EventHandler RequestReloadAudioServices;

    /// <summary>
    /// 请求重载翻译服务.
    /// </summary>
    public event EventHandler RequestReloadTranslateServices;
}
