// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.Pages;

namespace RodelAgent.UI.ViewModels.View;

/// <summary>
/// 绘图页面视图模型.
/// </summary>
public sealed partial class DrawPageViewModel : LayoutPageViewModelBase
{
    /// <inheritdoc/>
    protected override string GetPageKey() => nameof(DrawPage);
}
