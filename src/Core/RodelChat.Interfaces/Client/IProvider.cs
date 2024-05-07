// Copyright (c) Rodel. All rights reserved.

using System.Collections.Generic;
using Microsoft.SemanticKernel;
using RodelChat.Models.Chat;

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
    Kernel? GetOrCreateKernel(string modelId);

    /// <summary>
    /// 获取模型信息.
    /// </summary>
    /// <param name="modelId">模型标识符.</param>
    /// <returns>模型信息或者 <c>null</c>.</returns>
    ChatModel? GetModelOrDefault(string modelId);

    /// <summary>
    /// 转换执行设置.
    /// </summary>
    /// <param name="parameters">会话参数.</param>
    /// <returns>执行设置.</returns>
    PromptExecutionSettings ConvertExecutionSettings(ChatParameters parameters);

    /// <summary>
    /// 设置自定义模型列表.
    /// </summary>
    /// <param name="models">自定义模型列表.</param>
    void SetCustomModels(List<ChatModel> models);

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
