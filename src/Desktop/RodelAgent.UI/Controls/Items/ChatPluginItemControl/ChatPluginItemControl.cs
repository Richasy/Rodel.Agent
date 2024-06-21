// Copyright (c) Rodel. All rights reserved.

using RodelAgent.UI.ViewModels.Items;

namespace RodelAgent.UI.Controls.Items;

/// <summary>
/// 聊天插件项控件.
/// </summary>
public sealed class ChatPluginItemControl : ReactiveControl<ChatPluginItemViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChatPluginItemControl"/> class.
    /// </summary>
    public ChatPluginItemControl() => DefaultStyleKey = typeof(ChatPluginItemControl);

    /// <summary>
    /// 点击事件.
    /// </summary>
    public event EventHandler Click;

    /// <inheritdoc/>
    protected override void OnApplyTemplate()
    {
        var card = GetTemplateChild("RootCard") as CardPanel;
        if (card != null)
        {
            card.Click += (sender, e) => Click?.Invoke(this, EventArgs.Empty);
        }
    }
}
