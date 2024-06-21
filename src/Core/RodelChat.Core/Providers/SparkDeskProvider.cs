// Copyright (c) Rodel. All rights reserved.

using System.Text.Json.Serialization;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.SparkDesk;
using RodelAgent.Models.Abstractions;
using RodelChat.Interfaces.Client;
using RodelChat.Models.Client;

namespace RodelChat.Core.Providers;

/// <summary>
/// 百度千帆提供程序.
/// </summary>
public sealed class SparkDeskProvider : ProviderBase, IProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SparkDeskProvider"/> class.
    /// </summary>
    public SparkDeskProvider(SparkDeskClientConfig config)
        : base(config.Key, config.CustomModels)
    {
        Secret = config.Secret;
        AppId = config.AppId;
        ServerModels = PredefinedModels.SparkDeskModels;
    }

    /// <summary>
    /// 获取应用程序 ID.
    /// </summary>
    private string AppId { get; }

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
                .AddSparkDeskChatCompletion(AccessKey, Secret, AppId, ConvertToSparkVersion(modelId))
                .Build();
        }

        return Kernel;
    }

    /// <inheritdoc/>
    public override PromptExecutionSettings ConvertExecutionSettings(ChatSession sessionData)
        => new SparkDeskPromptExecutionSettings
        {
            MaxTokens = sessionData.Parameters.GetValueOrDefault<int>(nameof(SparkDeskChatParameters.MaxTokens)),
            Temperature = sessionData.Parameters.GetValueOrDefault<double>(nameof(SparkDeskChatParameters.Temperature)),
            ToolCallBehavior = SparkDeskToolCallBehavior.AutoInvokeKernelFunctions,
            ChatId = sessionData.Id,
            ModelId = sessionData.Model,
            TopK = Convert.ToInt32(sessionData.Parameters.GetValueOrDefault<int>(nameof(SparkDeskChatParameters.TopK))),
        };

    private static SparkDeskTextVersion ConvertToSparkVersion(string modelId)
        => modelId switch
        {
            "V1_5" => SparkDeskTextVersion.V1_5,
            "V2" => SparkDeskTextVersion.V2,
            "V3" => SparkDeskTextVersion.V3,
            "V3_5" => SparkDeskTextVersion.V3_5,
            _ => throw new NotSupportedException("Version not supported."),
        };

    /// <summary>
    /// 星火对话参数.
    /// </summary>
    public sealed class SparkDeskChatParameters : BaseFieldParameters
    {
        /// <summary>
        /// 温度.
        /// </summary>
        [RangeFloatField(0.01, 1)]
        [JsonPropertyName("temperature")]
        public double? Temperature { get; set; } = 0.7;

        /// <summary>
        /// 最大输出标记数.
        /// </summary>
        [JsonPropertyName("max_tokens")]
        [RangeIntField(0, 8192)]
        public int? MaxTokens { get; set; }

        /// <summary>
        /// 顶部 K.
        /// </summary>
        [JsonPropertyName("top_k")]
        [RangeIntField(1, 6)]
        public int? TopK { get; set; } = 1;
    }
}
