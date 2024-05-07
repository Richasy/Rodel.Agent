// Copyright (c) Rodel. All rights reserved.

using Microsoft.SemanticKernel;
using RodelChat.Interfaces.Client;
using RodelChat.Models.Client;

namespace RodelChat.Core.Providers;

/// <summary>
/// Perplexity 服务商.
/// </summary>
public sealed class PerplexityProvider : ProviderBase, IProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PerplexityProvider"/> class.
    /// </summary>
    public PerplexityProvider(PerplexityClientConfig config)
        : base(config.Key, config.CustomModels)
    {
        SetBaseUri(ProviderConstants.PerplexityApi);
        ServerModels = PredefinedModels.PerplexityModels;
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
