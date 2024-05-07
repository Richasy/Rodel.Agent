// Copyright (c) Rodel. All rights reserved.

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Google;
using RodelChat.Interfaces.Client;
using RodelChat.Models.Chat;
using RodelChat.Models.Client;

namespace RodelChat.Core.Providers;

/// <summary>
/// Gemini 服务商.
/// </summary>
public sealed class GeminiProvider : ProviderBase, IProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GeminiProvider"/> class.
    /// </summary>
    public GeminiProvider(GeminiClientConfig config)
        : base(config.Key, config.CustomModels)
    {
        SetBaseUri(ProviderConstants.GeminiApi);
        ServerModels = PredefinedModels.GeminiModels;
    }

    /// <inheritdoc/>
    public Kernel? GetOrCreateKernel(string modelId)
    {
        if (ShouldRecreateKernel(modelId))
        {
            Kernel = Kernel.CreateBuilder()
                .AddGoogleAIGeminiChatCompletion(modelId, AccessKey)
                .Build();
        }

        return Kernel;
    }

    /// <inheritdoc/>
    public override PromptExecutionSettings ConvertExecutionSettings(ChatParameters parameters)
        => new GeminiPromptExecutionSettings
        {
            TopP = parameters.TopP,
            MaxTokens = parameters.MaxTokens,
            Temperature = parameters.Temperature,
            ToolCallBehavior = GeminiToolCallBehavior.AutoInvokeKernelFunctions,
        };
}
