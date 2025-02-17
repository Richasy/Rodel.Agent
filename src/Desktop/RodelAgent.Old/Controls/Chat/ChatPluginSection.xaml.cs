// Copyright (c) Richasy. All rights reserved.

namespace RodelAgent.UI.Controls.Chat;

/// <summary>
/// 聊天插件部分.
/// </summary>
public sealed partial class ChatPluginSection : ChatServicePageControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChatPluginSection"/> class.
    /// </summary>
    public ChatPluginSection() => InitializeComponent();

    private void OnPluginItemClick(object sender, EventArgs e)
        => FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
}
