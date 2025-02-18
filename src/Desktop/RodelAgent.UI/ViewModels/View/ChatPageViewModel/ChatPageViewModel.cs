// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.Pages;

namespace RodelAgent.UI.ViewModels.View;

/// <summary>
/// 对话页面视图模型.
/// </summary>
public sealed partial class ChatPageViewModel : LayoutPageViewModelBase
{
    /// <inheritdoc/>
    protected override string GetPageKey() => nameof(ChatPage);
}
