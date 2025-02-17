// Copyright (c) Richasy. All rights reserved.

using RodelAudio.Models.Constants;

namespace RodelAudio.Interfaces.Client;

/// <summary>
/// 音频参数工厂.
/// </summary>
public interface IAudioParametersFactory
{
    /// <summary>
    /// 创建音频参数.
    /// </summary>
    /// <param name="provider">音频供应商.</param>
    /// <returns>参数.</returns>
    BaseFieldParameters CreateAudioParameters(ProviderType provider);
}
