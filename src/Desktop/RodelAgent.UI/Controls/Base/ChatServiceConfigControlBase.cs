// Copyright (c) Rodel. All rights reserved.

using RodelAgent.UI.ViewModels.Items;

namespace RodelAgent.UI.Controls;

/// <summary>
/// 聊天服务配置控件基类.
/// </summary>
public abstract class ChatServiceConfigControlBase : LayoutUserControlBase<ChatServiceItemViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChatServiceConfigControlBase"/> class.
    /// </summary>
    protected ChatServiceConfigControlBase() => IsTabStop = false;
}
