// Copyright (c) Rodel. All rights reserved.

using Microsoft.SemanticKernel;
using RodelChat.Interfaces.Client;
using RodelChat.Models.Client;

namespace RodelChat.Core.Providers;

/// <summary>
/// 阿里灵积（包含通义千问）.
/// </summary>
public sealed class GroqProvider : ProviderBase, IProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GroqProvider"/> class.
    /// </summary>
    public GroqProvider(GroqClientConfig config)
        : base(config.Key, config.CustomModels)
    {
        SetBaseUri(ProviderConstants.GroqApi);
        ServerModels = PredefinedModels.GroqModels;
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
