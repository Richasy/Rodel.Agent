// Copyright (c) Richasy. All rights reserved.

using Richasy.WinUIKernel.AI.ViewModels;
using RodelAgent.UI.ViewModels.Core;

namespace RodelAgent.UI.Controls.Chat;

/// <summary>
/// Chat model item control.
/// </summary>
public sealed partial class ChatModelItemControl : ChatModelItemControlBase
{
    public ChatModelItemControl() => InitializeComponent();

    private void OnModelClick(object sender, RoutedEventArgs e)
        => this.Get<ChatSessionViewModel>().SelectModelCommand.Execute(ViewModel);
}

public abstract class ChatModelItemControlBase : LayoutUserControlBase<ChatModelItemViewModel>;
