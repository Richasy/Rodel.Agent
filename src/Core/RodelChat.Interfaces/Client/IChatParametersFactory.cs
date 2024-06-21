// Copyright (c) Rodel. All rights reserved.

using RodelAgent.Models.Abstractions;
using RodelChat.Models.Constants;

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
    BaseFieldParameters CreateChatParameters(ProviderType provider);
}
