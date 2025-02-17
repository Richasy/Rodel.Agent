// Copyright (c) Richasy. All rights reserved.

using RodelAudio.Models.Client;
using RodelAudio.Models.Constants;

namespace RodelAudio.Interfaces.Client;

/// <summary>
/// 音频客户端.
/// </summary>
public interface IAudioClient : IDisposable
{
    /// <summary>
    /// 获取预定义模型.
    /// </summary>
    /// <param name="type">供应商类型.</param>
    /// <returns>预定义模型列表.</returns>
    List<AudioModel> GetPredefinedModels(ProviderType type);

    /// <summary>
    /// 获取模型列表.
    /// </summary>
    /// <param name="type">供应商类型.</param>
    /// <returns>模型列表.</returns>
    List<AudioModel> GetModels(ProviderType type);

    /// <summary>
    /// 检查并初始化 Azure 语音服务.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    Task InitialAzureSpeechAsync();

    /// <summary>
    /// 文本转语音.
    /// </summary>
    /// <param name="session">会话信息.</param>
    /// <param name="cancellationToken">终止令牌.</param>
    /// <returns>字节.</returns>
    Task<ReadOnlyMemory<byte>> TextToSpeechAsync(
        AudioSession session,
        CancellationToken cancellationToken = default);
}
