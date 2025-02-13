// Copyright (c) Rodel. All rights reserved.

using Microsoft.SemanticKernel;
using RodelChat.Interfaces.Client;
using RodelChat.Models.Client;

namespace RodelChat.Core.Providers;

/// <summary>
/// Open Router 服务商.
/// </summary>
public sealed class OpenRouterProvider : ProviderBase, IProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OpenRouterProvider"/> class.
    /// </summary>
    public OpenRouterProvider(OpenRouterClientConfig config)
        : base(config.Key, config.CustomModels)
    {
        SetBaseUri(ProviderConstants.OpenRouterApi);
        ServerModels = PredefinedModels.OpenRouterModels;
    }

    /// <inheritdoc/>
    public Kernel? GetOrCreateKernel(string modelId)
    {
        if (ShouldRecreateKernel(modelId))
        {
            Service = Service.CreateBuilder()
                .AddOpenAIChatCompletion(modelId, BaseUri, AccessKey)
                .Build();
        }

        return Service;
    }
}
