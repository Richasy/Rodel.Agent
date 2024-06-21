// Copyright (c) Rodel. All rights reserved.

using RodelAgent.Models.Abstractions;
using RodelTranslate.Models.Constants;

namespace RodelTranslate.Interfaces.Client;

/// <summary>
/// 翻译参数工厂.
/// </summary>
public interface ITranslateParametersFactory
{
    /// <summary>
    /// 创建翻译参数.
    /// </summary>
    /// <param name="provider">翻译供应商.</param>
    /// <returns>参数.</returns>
    BaseFieldParameters CreateTranslateParameters(ProviderType provider);
}
