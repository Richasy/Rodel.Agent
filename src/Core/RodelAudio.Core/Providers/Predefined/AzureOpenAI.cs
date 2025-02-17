// Copyright (c) Richasy. All rights reserved.

using RodelAudio.Models.Client;

/// <summary>
/// 预定义模型.
/// </summary>
internal static partial class PredefinedModels
{
    internal static List<AudioModel> AzureOpenAIModels { get; } = new List<AudioModel>
    {
        new AudioModel
        {
            Id = "tts",
            DisplayName = "TTS",
            Voices = GetOpenAIAudioVoices(),
        },
        new AudioModel
        {
            Id = "tts-hd",
            DisplayName = "TTS HD",
            Voices = GetOpenAIAudioVoices(),
        },
    };
}
