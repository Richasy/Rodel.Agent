// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.ViewModels.Pages;

namespace RodelAgent.UI.Controls.Settings;

/// <summary>
/// 设置页面控件的基类.
/// </summary>
public abstract class SettingSectionBase : LayoutUserControlBase<SettingsPageViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SettingSectionBase"/> class.
    /// </summary>
    public SettingSectionBase()
    {
        IsTabStop = false;
        ViewModel = this.Get<SettingsPageViewModel>();
    }
}
