// Copyright (c) Rodel. All rights reserved.

using Microsoft.SemanticKernel;
using RodelChat.Interfaces.Client;
using RodelChat.Models.Client;

namespace RodelChat.Core.Providers;

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
    public Kernel? GetOrCreateKernel(string modelId)
    {
        if (ShouldRecreateKernel(modelId))
        {
            Service = Service.CreateBuilder()
                .AddOpenAIChatCompletion(modelId, BaseUri, AccessKey, OrganizationId)
                .Build();
        }

        return Service;
    }
}
