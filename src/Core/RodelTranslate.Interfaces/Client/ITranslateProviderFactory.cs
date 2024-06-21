// Copyright (c) Rodel. All rights reserved.

using RodelTranslate.Models.Client;
using RodelTranslate.Models.Constants;

namespace RodelTranslate.Interfaces.Client;

/// <summary>
/// 翻译提供商工厂.
/// </summary>
public interface ITranslateProviderFactory
{
    /// <summary>
    /// 获取或创建提供商.
    /// </summary>
    /// <param name="type">提供商类型.</param>
    /// <returns>提供商.</returns>
    IProvider GetOrCreateProvider(ProviderType type);

    /// <summary>
    /// 清除所有提供商.
    /// </summary>
    void Clear();

    /// <summary>
    /// 重置配置.
    /// </summary>
    void ResetConfiguration(TranslateClientConfiguration configuration);
}
