// Copyright (c) Rodel. All rights reserved.

using RodelAgent.UI.ViewModels.Items;

namespace RodelAgent.UI.Controls.Items;

/// <summary>
/// 聊天模型卡片.
/// </summary>
public sealed class ChatModelCard : LayoutControlBase<ChatModelItemViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChatModelCard"/> class.
    /// </summary>
    public ChatModelCard()
    {
        DefaultStyleKey = typeof(ChatModelCard);
    }

    /// <summary>
    /// 点击事件.
    /// </summary>
    public event EventHandler<ChatModelItemViewModel> Click;

    /// <inheritdoc/>
    protected override void OnApplyTemplate()
    {
        var card = GetTemplateChild("RootCard") as CardPanel;
        if (card != null)
        {
            card.Click += (sender, e) => Click?.Invoke(this, ViewModel);
        }
    }
}
