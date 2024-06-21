// Copyright (c) Rodel. All rights reserved.

using RodelAgent.Models.Abstractions;
using RodelTranslate.Core.Providers;
using RodelTranslate.Interfaces.Client;
using RodelTranslate.Models.Constants;

namespace RodelTranslate.Console;

/// <summary>
/// 翻译参数工厂.
/// </summary>
public sealed class TranslateParametersFactory : ITranslateParametersFactory
{
    /// <inheritdoc/>
    public BaseFieldParameters CreateTranslateParameters(ProviderType provider)
    {
        return provider switch
        {
            ProviderType.Azure => new AzureProvider.AzureTranslateParameters(),
            ProviderType.Tencent => new TencentProvider.TencentTranslateParameters(),
            ProviderType.Ali => new AliProvider.AliTranslateParameters(),
            _ => default,
        };
    }
}
