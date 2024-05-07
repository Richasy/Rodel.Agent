// Copyright (c) Rodel. All rights reserved.

using Microsoft.SemanticKernel;
using RodelChat.Interfaces.Client;
using RodelChat.Models.Client;

namespace RodelChat.Core.Providers;

/// <summary>
/// 智谱服务商.
/// </summary>
public sealed class ZhiPuProvider : ProviderBase, IProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ZhiPuProvider"/> class.
    /// </summary>
    public ZhiPuProvider(ZhiPuClientConfig config)
        : base(config.Key, config.CustomModels)
    {
        SetBaseUri(ProviderConstants.ZhipuApi);
        ServerModels = PredefinedModels.ZhiPuModels;
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
