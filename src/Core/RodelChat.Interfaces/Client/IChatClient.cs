// Copyright (c) Rodel. All rights reserved.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Richasy.AgentKernel;
using RodelAgent.Models.Abstractions;
using RodelChat.Models.Client;

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
    /// 加载群组会话列表.
    /// </summary>
    /// <param name="groups">群组会话列表.</param>
    void LoadGroupSessions(List<ChatGroup> groups);

    /// <summary>
    /// 获取预定义模型.
    /// </summary>
    /// <param name="type">供应商类型.</param>
    /// <returns>预定义模型列表.</returns>
    List<ChatModel> GetPredefinedModels(ChatProviderType type);

    /// <summary>
    /// 创建新会话.
    /// </summary>
    /// <returns><see cref="ChatSession"/>.</returns>
    ChatSession CreateSession(ChatProviderType providerType, BaseFieldParameters parameters = null, string? modelId = null);

    /// <summary>
    /// 创建新会话.
    /// </summary>
    /// <param name="preset">会话预设.</param>
    /// <returns>会话信息.</returns>
    ChatSession CreateSession(ChatSessionPreset preset);

    /// <summary>
    /// 创建新群组会话.
    /// </summary>
    /// <param name="preset">群组预设.</param>
    /// <returns>群组会话.</returns>
    ChatGroup CreateSession(ChatGroupPreset preset);

    /// <summary>
    /// 获取模型列表.
    /// </summary>
    /// <param name="type">供应商类型.</param>
    /// <returns>模型列表.</returns>
    List<ChatModel> GetModels(ChatProviderType type);

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
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 发送群组消息.
    /// </summary>
    /// <param name="groupId">群组标识符.</param>
    /// <param name="message">群组消息.</param>
    /// <param name="messageAction">消息生成事件.</param>
    /// <param name="agents">助理列表.</param>
    /// <param name="cancellationToken">终止令牌.</param>
    /// <returns><see cref="Task"/>.</returns>
    Task SendGroupMessageAsync(
        string groupId,
        ChatMessage message,
        Action<ChatMessage> messageAction = default,
        List<ChatSessionPreset> agents = default,
        CancellationToken cancellationToken = default);
}
