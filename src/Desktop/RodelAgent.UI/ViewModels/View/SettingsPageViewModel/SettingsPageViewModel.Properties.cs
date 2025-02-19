// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.ViewModels.Items;

namespace RodelAgent.UI.ViewModels.View;

/// <summary>
/// 设置页面视图模型.
/// </summary>
public sealed partial class SettingsPageViewModel
{
    private bool _isInitialized;

    [ObservableProperty]
    public partial ElementTheme AppTheme { get; set; }

    [ObservableProperty]
    public partial string AppThemeText { get; set; }

    [ObservableProperty]
    public partial string PackageVersion { get; set; }

    [ObservableProperty]
    public partial string Copyright { get; set; }

    [ObservableProperty]
    public partial string WorkingDirectory { get; set; }

    [ObservableProperty]
    public partial bool HideWhenWindowClosing { get; set; }

    /// <summary>
    /// 链接项.
    /// </summary>
    public ObservableCollection<LinkItemViewModel> Links { get; } = [];

    /// <summary>
    /// 库项.
    /// </summary>
    public ObservableCollection<LinkItemViewModel> Libraries { get; } = [];
}
