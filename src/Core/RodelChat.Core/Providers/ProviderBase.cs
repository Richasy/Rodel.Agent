// Copyright (c) Rodel. All rights reserved.

using System.Text.Json.Serialization;
using Microsoft.Extensions.AI;
using Richasy.AgentKernel;
using Richasy.AgentKernel.Chat;
using RodelAgent.Models.Abstractions;
using RodelChat.Models.Client;

namespace RodelChat.Core.Providers;

/// <summary>
/// 服务商基类.
/// </summary>
public abstract class ProviderBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProviderBase"/> class.
    /// </summary>
    protected ProviderBase(string key, List<ChatModel>? customModels = null)
    {
        AccessKey = key;

        if (customModels != null)
        {
            CustomModels = customModels;
        }
    }

    /// <summary>
    /// 自定义的模型列表.
    /// </summary>
    public List<ChatModel>? CustomModels { get; set; }

    /// <summary>
    /// 服务端模型列表.
    /// </summary>
    public List<ChatModel>? ServerModels { get; set; }

    /// <summary>
    /// 访问密钥.
    /// </summary>
    protected string? AccessKey { get; set; }

    /// <summary>
    /// 内核.
    /// </summary>
    protected IChatService? Service { get; set; }

    /// <summary>
    /// 基础 URL.
    /// </summary>
    protected Uri? BaseUri { get; private set; }

    /// <summary>
    /// 服务商类型.
    /// </summary>
    protected ChatProviderType Type { get; init; }

    /// <summary>
    /// 获取模型信息.
    /// </summary>
    /// <param name="modelId">模型标识符.</param>
    /// <returns>模型信息或者 <c>null</c>.</returns>
    public ChatModel GetModelOrDefault(string modelId)
        => CustomModels?.FirstOrDefault(m => m.Id == modelId)
            ?? ServerModels?.FirstOrDefault(m => m.Id == modelId)
            ?? default;

    /// <summary>
    /// 获取模型列表.
    /// </summary>
    /// <returns>模型列表.</returns>
    public List<ChatModel> GetModelList()
    {
        var models = new List<ChatModel>();
        if (ServerModels != null)
        {
            models.AddRange(ServerModels);
        }

        if (CustomModels != null)
        {
            models.AddRange(CustomModels);
        }

        return [.. models.Distinct().OrderByDescending(p => p.IsCustomModel)];
    }

    /// <summary>
    /// 释放资源.
    /// </summary>
    public void Release()
        => Service = default;

    /// <summary>
    /// 转换执行设置.
    /// </summary>
    /// <param name="sessionData">会话.</param>
    /// <returns>执行设置.</returns>
    public virtual ChatOptions ConvertExecutionSettings(ChatSessionPresetOld sessionData)
    {
        var settings = new ChatOptions
        {
            PresencePenalty = (float)sessionData.Parameters.GetValueOrDefault<double>(nameof(OpenAIChatParameters.FrequencyPenalty)),
            FrequencyPenalty = (float)sessionData.Parameters.GetValueOrDefault<double>(nameof(OpenAIChatParameters.PresencePenalty)),
            MaxOutputTokens = sessionData.Parameters.GetValueOrDefault<int>(nameof(OpenAIChatParameters.MaxTokens)),
            Temperature = (float)sessionData.Parameters.GetValueOrDefault<double>(nameof(OpenAIChatParameters.Temperature)),
            TopP = (float)sessionData.Parameters.GetValueOrDefault<double>(nameof(OpenAIChatParameters.TopP)),
            Seed = sessionData.Parameters.GetValueOrDefault<long>(nameof(OpenAIChatParameters.Seed)),
            ResponseFormat = sessionData.Parameters.GetValueOrDefault<string>(nameof(OpenAIChatParameters.ResponseFormat)) is "json_object" ? ChatResponseFormat.Json : ChatResponseFormat.Text,
            ModelId = sessionData.Model,
            StopSequences = sessionData.StopSequences,
        };

        if (settings.MaxOutputTokens == 0)
        {
            settings.MaxOutputTokens = null;
        }

        return settings;
    }

    /// <summary>
    /// 设置基础 URL.
    /// </summary>
    protected void SetBaseUri(string baseUrl, string? proxyUrl = null)
    {
        var url = !string.IsNullOrEmpty(proxyUrl) ? proxyUrl : baseUrl;
        if (!url.StartsWith("http"))
        {
            var isLocalHost = url.Contains("localhost") || url.Contains("127.0.0.1") || url.Contains("0.0.0.0");
            var schema = isLocalHost ? "http" : "https";
            url = $"{schema}://{url}";
        }

        if (Uri.TryCreate(url, UriKind.Absolute, out var uri))
        {
            BaseUri = uri;
        }
    }

    /// <summary>
    /// OpenAI 类型的聊天参数.
    /// </summary>
    [JsonConverter(typeof(BaseFieldParametersConverter))]
    public sealed class OpenAIChatParameters : BaseFieldParameters
    {
        /// <summary>
        /// 用于减少模型生成的重复性的参数.
        /// </summary>
        [JsonPropertyName("frequency_penalty")]
        [RangeFloatField(-2, 2)]
        public double FrequencyPenalty { get; set; } = 0d;

        /// <summary>
        /// 用于减少模型主题多样性的参数.
        /// </summary>
        [JsonPropertyName("presence_penalty")]
        [RangeFloatField(-2, 2)]
        public double PresencePenalty { get; set; } = 0d;

        /// <summary>
        /// 生成文本的最大长度.
        /// </summary>
        [JsonPropertyName("max_tokens")]
        [RangeIntField(0, int.MaxValue)]
        public int? MaxTokens { get; set; }

        /// <summary>
        /// 用于控制文本的多样性和创造性.
        /// </summary>
        [JsonPropertyName("temperature")]
        [RangeFloatField(0, 2)]
        public double Temperature { get; set; } = 1;

        /// <summary>
        /// 用于控制生成文本的顶部概率.
        /// </summary>
        [JsonPropertyName("top_p")]
        [RangeFloatField(0, 1)]
        public double TopP { get; set; } = 0.9d;

        /// <summary>
        /// 响应格式.
        /// </summary>
        [JsonPropertyName("response_format")]
        [SelectionField("message", "json_object")]
        public string? ResponseFormat { get; set; }

        /// <summary>
        /// 随机种子.
        /// </summary>
        [JsonPropertyName("seed")]
        [RangeLongField(0, int.MaxValue)]
        public long Seed { get; set; } = 0;
    }
}
