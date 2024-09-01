// Copyright (c) Rodel. All rights reserved.

using RodelAgent.UI.ViewModels.Pages;

namespace RodelAgent.UI.Controls.Audio;

/// <summary>
/// 音频服务页面控件基类.
/// </summary>
public abstract class AudioServicePageControlBase : LayoutUserControlBase<AudioServicePageViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AudioServicePageControlBase"/> class.
    /// </summary>
    protected AudioServicePageControlBase()
        => ViewModel = this.Get<AudioServicePageViewModel>();
}
