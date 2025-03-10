// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.ViewModels.Core;

namespace RodelAgent.UI.Controls.Chat;

public abstract class ChatAgentConfigControlBase : LayoutUserControlBase<ChatAgentConfigViewModel>
{
    protected ChatAgentConfigControlBase() => ViewModel = this.Get<ChatAgentConfigViewModel>();
}
