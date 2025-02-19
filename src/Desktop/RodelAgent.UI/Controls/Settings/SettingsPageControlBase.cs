// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.ViewModels.View;

namespace RodelAgent.UI.Controls.Settings;

/// <summary>
/// 设置页面控件基类.
/// </summary>
public abstract class SettingsPageControlBase : LayoutUserControlBase<SettingsPageViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsPageControlBase"/> class.
    /// </summary>
    protected SettingsPageControlBase() => ViewModel = this.Get<SettingsPageViewModel>();
}
