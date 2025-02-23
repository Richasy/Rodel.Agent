// Copyright (c) Richasy. All rights reserved.

using Richasy.WinUIKernel.AI.ViewModels;
using RodelAgent.UI.ViewModels.View;

namespace RodelAgent.UI.Controls.Chat;

/// <summary>
/// Chat service item control.
/// </summary>
public sealed partial class ChatServiceItemControl : ChatServiceItemControlBase
{
    public ChatServiceItemControl() => InitializeComponent();

    private void OnServiceClick(object sender, RoutedEventArgs e)
        => this.Get<ChatPageViewModel>().SelectServiceCommand.Execute(ViewModel);
}

public abstract class ChatServiceItemControlBase : LayoutUserControlBase<ChatServiceItemViewModel>;