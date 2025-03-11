// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.ViewModels.Items;

namespace RodelAgent.UI.Controls.Chat;

public sealed partial class ChatAgentItemControl : ChatAgentItemControlBase
{
    public ChatAgentItemControl() => InitializeComponent();

    public event EventHandler Click;

    private void OnAgentClick(object sender, RoutedEventArgs e)
        => Click?.Invoke(this, EventArgs.Empty);
}

public abstract class ChatAgentItemControlBase : LayoutUserControlBase<ChatAgentItemViewModel>;