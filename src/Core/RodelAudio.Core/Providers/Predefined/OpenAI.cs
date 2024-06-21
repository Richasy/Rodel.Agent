// Copyright (c) Rodel. All rights reserved.

using RodelAudio.Models.Client;
using RodelAudio.Models.Constants;

/// <summary>
/// 预定义模型.
/// </summary>
internal static partial class PredefinedModels
{
    internal static List<AudioModel> OpenAIModels { get; } = new List<AudioModel>
    {
        new AudioModel
        {
            Id = "tts-1",
            DisplayName = "TTS",
            Voices = GetOpenAIAudioVoices(),
        },
        new AudioModel
        {
            Id = "tts-1-hd",
            DisplayName = "TTS HD",
            Voices = GetOpenAIAudioVoices(),
        },
    };

    internal static string[] GetOpenAIAudioLanguages()
    {
        return
        [
            "af", "ar", "hy", "az", "be", "bs", "bg", "ca", "zh",
            "hr", "cs", "da", "nl", "en", "et", "fi", "fr", "gl",
            "de", "el", "he", "hi", "hu", "is", "id", "it"
        ];
    }

    internal static List<AudioVoice> GetOpenAIAudioVoices()
    {
        return new List<AudioVoice>
        {
            new AudioVoice("alloy", "Alloy", VoiceGender.Male, GetOpenAIAudioLanguages()),
            new AudioVoice("echo", "Echo", VoiceGender.Male, GetOpenAIAudioLanguages()),
            new AudioVoice("fable", "Fable", VoiceGender.Male, GetOpenAIAudioLanguages()),
            new AudioVoice("onyx", "Onyx", VoiceGender.Male, GetOpenAIAudioLanguages()),
            new AudioVoice("nova", "Nova", VoiceGender.Female, GetOpenAIAudioLanguages()),
            new AudioVoice("shimmer", "Shimmer", VoiceGender.Female, GetOpenAIAudioLanguages()),
        };
    }
}
