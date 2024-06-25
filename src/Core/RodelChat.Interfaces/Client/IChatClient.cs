// Copyright (c) Rodel. All rights reserved.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using RodelAgent.Models.Abstractions;
using RodelChat.Models.Client;
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
    void LoadChatSessions(List<ChatSession> sessions);

    /// <summary>
    /// 获取预定义模型.
    /// </summary>
    /// <param name="type">供应商类型.</param>
    /// <returns>预定义模型列表.</returns>
    List<ChatModel> GetPredefinedModels(ProviderType type);

    /// <summary>
    /// 创建新会话.
    /// </summary>
    /// <returns><see cref="ChatSession"/>.</returns>
    ChatSession CreateSession(ProviderType providerType, BaseFieldParameters parameters = null, string? modelId = null);

    /// <summary>
    /// 创建新会话.
    /// </summary>
    /// <param name="preset">会话预设.</param>
    /// <returns>会话信息.</returns>
    ChatSession CreateSession(ChatSessionPreset preset);

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
    /// <param name="plugins">插件.</param>
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
    /// 发送群组消息.
    /// </summary>
    /// <param name="message">群组消息.</param>
    /// <param name="preset">群组预设.</param>
    /// <param name="messageAction">消息生成事件.</param>
    /// <param name="cancellationToken">终止令牌.</param>
    /// <param name="agents">助理列表.</param>
    /// <returns><see cref="Task"/>.</returns>
    Task SendGroupMessageAsync(
        ChatMessage message,
        ChatGroupPreset preset,
        Action<ChatMessage> messageAction = default,
        CancellationToken cancellationToken = default,
        params ChatSessionPreset[] agents);

    /// <summary>
    /// 从 DLL 中检索插件.
    /// </summary>
    /// <param name="dllPath">DLL 路径.</param>
    /// <returns>插件实例列表.</returns>
    Task<List<KernelPlugin>> RetrievePluginsFromDllAsync(string dllPath);

    /// <summary>
    /// 获取供应商的内核聊天补全服务.
    /// </summary>
    /// <param name="type">类型.</param>
    /// <returns>聊天服务.</returns>
    IChatCompletionService? GetKernelChatCompletionService(ProviderType type);

    /// <summary>
    /// 通过内核进行一次性函数调用.
    /// </summary>
    /// <param name="type">供应商类型.</param>
    /// <param name="model">模型 ID.</param>
    /// <param name="function">函数定义.</param>
    /// <param name="arguments">函数参数.</param>
    /// <returns>结果.</returns>
    Task<string?> InvokeFunctionAsync(ProviderType type, string model, KernelFunction function, KernelArguments arguments);
}
