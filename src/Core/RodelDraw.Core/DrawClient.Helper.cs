// Copyright (c) Rodel. All rights reserved.

using Microsoft.SemanticKernel;
using RodelAgent.Models.Abstractions;
using RodelDraw.Interfaces.Client;
using RodelDraw.Models.Client;
using RodelDraw.Models.Constants;

namespace RodelDraw.Core;

/// <summary>
/// 绘图客户端的帮助方法.
/// </summary>
public sealed partial class DrawClient
{
    private DrawParameters GetExecutionSettings(DrawSession session)
        => GetProvider(session.Provider).ConvertDrawParameters(session);

    private Kernel? FindKernelProvider(ProviderType type, string modelId)
        => GetProvider(type).GetOrCreateKernel(modelId);

    private IProvider GetProvider(ProviderType type)
        => _providerFactory.GetOrCreateProvider(type);

    private DrawModel FindModelInProvider(ProviderType type, string modelId)
        => GetProvider(type).GetModelOrDefault(modelId);

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

    private BaseFieldParameters GetDrawParameters(ProviderType type, BaseFieldParameters? additionalParams = null)
    {
        var parameters = _parameterFactory.CreateDrawParameters(type);
        if (additionalParams != null)
        {
            parameters.SetDictionary(additionalParams.Fields);
        }

        return parameters;
    }
}
