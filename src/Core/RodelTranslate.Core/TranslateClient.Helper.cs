// Copyright (c) Rodel. All rights reserved.

using Microsoft.SemanticKernel;
using RodelAgent.Models.Abstractions;
using RodelTranslate.Interfaces.Client;
using RodelTranslate.Models.Client;
using RodelTranslate.Models.Constants;

namespace RodelTranslate.Core;

/// <summary>
/// 翻译客户端.
/// </summary>
public sealed partial class TranslateClient
{
    private TranslateExecutionSettings GetExecutionSettings(TranslateSession session)
    {
        session.Parameters ??= GetTranslateParameters(session.Provider);
        return GetProvider(session.Provider).ConvertExecutionSettings(session);
    }

    private Kernel? FindKernelProvider(ProviderType type)
        => GetProvider(type).GetOrCreateKernel();

    private IProvider GetProvider(ProviderType type)
        => _providerFactory.GetOrCreateProvider(type);

    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                // Release managed resources.
                _providerFactory?.Clear();
            }

            _disposedValue = true;
        }
    }

    /// <summary>
    /// 获取翻译参数.
    /// </summary>
    /// <returns><see cref="BaseFieldParameters"/>.</returns>
    /// <remarks>
    /// <para>这个方法会首先根据 <paramref name="type"/> 获取默认的参数，然后再将 <paramref name="additionalParams"/> 中的字段合并到默认参数中.</para>
    /// <para>这样就允许插入可变数量的参数.</para>
    /// </remarks>
    private BaseFieldParameters GetTranslateParameters(ProviderType type, BaseFieldParameters? additionalParams = null)
    {
        var parameters = _parameterFactory.CreateTranslateParameters(type);
        if (additionalParams != null)
        {
            parameters.SetDictionary(additionalParams.Fields);
        }

        return parameters;
    }
}
