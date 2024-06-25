// Copyright (c) Rodel. All rights reserved.

using System.Text.Json.Serialization;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.QianFan;
using RodelAgent.Models.Abstractions;
using RodelChat.Interfaces.Client;
using RodelChat.Models.Client;

namespace RodelChat.Core.Providers;

/// <summary>
/// 百度千帆提供程序.
/// </summary>
public sealed class QianFanProvider : ProviderBase, IProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="QianFanProvider"/> class.
    /// </summary>
    public QianFanProvider(QianFanClientConfig config)
        : base(config.Key, config.CustomModels)
    {
        SetBaseUri(ProviderConstants.QianFanApi);
        Secret = config.Secret;
        ServerModels = PredefinedModels.QianFanModels;
    }

    /// <summary>
    /// 获取密钥.
    /// </summary>
    private string Secret { get; }

    /// <inheritdoc/>
    public Kernel? GetOrCreateKernel(string modelId)
    {
        if (ShouldRecreateKernel(modelId))
        {
            Kernel = Kernel.CreateBuilder()
                .AddQianFanChatCompletion(modelId, AccessKey, Secret)
                .Build();
        }

        return Kernel;
    }

    /// <inheritdoc/>
    public override PromptExecutionSettings ConvertExecutionSettings(ChatSessionPreset sessionData)
        => new QianFanPromptExecutionSettings
        {
            MaxTokens = sessionData.Parameters.GetValueOrDefault<int>(nameof(QianFanChatParameters.MaxOutputTokens)),
            Temperature = sessionData.Parameters.GetValueOrDefault<double>(nameof(QianFanChatParameters.Temperature)),
            TopP = sessionData.Parameters.GetValueOrDefault<double>(nameof(QianFanChatParameters.TopP)),
            PenaltyScore = sessionData.Parameters.GetValueOrDefault<double>(nameof(QianFanChatParameters.PenaltyScore)),
            StopSequences = sessionData.StopSequences,
            EnableCitation = sessionData.Parameters.GetValueOrDefault<bool>(nameof(QianFanChatParameters.EnableCitation)),
            EnableTrace = sessionData.Parameters.GetValueOrDefault<bool>(nameof(QianFanChatParameters.EnableTrace)),
            DisableSearch = sessionData.Parameters.GetValueOrDefault<bool>(nameof(QianFanChatParameters.DisableSearch)),
            ResponseFormat = sessionData.Parameters.GetValueOrDefault<string>(nameof(QianFanChatParameters.ResponseFormat)),
            ModelId = sessionData.Model,
        };

    /// <summary>
    /// 千帆聊天参数.
    /// </summary>
    [JsonConverter(typeof(BaseFieldParametersConverter))]
    public sealed class QianFanChatParameters : BaseFieldParameters
    {
        /// <summary>
        /// 温度数值.
        /// </summary>
        [JsonPropertyName("temperature")]
        [RangeFloatField(0.01, 1.0)]
        public double Temperature { get; set; } = 0.8;

        /// <summary>
        /// 影响多样性.
        /// </summary>
        [JsonPropertyName("top_p")]
        [RangeFloatField(0, 1)]
        public double TopP { get; set; } = 0.8;

        /// <summary>
        /// 重复惩罚.
        /// </summary>
        [JsonPropertyName("penalty_score")]
        [RangeFloatField(1, 2)]
        public double PenaltyScore { get; set; } = 1;

        /// <summary>
        /// 是否关闭实时搜索.
        /// </summary>
        [JsonPropertyName("disable_search")]
        [BooleanField]
        public bool DisableSearch { get; set; } = false;

        /// <summary>
        /// 是否启用引用.
        /// </summary>
        [JsonPropertyName("enable_citation")]
        [BooleanField]
        public bool EnableCitation { get; set; } = false;

        /// <summary>
        /// 是否启用跟踪.
        /// </summary>
        [JsonPropertyName("enable_trace")]
        [BooleanField]
        public bool EnableTrace { get; set; } = false;

        /// <summary>
        /// 最大输出标记数.
        /// </summary>
        [JsonPropertyName("max_output_tokens")]
        [RangeIntField(0, 2048)]
        public int? MaxOutputTokens { get; set; }

        /// <summary>
        /// 响应格式.
        /// </summary>
        [JsonPropertyName("response_format")]
        [SelectionField("text", "json_object")]
        public string? ResponseFormat { get; set; }
    }
}
