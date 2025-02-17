// Copyright (c) Richasy. All rights reserved.

using RodelTranslate.Models.Client;

namespace RodelTranslate.Interfaces.Client;

/// <summary>
/// 服务提供商.
/// </summary>
public interface IProvider
{
    /// <summary>
    /// 获取语言列表.
    /// </summary>
    /// <returns>语言列表.</returns>
    List<Language> GetLanguages();

    /// <summary>
    /// 获取或创建内核.
    /// </summary>
    /// <returns>内核.</returns>
    Kernel? GetOrCreateKernel();

    /// <summary>
    /// 转换执行设置.
    /// </summary>
    /// <param name="sessionData">会话.</param>
    /// <returns>执行设置.</returns>
    TranslateExecutionSettings ConvertExecutionSettings(TranslateSession sessionData);

    /// <summary>
    /// 获取最大文本长度.
    /// </summary>
    /// <returns>最大文本长度.</returns>
    long GetMaxTextLength();

    /// <summary>
    /// 释放资源.
    /// </summary>
    void Release();
}
