// Copyright (c) Rodel. All rights reserved.

using Microsoft.SemanticKernel;
using RodelChat.Interfaces.Client;
using RodelChat.Models.Client;

namespace RodelChat.Core.Providers;

/// <summary>
/// Silicon Flow 服务商.
/// </summary>
public sealed class SiliconFlowProvider : ProviderBase, IProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SiliconFlowProvider"/> class.
    /// </summary>
    public SiliconFlowProvider(SiliconFlowClientConfig config)
        : base(config.Key, config.CustomModels)
    {
        ServerModels = PredefinedModels.SiliconFlowModels;
        SetBaseUri(ProviderConstants.SiliconFlowApi);
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
