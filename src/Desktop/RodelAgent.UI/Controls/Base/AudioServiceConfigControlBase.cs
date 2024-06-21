// Copyright (c) Rodel. All rights reserved.

using RodelAgent.UI.ViewModels.Items;

namespace RodelAgent.UI.Controls;

/// <summary>
/// 音频服务配置控件基类.
/// </summary>
public abstract class AudioServiceConfigControlBase : ReactiveUserControl<AudioServiceItemViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AudioServiceConfigControlBase"/> class.
    /// </summary>
    protected AudioServiceConfigControlBase() => IsTabStop = false;
}
