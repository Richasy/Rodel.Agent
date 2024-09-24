// Copyright (c) Rodel. All rights reserved.

using RodelAudio.Models.Client;

namespace RodelAgent.UI.ViewModels.Items;

/// <summary>
/// 音频会话视图模型.
/// </summary>
public sealed partial class AudioSessionItemViewModel : ViewModelBase<AudioSession>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AudioSessionItemViewModel"/> class.
    /// </summary>
    public AudioSessionItemViewModel(AudioSession data)
        : base(data)
    {
    }
}
