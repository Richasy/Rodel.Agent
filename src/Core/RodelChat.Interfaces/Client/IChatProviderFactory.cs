// Copyright (c) Rodel. All rights reserved.

using Richasy.AgentKernel;

namespace RodelChat.Interfaces.Client;

/// <summary>
/// 创建聊天服务商的工厂接口.
/// </summary>
public interface IChatProviderFactory
{
    /// <summary>
    /// 获取或创建服务商.
    /// </summary>
    /// <param name="type">服务商类型.</param>
    /// <returns>服务商.</returns>
    IProvider GetOrCreateProvider(ChatProviderType type);

    /// <summary>
    /// 清除所有服务商.
    /// </summary>
    void Clear();

    /// <summary>
    /// 重置配置.
    /// </summary>
    /// <param name="configuration">配置内容.</param>
    void ResetConfiguration(ChatClientConfiguration configuration);
}
