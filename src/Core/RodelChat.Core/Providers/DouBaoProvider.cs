// Copyright (c) Rodel. All rights reserved.

using System.Text.Json.Serialization;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.DouBao;
using RodelAgent.Models.Abstractions;
using RodelChat.Interfaces.Client;
using RodelChat.Models.Client;

namespace RodelChat.Core.Providers;

/// <summary>
/// Azure Open AI 服务商.
/// </summary>
public sealed class DouBaoProvider : ProviderBase, IProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DouBaoProvider"/> class.
    /// </summary>
    public DouBaoProvider(DouBaoClientConfig config)
        : base(config.Key, config.CustomModels)
    {
    }

    /// <inheritdoc/>
    public Kernel? GetOrCreateKernel(string modelId)
    {
        if (ShouldRecreateKernel(modelId))
        {
            Kernel = Kernel.CreateBuilder()
                .AddDouBaoChatCompletion(modelId, AccessKey)
                .Build();
        }

        return Kernel;
    }

    /// <inheritdoc/>
    public override PromptExecutionSettings ConvertExecutionSettings(ChatSessionPreset sessionData)
    {
        var maxTokens = sessionData.Parameters.GetValueOrDefault<int>(nameof(DouBaoChatParameters.MaxTokens));
        return new DouBaoPromptExecutionSettings
        {
            Temperature = sessionData.Parameters.GetValueOrDefault<double>(nameof(DouBaoChatParameters.Temperature)),
            TopP = sessionData.Parameters.GetValueOrDefault<double>(nameof(DouBaoChatParameters.TopP)),
            MaxTokens = maxTokens == 0 ? default : maxTokens,
            FrequencyPenalty = sessionData.Parameters.GetValueOrDefault<double>(nameof(DouBaoChatParameters.FrequencyPenalty)),
            ModelId = sessionData.Model,
        };
    }

    /// <summary>
    /// 混元大模型参数.
    /// </summary>
    public sealed class DouBaoChatParameters : BaseFieldParameters
    {
        /// <summary>
        /// 顶部P值.
        /// </summary>
        [JsonPropertyName("top_p")]
        [RangeFloatField(0, 1)]
        public double? TopP { get; set; } = 1d;

        /// <summary>
        /// 温度.
        /// </summary>
        [JsonPropertyName("temperature")]
        [RangeFloatField(0, 2)]
        public double Temperature { get; set; } = 0.7d;

        /// <summary>
        /// 用于减少模型生成的重复性的参数.
        /// </summary>
        [JsonPropertyName("frequency_penalty")]
        [RangeFloatField(-2, 2)]
        public double FrequencyPenalty { get; set; } = 0d;

        /// <summary>
        /// 生成文本的最大长度.
        /// </summary>
        [JsonPropertyName("max_tokens")]
        [RangeIntField(0, int.MaxValue)]
        public int? MaxTokens { get; set; }
    }
}
