// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.ViewModels.View;

namespace RodelAgent.UI.Controls.Chat;

/// <summary>
/// Chat side body.
/// </summary>
public sealed partial class ChatSideBody : ChatPageControlBase
{
    public ChatSideBody() => InitializeComponent();

    private void OnAgentItemClick(object sender, EventArgs e)
    {
        var data = (sender as ChatAgentItemControl)?.ViewModel;
        this.Get<ChatPageViewModel>().SelectAgentCommand.Execute(data);
    }
}
