// Copyright (c) Rodel. All rights reserved.

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Anthropic;
using RodelChat.Interfaces.Client;
using RodelChat.Models.Chat;
using RodelChat.Models.Client;

namespace RodelChat.Core.Providers;

/// <summary>
/// Anthropic 服务商.
/// </summary>
public sealed class AnthropicProvider : ProviderBase, IProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AnthropicProvider"/> class.
    /// </summary>
    public AnthropicProvider(AnthropicClientConfig config)
        : base(config.Key, config.CustomModels)
    {
        ServerModels = PredefinedModels.AnthropicModels;
        SetBaseUri(ProviderConstants.AnthropicApi, config.Endpoint);
    }

    /// <inheritdoc/>
    public Kernel? GetOrCreateKernel(string modelId)
    {
        if (ShouldRecreateKernel(modelId))
        {
            Kernel = Kernel.CreateBuilder()
                .AddAnthropicChatCompletion(modelId, AccessKey, BaseUri)
                .Build();
        }

        return Kernel;
    }

    /// <inheritdoc/>
    public override PromptExecutionSettings ConvertExecutionSettings(ChatParameters parameters)
        => new AnthropicPromptExecutionSettings
        {
            MaxTokens = parameters.MaxTokens,
            Temperature = parameters.Temperature,
            TopP = parameters.TopP,
        };
}
