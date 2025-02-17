// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.ViewModels.Components;

namespace RodelAgent.UI.Controls.Items;

/// <summary>
/// 群组会话项控件.
/// </summary>
public sealed class ChatGroupItemControl : LayoutControlBase<ChatGroupViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChatGroupItemControl"/> class.
    /// </summary>
    public ChatGroupItemControl() => DefaultStyleKey = typeof(ChatGroupItemControl);

    /// <summary>
    /// 条目被点击.
    /// </summary>
    public event EventHandler<ChatGroupViewModel> Click;

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
