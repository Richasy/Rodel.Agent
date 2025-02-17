// Copyright (c) Richasy. All rights reserved.

using RodelAudio.Core.Providers;
using RodelAudio.Models.Client;
using RodelAudio.Models.Constants;

namespace RodelAudio.Core.Factories;

/// <summary>
/// 音频服务商工厂.
/// </summary>
public sealed partial class AudioProviderFactory
{
    private void InjectOpenAI(OpenAIClientConfig? config)
    {
        if (config?.IsValid() ?? false)
        {
            AddCreateMethod(ProviderType.OpenAI, () => new OpenAIProvider(config));
        }
    }

    private void InjectAzureOpenAI(AzureOpenAIClientConfig? config)
    {
        if (config?.IsValid() ?? false)
        {
            AddCreateMethod(ProviderType.AzureOpenAI, () => new AzureOpenAIProvider(config));
        }
    }

    private void InjectAzureSpeech(AzureSpeechClientConfig? config)
    {
        if (config?.IsValid() ?? false)
        {
            AddCreateMethod(ProviderType.AzureSpeech, () => new AzureSpeechProvider(config));
        }
    }
}
