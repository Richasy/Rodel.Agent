// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.ViewModels.Items;
using RodelAgent.UI.ViewModels.View;

namespace RodelAgent.UI.Controls.Chat;

public sealed partial class ChatAgentItemControl : ChatAgentItemControlBase
{
    public ChatAgentItemControl() => InitializeComponent();

    private void OnAgentClick(object sender, RoutedEventArgs e)
        => this.Get<ChatPageViewModel>().SelectAgentCommand.Execute(ViewModel);
}

public abstract class ChatAgentItemControlBase : LayoutUserControlBase<ChatAgentItemViewModel>;