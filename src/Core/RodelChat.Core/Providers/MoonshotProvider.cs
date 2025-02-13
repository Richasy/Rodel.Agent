// Copyright (c) Rodel. All rights reserved.

using Microsoft.SemanticKernel;
using RodelChat.Interfaces.Client;
using RodelChat.Models.Client;

namespace RodelChat.Core.Providers;

/// <summary>
/// 月之暗面服务商.
/// </summary>
public sealed class MoonshotProvider : ProviderBase, IProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MoonshotProvider"/> class.
    /// </summary>
    public MoonshotProvider(MoonshotClientConfig config)
        : base(config.Key, config.CustomModels)
    {
        ServerModels = PredefinedModels.MoonshotModels;
        SetBaseUri(ProviderConstants.MoonshotApi);
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
