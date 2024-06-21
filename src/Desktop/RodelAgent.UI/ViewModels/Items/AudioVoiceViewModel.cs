// Copyright (c) Rodel. All rights reserved.

using RodelAudio.Models.Client;

namespace RodelAgent.UI.ViewModels.Items;

/// <summary>
/// 音频声音视图模型.
/// </summary>
public sealed partial class AudioVoiceViewModel : ViewModelBase<AudioVoice>
{
    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private bool _isMale;

    [ObservableProperty]
    private bool _isFemale;

    [ObservableProperty]
    private bool _isNeutral;

    [ObservableProperty]
    private bool _isSelected;

    /// <summary>
    /// Initializes a new instance of the <see cref="AudioVoiceViewModel"/> class.
    /// </summary>
    public AudioVoiceViewModel(AudioVoice voice)
        : base(voice)
    {
        Name = voice.DisplayName;
        IsMale = voice.Gender == RodelAudio.Models.Constants.VoiceGender.Male;
        IsFemale = voice.Gender == RodelAudio.Models.Constants.VoiceGender.Female;
        IsNeutral = voice.Gender == RodelAudio.Models.Constants.VoiceGender.Neutral;
    }
}
