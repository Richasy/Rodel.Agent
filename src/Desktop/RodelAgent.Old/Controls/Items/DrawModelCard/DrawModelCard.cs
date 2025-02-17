// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.ViewModels.Items;

namespace RodelAgent.UI.Controls.Items;

/// <summary>
/// 聊天模型卡片.
/// </summary>
public sealed class DrawModelCard : LayoutControlBase<DrawModelItemViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DrawModelCard"/> class.
    /// </summary>
    public DrawModelCard() => DefaultStyleKey = typeof(DrawModelCard);

    /// <summary>
    /// 点击事件.
    /// </summary>
    public event EventHandler<DrawModelItemViewModel> Click;

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
