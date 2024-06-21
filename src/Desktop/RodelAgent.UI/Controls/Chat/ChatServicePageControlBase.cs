// Copyright (c) Rodel. All rights reserved.

using RodelAgent.UI.ViewModels.Pages;

namespace RodelAgent.UI.Controls.Chat;

/// <summary>
/// 聊天页面控件基类.
/// </summary>
public abstract class ChatServicePageControlBase : ReactiveUserControl<ChatServicePageViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChatServicePageControlBase"/> class.
    /// </summary>
    protected ChatServicePageControlBase() => ViewModel = ServiceProvider.GetRequiredService<ChatServicePageViewModel>();
}
