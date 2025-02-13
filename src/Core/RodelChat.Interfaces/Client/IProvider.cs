// Copyright (c) Rodel. All rights reserved.

using System.Collections.Generic;
using Microsoft.Extensions.AI;
using Richasy.AgentKernel.Chat;
using RodelChat.Models.Client;

namespace RodelChat.Interfaces.Client;

/// <summary>
/// 在线服务商.
/// </summary>
public interface IProvider
{
    /// <summary>
    /// 创建一个内核.
    /// </summary>
    /// <param name="modelId">要使用的模型标识符.</param>
    /// <returns>内核.</returns>
    IChatService GetChatService(string modelId);

    /// <summary>
    /// 获取模型信息.
    /// </summary>
    /// <param name="modelId">模型标识符.</param>
    /// <returns>模型信息或者 <c>null</c>.</returns>
    ChatModel? GetModelOrDefault(string modelId);

    /// <summary>
    /// 转换执行设置.
    /// </summary>
    /// <param name="sessionData">会话.</param>
    /// <returns>执行设置.</returns>
    ChatOptions ConvertExecutionSettings(ChatSessionPreset sessionData);

    /// <summary>
    /// 获取模型列表.
    /// </summary>
    /// <returns>模型列表.</returns>
    List<ChatModel> GetModelList();

    /// <summary>
    /// 释放资源.
    /// </summary>
    void Release();
}
