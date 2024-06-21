// Copyright (c) Rodel. All rights reserved.

using RodelAgent.Models.Abstractions;
using RodelAudio.Interfaces.Client;
using RodelAudio.Models.Constants;

namespace RodelAudio.Console;

/// <summary>
/// 音频参数工厂.
/// </summary>
public sealed class AudioParametersFactory : IAudioParametersFactory
{
    /// <inheritdoc/>
    public BaseFieldParameters CreateAudioParameters(ProviderType provider)
    {
        return provider switch
        {
            _ => default,
        };
    }
}
