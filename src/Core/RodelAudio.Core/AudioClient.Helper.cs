// Copyright (c) Rodel. All rights reserved.

using Microsoft.SemanticKernel;
using RodelAgent.Models.Abstractions;
using RodelAudio.Interfaces.Client;
using RodelAudio.Models.Client;
using RodelAudio.Models.Constants;

namespace RodelAudio.Core;

/// <summary>
/// 绘图客户端的帮助方法.
/// </summary>
public sealed partial class AudioClient
{
    private PromptExecutionSettings GetExecutionSettings(AudioSession session)
        => GetProvider(session.Provider).ConvertExecutionSettings(session);

    private Kernel? FindKernelProvider(ProviderType type, string modelId)
        => GetProvider(type).GetOrCreateKernel(modelId);

    private IProvider GetProvider(ProviderType type)
        => _providerFactory.GetOrCreateProvider(type);

    private AudioModel FindModelInProvider(ProviderType type, string modelId)
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

    private BaseFieldParameters GetAudioParameters(ProviderType type, BaseFieldParameters? additionalParams = null)
    {
        var parameters = _parameterFactory.CreateAudioParameters(type);
        if (additionalParams != null)
        {
            parameters.SetDictionary(additionalParams.Fields);
        }

        return parameters;
    }
}
