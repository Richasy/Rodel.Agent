// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.ViewModels.Components;

namespace RodelAgent.UI.Controls.Chat;

/// <summary>
/// 群组预设控件基类.
/// </summary>
public abstract class GroupPresetControlBase : LayoutUserControlBase<GroupPresetModuleViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GroupPresetControlBase"/> class.
    /// </summary>
    protected GroupPresetControlBase() => ViewModel = this.Get<GroupPresetModuleViewModel>();
}
