// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.Pages;

namespace RodelAgent.UI.ViewModels.View;

/// <summary>
/// 语音页面视图模型.
/// </summary>
public sealed partial class SpeechPageViewModel : LayoutPageViewModelBase
{
    /// <inheritdoc/>
    protected override string GetPageKey() => nameof(SpeechPage);
}
