// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.ViewModels.View;

namespace RodelAgent.UI.Controls.Draw;

/// <summary>
/// 绘图页面控件基类.
/// </summary>
public abstract class DrawPageControlBase : LayoutUserControlBase<DrawPageViewModel>
{
    /// <summary>
    /// 初始化.
    /// </summary>
    protected DrawPageControlBase() => ViewModel = this.Get<DrawPageViewModel>();
}
