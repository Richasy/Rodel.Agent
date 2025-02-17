// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.ViewModels.Components;

namespace RodelAgent.UI.Controls.Items;

/// <summary>
/// 聊天会话项控件.
/// </summary>
public sealed class ChatSessionItemControl : LayoutControlBase<ChatSessionViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChatSessionItemControl"/> class.
    /// </summary>
    public ChatSessionItemControl() => DefaultStyleKey = typeof(ChatSessionItemControl);

    /// <summary>
    /// 条目被点击.
    /// </summary>
    public event EventHandler<ChatSessionViewModel> Click;

    /// <inheritdoc/>
    protected override void OnApplyTemplate()
    {
        var rootCard = GetTemplateChild("RootCard") as CardPanel;
        if (rootCard != null)
        {
            rootCard.Click += (sender, e) => Click?.Invoke(this, ViewModel);
        }
    }
}
