// Copyright (c) Rodel. All rights reserved.

using System.Text.Json.Serialization;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Google;
using RodelAgent.Models.Abstractions;
using RodelChat.Interfaces.Client;
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
        SetBaseUri(ProviderConstants.GeminiApi, config.Endpoint);
        ServerModels = PredefinedModels.GeminiModels;
    }

    /// <inheritdoc/>
    public Kernel? GetOrCreateKernel(string modelId)
    {
        if (ShouldRecreateKernel(modelId))
        {
            Service = Service.CreateBuilder()
                .AddGoogleAIGeminiChatCompletion(modelId, BaseUri, AccessKey)
                .Build();
        }

        return Service;
    }

    /// <inheritdoc/>
    public override PromptExecutionSettings ConvertExecutionSettings(ChatSessionPreset sessionData)
        => new GeminiPromptExecutionSettings
        {
            TopP = sessionData.Parameters.GetValueOrDefault<double>(nameof(GeminiChatParameters.TopP)),
            MaxTokens = sessionData.Parameters.GetValueOrDefault<int>(nameof(GeminiChatParameters.MaxTokens)),
            Temperature = sessionData.Parameters.GetValueOrDefault<double>(nameof(GeminiChatParameters.Temperature)),
            StopSequences = sessionData?.StopSequences,
            ModelId = sessionData.Model,
            TopK = sessionData.Parameters.GetValueOrDefault<int>(nameof(GeminiChatParameters.TopK)),
            ToolCallBehavior = GeminiToolCallBehavior.AutoInvokeKernelFunctions,
        };

    /// <summary>
    /// Gemini 对话参数.
    /// </summary>
    public sealed class GeminiChatParameters : BaseFieldParameters
    {
        /// <summary>
        /// 最大输出标记数.
        /// </summary>
        [JsonPropertyName("maxOutputTokens")]
        [RangeIntField(0, 4096)]
        public int? MaxTokens { get; set; }

        /// <summary>
        /// 温度.
        /// </summary>
        [JsonPropertyName("temperature")]
        [RangeFloatField(0, 2)]
        public double Temperature { get; set; } = 1;

        /// <summary>
        /// Top-P.
        /// </summary>
        [JsonPropertyName("topP")]
        [RangeFloatField(0, 1)]
        public double TopP { get; set; } = 1;

        /// <summary>
        /// Top-K.
        /// </summary>
        [RangeIntField(0, 50)]
        public int TopK { get; set; } = 20;
    }
}
