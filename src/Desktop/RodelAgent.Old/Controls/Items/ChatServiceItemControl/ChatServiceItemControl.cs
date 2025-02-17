// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.ViewModels.Items;

namespace RodelAgent.UI.Controls.Items;

/// <summary>
/// 聊天服务项控件.
/// </summary>
public sealed class ChatServiceItemControl : LayoutControlBase<ChatServiceItemViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChatServiceItemControl"/> class.
    /// </summary>
    public ChatServiceItemControl() => DefaultStyleKey = typeof(ChatServiceItemControl);

    /// <summary>
    /// 点击事件.
    /// </summary>
    public event EventHandler<ChatServiceItemViewModel> Click;

    /// <inheritdoc/>
    protected override void OnApplyTemplate()
    {
        var rootCard = GetTemplateChild("RootCard") as CardPanel;
        if (rootCard != null)
        {
            rootCard.Click += OnClick;
        }
    }

    private void OnClick(object sender, RoutedEventArgs e)
        => Click?.Invoke(this, ViewModel);
}
