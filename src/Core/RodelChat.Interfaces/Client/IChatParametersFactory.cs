// Copyright (c) Rodel. All rights reserved.

using Richasy.AgentKernel;
using RodelAgent.Models.Abstractions;

namespace RodelChat.Interfaces.Client;

/// <summary>
/// 聊天参数工厂.
/// </summary>
public interface IChatParametersFactory
{
    /// <summary>
    /// 创建聊天参数.
    /// </summary>
    /// <param name="provider">聊天供应商.</param>
    /// <returns>参数.</returns>
    BaseFieldParameters CreateChatParameters(ChatProviderType provider);
}
