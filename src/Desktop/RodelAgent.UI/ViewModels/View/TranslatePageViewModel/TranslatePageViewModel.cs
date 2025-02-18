// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.Pages;

namespace RodelAgent.UI.ViewModels.View;

/// <summary>
/// 翻译页面视图模型.
/// </summary>
public sealed partial class TranslatePageViewModel : LayoutPageViewModelBase
{
    /// <inheritdoc/>
    protected override string GetPageKey() => nameof(TranslatePage);
}
