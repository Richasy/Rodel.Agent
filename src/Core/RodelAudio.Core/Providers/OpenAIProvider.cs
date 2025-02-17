// Copyright (c) Richasy. All rights reserved.

using RodelAudio.Interfaces.Client;
using RodelAudio.Models.Client;

namespace RodelAudio.Core.Providers;

/// <summary>
/// Open AI 服务商.
/// </summary>
public sealed class OpenAIProvider : ProviderBase, IProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OpenAIProvider"/> class.
    /// </summary>
    public OpenAIProvider(OpenAIClientConfig config)
        : base(config.Key, config.CustomModels)
    {
        SetBaseUri(ProviderConstants.OpenAIApi, config.Endpoint);
        ServerModels = PredefinedModels.OpenAIModels;
        OrganizationId = config.OrganizationId;
    }

    /// <summary>
    /// 组织标识符.
    /// </summary>
    private string OrganizationId { get; }

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
                .AddOpenAITextToAudio(AccessKey, modelId, BaseUri?.ToString(), OrganizationId)
                .Build();
        }

        return Kernel;
    }
}
