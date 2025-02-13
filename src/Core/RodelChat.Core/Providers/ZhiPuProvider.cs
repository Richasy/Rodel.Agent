// Copyright (c) Rodel. All rights reserved.

using System.Text.Json.Serialization;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using RodelAgent.Models.Abstractions;
using RodelChat.Interfaces.Client;
using RodelChat.Models.Client;

namespace RodelChat.Core.Providers;

/// <summary>
/// 智谱服务商.
/// </summary>
public sealed class ZhiPuProvider : ProviderBase, IProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ZhiPuProvider"/> class.
    /// </summary>
    public ZhiPuProvider(ZhiPuClientConfig config)
        : base(config.Key, config.CustomModels)
    {
        SetBaseUri(ProviderConstants.ZhipuApi);
        ServerModels = PredefinedModels.ZhiPuModels;
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

    /// <inheritdoc/>
    public override PromptExecutionSettings ConvertExecutionSettings(ChatSessionPresetOld sessionData)
        => new OpenAIPromptExecutionSettings
        {
            MaxTokens = sessionData.Parameters.GetValueOrDefault<int>(nameof(ZhiPuChatParameters.MaxTokens)),
            Temperature = sessionData.Parameters.GetValueOrDefault<double>(nameof(ZhiPuChatParameters.Temperature)),
            TopP = sessionData.Parameters.GetValueOrDefault<double>(nameof(OpenAIChatParameters.TopP)),
            ChatSystemPrompt = sessionData.SystemInstruction,
            ModelId = sessionData.Model,
            StopSequences = sessionData.StopSequences,
            ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
        };

    /// <summary>
    /// 智谱的聊天参数.
    /// </summary>
    [JsonConverter(typeof(BaseFieldParametersConverter))]
    public sealed class ZhiPuChatParameters : BaseFieldParameters
    {
        /// <summary>
        /// 生成文本的最大长度.
        /// </summary>
        [JsonPropertyName("max_tokens")]
        [RangeIntField(0, 8192)]
        public int? MaxTokens { get; set; }

        /// <summary>
        /// 用于控制文本的多样性和创造性.
        /// </summary>
        [JsonPropertyName("temperature")]
        [RangeFloatField(0.01, 0.99)]
        public double Temperature { get; set; } = 0.95;

        /// <summary>
        /// 用于控制生成文本的顶部概率.
        /// </summary>
        [JsonPropertyName("top_p")]
        [RangeFloatField(0.01, 0.99)]
        public double TopP { get; set; } = 0.7d;
    }
}
