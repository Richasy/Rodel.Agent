// Copyright (c) Richasy. All rights reserved.

using Richasy.AgentKernel.Models;

namespace RodelAgent.UI.ViewModels.Items;

/// <summary>
/// 语音声音视图模型.
/// </summary>
public sealed partial class AudioVoiceItemViewModel : ViewModelBase<AudioVoice>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AudioVoiceItemViewModel"/> class.
    /// </summary>
    public AudioVoiceItemViewModel(AudioVoice data)
        : base(data)
    {
        Gender = Data.Gender switch
        {
            VoiceGender.Male => "♂️",
            VoiceGender.Female => "♀️",
            _ => "⚧️",
        };
    }

    /// <summary>
    /// 性别标识.
    /// </summary>
    [ObservableProperty]
    public partial string Gender { get; set; }
}
