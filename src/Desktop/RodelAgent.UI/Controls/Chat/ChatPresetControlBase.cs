﻿// Copyright (c) Rodel. All rights reserved.

using RodelAgent.UI.ViewModels.Components;

namespace RodelAgent.UI.Controls.Chat;

/// <summary>
/// 聊天预设控件基类.
/// </summary>
public abstract class ChatPresetControlBase : ReactiveUserControl<ChatPresetModuleViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChatPresetControlBase"/> class.
    /// </summary>
    protected ChatPresetControlBase() => ViewModel = ServiceProvider.GetRequiredService<ChatPresetModuleViewModel>();
}
