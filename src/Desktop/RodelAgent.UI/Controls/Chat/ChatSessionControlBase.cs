// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.ViewModels.Core;

namespace RodelAgent.UI.Controls.Chat;

public abstract class ChatSessionControlBase : LayoutUserControlBase<ChatSessionViewModel>
{
    protected ChatSessionControlBase() => ViewModel = this.Get<ChatSessionViewModel>();
}
