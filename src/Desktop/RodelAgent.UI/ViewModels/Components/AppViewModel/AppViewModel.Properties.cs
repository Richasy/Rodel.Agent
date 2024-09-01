// Copyright (c) Rodel. All rights reserved.

namespace RodelAgent.UI.ViewModels;

/// <summary>
/// 应用视图模型.
/// </summary>
public sealed partial class AppViewModel
{
    private readonly ILogger<AppViewModel> _logger;

    [ObservableProperty]
    private Window _activatedWindow;

    /// <summary>
    /// 预设头像更新请求.
    /// </summary>
    public event EventHandler<string> PresetAvatarUpdateRequested;

    /// <summary>
    /// 显示的窗口列表.
    /// </summary>
    public List<Window> DisplayWindows { get; } = new();

    /// <summary>
    /// 当前显示的对话框.
    /// </summary>
    public ContentDialog CurrentDialog { get; set; }
}
