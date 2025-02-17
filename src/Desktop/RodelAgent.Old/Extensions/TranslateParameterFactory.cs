// Copyright (c) Richasy. All rights reserved.

using RodelTranslate.Core.Providers;
using RodelTranslate.Interfaces.Client;
using RodelTranslate.Models.Constants;

namespace RodelAgent.UI.Extensions;

/// <summary>
/// 翻译参数工厂.
/// </summary>
public sealed class TranslateParameterFactory : ITranslateParametersFactory
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
