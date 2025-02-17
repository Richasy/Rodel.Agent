// Copyright (c) Richasy. All rights reserved.

using RodelAudio.Interfaces.Client;
using RodelAudio.Models.Client;

namespace RodelAudio.Core.Providers;

/// <summary>
/// Azure OpenAI 服务提供者.
/// </summary>
public sealed class AzureOpenAIProvider : ProviderBase, IProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AzureOpenAIProvider"/> class.
    /// </summary>
    public AzureOpenAIProvider(AzureOpenAIClientConfig config)
        : base(config.Key, config.CustomModels)
    {
        SetBaseUri(config.Endpoint);
        ServerModels = PredefinedModels.AzureOpenAIModels;
    }

    /// <inheritdoc/>
    public PromptExecutionSettings ConvertExecutionSettings(AudioSession sessionData)
    {
        return new OpenAITextToAudioExecutionSettings
        {
            ModelId = sessionData.Model,
            Speed = (float)(sessionData.Speed ?? 1.0),
            ResponseFormat = "wav",
            Voice = sessionData.Voice,
        };
    }

    /// <inheritdoc/>
    public Kernel? GetOrCreateKernel(string modelId)
    {
        if (ShouldRecreateKernel(modelId))
        {
            Kernel = Kernel.CreateBuilder()
                .AddAzureOpenAITextToAudio(modelId, endpoint: BaseUri.AbsoluteUri, apiKey: AccessKey, modelId: modelId)
                .Build();
        }

        return Kernel;
    }
}
