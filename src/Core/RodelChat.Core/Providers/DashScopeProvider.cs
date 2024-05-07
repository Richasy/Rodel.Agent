// Copyright (c) Rodel. All rights reserved.

using Microsoft.SemanticKernel;
using RodelChat.Interfaces.Client;
using RodelChat.Models.Client;

namespace RodelChat.Core.Providers;

/// <summary>
/// 阿里灵积（包含通义千问）.
/// </summary>
public sealed class DashScopeProvider : ProviderBase, IProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DashScopeProvider"/> class.
    /// </summary>
    public DashScopeProvider(DashScopeClientConfig config)
        : base(config.Key, config.CustomModels)
    {
        SetBaseUri(ProviderConstants.DashScopeApi);
        ServerModels = PredefinedModels.DashScopeModels;
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
