// Copyright (c) Richasy. All rights reserved.

using RodelAudio.Models.Client;

namespace RodelAudio.Interfaces.Client;

/// <summary>
/// 服务提供者.
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
    /// 获取当前内核.
    /// </summary>
    /// <returns>如果有，则返回当前正在使用的内核.</returns>
    Kernel? GetCurrentKernel();

    /// <summary>
    /// 获取模型信息.
    /// </summary>
    /// <param name="modelId">模型标识符.</param>
    /// <returns>模型信息或者 <c>null</c>.</returns>
    AudioModel? GetModelOrDefault(string modelId);

    /// <summary>
    /// 转换执行设置.
    /// </summary>
    /// <param name="sessionData">会话.</param>
    /// <returns>执行设置.</returns>
    PromptExecutionSettings ConvertExecutionSettings(AudioSession sessionData);

    /// <summary>
    /// 获取模型列表.
    /// </summary>
    /// <returns>模型列表.</returns>
    List<AudioModel> GetModelList();

    /// <summary>
    /// 释放资源.
    /// </summary>
    void Release();
}
