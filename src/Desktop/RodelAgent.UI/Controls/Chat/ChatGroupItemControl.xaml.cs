// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.ViewModels.Items;
using RodelAgent.UI.ViewModels.View;

namespace RodelAgent.UI.Controls.Chat;

/// <summary>
/// Chat group item control.
/// </summary>
public sealed partial class ChatGroupItemControl : ChatGroupItemControlBase
{
    public ChatGroupItemControl() => InitializeComponent();

    private void OnGroupClick(object sender, RoutedEventArgs e)
        => this.Get<ChatPageViewModel>().SelectGroupCommand.Execute(ViewModel);
}

public abstract class ChatGroupItemControlBase : LayoutUserControlBase<ChatGroupItemViewModel>;