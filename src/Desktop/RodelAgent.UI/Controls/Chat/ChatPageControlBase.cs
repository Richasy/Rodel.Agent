// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.ViewModels.View;

namespace RodelAgent.UI.Controls.Chat;

public abstract class ChatPageControlBase : LayoutUserControlBase<ChatPageViewModel>
{
    protected ChatPageControlBase() => ViewModel = this.Get<ChatPageViewModel>();
}
