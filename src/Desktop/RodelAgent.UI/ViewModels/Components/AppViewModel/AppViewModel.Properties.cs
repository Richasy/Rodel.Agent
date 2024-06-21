// Copyright (c) Rodel. All rights reserved.

using RodelAgent.UI.Models.Args;
using RodelAgent.UI.ViewModels.Items;

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
    /// 在有新的提示请求时触发.
    /// </summary>
    public event EventHandler<AppTipNotification> RequestShowTip;

    /// <summary>
    /// 导航请求.
    /// </summary>
    public event EventHandler<AppNavigationEventArgs> NavigationRequested;

    /// <summary>
    /// 预设头像更新请求.
    /// </summary>
    public event EventHandler<string> PresetAvatarUpdateRequested;

    /// <summary>
    /// 显示的窗口列表.
    /// </summary>
    public List<Window> DisplayWindows { get; } = new();

    /// <summary>
    /// 导航条目.
    /// </summary>
    public ObservableCollection<NavigateItemViewModel> NavigateItems { get; } = new();

    /// <summary>
    /// 设置项.
    /// </summary>
    public NavigateItemViewModel SettingsItem { get; }

    /// <summary>
    /// 当前显示的对话框.
    /// </summary>
    public ContentDialog CurrentDialog { get; set; }
}
