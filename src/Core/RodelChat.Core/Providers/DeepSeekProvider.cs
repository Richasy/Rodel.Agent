// Copyright (c) Rodel. All rights reserved.

using Microsoft.SemanticKernel;
using RodelChat.Interfaces.Client;
using RodelChat.Models.Client;

namespace RodelChat.Core.Providers;

/// <summary>
/// DeepSeek 服务商.
/// </summary>
public sealed class DeepSeekProvider : ProviderBase, IProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DeepSeekProvider"/> class.
    /// </summary>
    public DeepSeekProvider(DeepSeekClientConfig config)
        : base(config.Key, config.CustomModels)
    {
        SetBaseUri(ProviderConstants.DeepSeekApi);
        ServerModels = PredefinedModels.DeepSeekModels;
    }

    /// <inheritdoc/>
    public Kernel? GetOrCreateKernel(string modelId)
    {
        if (ShouldRecreateKernel(modelId))
        {
            Kernel = Kernel.CreateBuilder()
                .AddOpenAIChatCompletion(modelId, BaseUri, AccessKey)
                .Build();
        }

        return Kernel;
    }
}
