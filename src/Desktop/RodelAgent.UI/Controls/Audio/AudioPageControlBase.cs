// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.ViewModels.View;

namespace RodelAgent.UI.Controls.Audio;

/// <summary>
/// 音频页面控件基类.
/// </summary>
public abstract class AudioPageControlBase : LayoutUserControlBase<AudioPageViewModel>
{
    protected AudioPageControlBase() => ViewModel = this.Get<AudioPageViewModel>();
}
