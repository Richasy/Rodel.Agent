// Copyright (c) Rodel. All rights reserved.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using RodelChat.Models.Chat;
using RodelChat.Models.Constants;

namespace RodelChat.Interfaces.Client;

/// <summary>
/// 聊天客户端的接口定义.
/// </summary>
public interface IChatClient : IDisposable
{
    /// <summary>
    /// 加载会话列表.
    /// </summary>
    /// <param name="sessions">会话列表.</param>
    void LoadSessions(List<ChatSession> sessions);

    /// <summary>
    /// 设置服务商的自定义模型列表.
    /// </summary>
    /// <param name="type">提供商类型.</param>
    /// <param name="models">模型列表.</param>
    void SetCustomModels(ProviderType type, List<ChatModel> models);

    /// <summary>
    /// 创建新会话.
    /// </summary>
    /// <returns><see cref="ChatSession"/>.</returns>
    ChatSession CreateSession(ProviderType providerType, ChatParameters parameters = null, string? modelId = null);

    /// <summary>
    /// 获取模型列表.
    /// </summary>
    /// <param name="type">供应商类型.</param>
    /// <returns>模型列表.</returns>
    List<ChatModel> GetModels(ProviderType type);

    /// <summary>
    /// 发送消息.
    /// </summary>
    /// <param name="sessionId">会话标识符.</param>
    /// <param name="message">用户输入.</param>
    /// <param name="modelId">指定的模型 ID.</param>
    /// <param name="streamingAction">流式输出的处理.</param>
    /// <param name="cancellationToken">终止令牌.</param>
    /// <returns><see cref="ChatMessage"/>.</returns>
    Task<ChatMessage> SendMessageAsync(
        string sessionId,
        ChatMessage? message = null,
        string? modelId = null,
        Action<string> streamingAction = default,
        List<KernelPlugin> plugins = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 从 DLL 中检索插件.
    /// </summary>
    /// <param name="dllPath">DLL 路径.</param>
    /// <returns>插件实例列表.</returns>
    Dictionary<string, object> RetrievePluginsFromDll(string dllPath);

    /// <summary>
    /// 注入插件到内核.
    /// </summary>
    /// <param name="plugins">插件列表.</param>
    void InjectPluginsToKernel(Dictionary<string, object> plugins);

    /// <summary>
    /// 获取内核插件列表.
    /// </summary>
    /// <returns>插件列表.</returns>
    List<KernelPlugin> GetKernelPlugins();
}
