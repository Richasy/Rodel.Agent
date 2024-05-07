// Copyright (c) Rodel. All rights reserved.

using Microsoft.SemanticKernel;
using RodelChat.Interfaces.Client;
using RodelChat.Models.Client;

namespace RodelChat.Core.Providers;

/// <summary>
/// 零一万物服务商.
/// </summary>
public sealed class LingYiProvider : ProviderBase, IProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LingYiProvider"/> class.
    /// </summary>
    public LingYiProvider(LingYiClientConfig config)
        : base(config.Key, config.CustomModels)
    {
        SetBaseUri(ProviderConstants.LingYiApi);
        ServerModels = PredefinedModels.LingYiModels;
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
