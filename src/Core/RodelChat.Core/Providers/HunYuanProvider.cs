// Copyright (c) Rodel. All rights reserved.

using System.Text.Json.Serialization;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.HunYuan;
using RodelAgent.Models.Abstractions;
using RodelChat.Interfaces.Client;
using RodelChat.Models.Client;

namespace RodelChat.Core.Providers;

/// <summary>
/// 百度千帆提供程序.
/// </summary>
public sealed class HunYuanProvider : ProviderBase, IProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HunYuanProvider"/> class.
    /// </summary>
    public HunYuanProvider(HunYuanClientConfig config)
        : base(config.Key, config.CustomModels)
    {
        SetBaseUri(ProviderConstants.HunYuanApi);
        SecretId = config.SecretId;
        ServerModels = PredefinedModels.HunYuanModels;
    }

    /// <summary>
    /// 密钥ID.
    /// </summary>
    private string SecretId { get; }

    /// <inheritdoc/>
    public Kernel? GetOrCreateKernel(string modelId)
    {
        if (ShouldRecreateKernel(modelId))
        {
            Kernel = Kernel.CreateBuilder()
                .AddHunYuanChatCompletion(modelId, SecretId, AccessKey)
                .Build();
        }

        return Kernel;
    }

    /// <inheritdoc/>
    public override PromptExecutionSettings ConvertExecutionSettings(ChatSession sessionData)
        => new HunYuanPromptExecutionSettings
        {
            Temperature = sessionData.Parameters.GetValueOrDefault<double>(nameof(HunYuanChatParameters.Temperature)),
            TopP = sessionData.Parameters.GetValueOrDefault<double>(nameof(HunYuanChatParameters.TopP)),
            ModelId = sessionData.Model,
        };

    /// <summary>
    /// 混元大模型参数.
    /// </summary>
    public sealed class HunYuanChatParameters : BaseFieldParameters
    {
        /// <summary>
        /// 顶部P值.
        /// </summary>
        [JsonPropertyName("top_p")]
        [RangeFloatField(0, 1)]
        public double? TopP { get; set; }

        /// <summary>
        /// 温度.
        /// </summary>
        [JsonPropertyName("temperature")]
        [RangeFloatField(0, 2)]
        public double Temperature { get; set; }
    }
}

