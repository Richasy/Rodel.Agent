// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.ViewModels.View;

namespace RodelAgent.UI.Pages;

/// <summary>
/// Chat page.
/// </summary>
public sealed partial class ChatPage : ChatPageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChatPage"/> class.
    /// </summary>
    public ChatPage() => InitializeComponent();
}

/// <summary>
/// Chat page base.
/// </summary>
public abstract class ChatPageBase : LayoutPageBase<ChatPageViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChatPageBase"/> class.
    /// </summary>
    protected ChatPageBase() => ViewModel = this.Get<ChatPageViewModel>();
}