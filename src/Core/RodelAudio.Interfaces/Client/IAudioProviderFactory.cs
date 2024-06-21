// Copyright (c) Rodel. All rights reserved.

using RodelAudio.Models.Client;
using RodelAudio.Models.Constants;

namespace RodelAudio.Interfaces.Client;

/// <summary>
/// 创建音频服务商的工厂接口.
/// </summary>
public interface IAudioProviderFactory
{
    /// <summary>
    /// 获取或创建服务商.
    /// </summary>
    /// <param name="type">服务商类型.</param>
    /// <returns>服务商.</returns>
    IProvider GetOrCreateProvider(ProviderType type);

    /// <summary>
    /// 清除所有服务商.
    /// </summary>
    void Clear();

    /// <summary>
    /// 重置配置.
    /// </summary>
    /// <param name="configuration">配置内容.</param>
    void ResetConfiguration(AudioClientConfiguration configuration);
}
