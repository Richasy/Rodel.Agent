// Copyright (c) Rodel. All rights reserved.

using System.Text.Json.Serialization;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Anthropic;
using RodelAgent.Models.Abstractions;
using RodelChat.Interfaces.Client;
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
            Service = Service.CreateBuilder()
                .AddAnthropicChatCompletion(modelId, AccessKey, BaseUri)
                .Build();
        }

        return Service;
    }

    /// <inheritdoc/>
    public override PromptExecutionSettings ConvertExecutionSettings(ChatSessionPreset sessionData)
        => new AnthropicPromptExecutionSettings
        {
            MaxTokens = sessionData.Parameters.GetValueOrDefault<int>(nameof(AnthropicChatParameters.MaxTokens)),
            Temperature = sessionData.Parameters.GetValueOrDefault<double>(nameof(AnthropicChatParameters.Temperature)),
            TopP = sessionData.Parameters.GetValueOrDefault<double>(nameof(AnthropicChatParameters.TopP)),
            StopSequences = sessionData.StopSequences,
            Stream = sessionData.UseStreamOutput,
            ModelId = sessionData.Model,
            TopK = sessionData.Parameters.GetValueOrDefault<int>(nameof(AnthropicChatParameters.TopK)),
        };

    /// <summary>
    /// Anthropic 对话参数.
    /// </summary>
    [JsonConverter(typeof(BaseFieldParametersConverter))]
    public sealed class AnthropicChatParameters : BaseFieldParameters
    {
        /// <summary>
        /// 最大输出标记数.
        /// </summary>
        [JsonPropertyName("max_tokens")]
        [JsonRequired]
        [RangeIntField(0, 2048)]
        public int? MaxTokens { get; set; }

        /// <summary>
        /// 温度.
        /// </summary>
        [JsonPropertyName("temperature")]
        [RangeFloatField(0, 1)]
        public double Temperature { get; set; } = 1;

        /// <summary>
        /// 顶部概率.
        /// </summary>
        [JsonPropertyName("top_p")]
        [RangeFloatField(0.01, 1)]
        public double? TopP { get; set; }

        /// <summary>
        /// 顶部 K.
        /// </summary>
        [JsonPropertyName("top_k")]
        [RangeIntField(1, 2048)]
        public int? TopK { get; set; }
    }
}
