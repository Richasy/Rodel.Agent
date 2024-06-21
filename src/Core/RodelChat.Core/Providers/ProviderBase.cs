// Copyright (c) Rodel. All rights reserved.

using System.Text.Json.Serialization;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Services;
using RodelAgent.Models.Abstractions;
using RodelChat.Models.Client;
using RodelChat.Models.Constants;

namespace RodelChat.Core.Providers;

/// <summary>
/// 服务商基类.
/// </summary>
public abstract class ProviderBase
{
    private Kernel? _kernel;

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
    /// 工具调用事件.
    /// </summary>
    public event EventHandler<ToolInvokingEventArgs>? ToolInvoking;

    /// <summary>
    /// 工具调用完成事件.
    /// </summary>
    public event EventHandler<ToolInvokedEventArgs>? ToolInvoked;

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
    protected Kernel? Kernel
    {
        get => _kernel;
        set
        {
            _kernel = value;
            RegisterKernelFilters();
        }
    }

    /// <summary>
    /// 基础 URL.
    /// </summary>
    protected Uri? BaseUri { get; private set; }

    /// <summary>
    /// 服务商类型.
    /// </summary>
    protected ProviderType Type { get; init; }

    /// <summary>
    /// 获取当前模型 ID.
    /// </summary>
    /// <returns>模型 ID.</returns>
    public string? GetCurrentModelId()
        => GetKernelModelId(Kernel);

    /// <summary>
    /// 获取当前内核.
    /// </summary>
    /// <returns>当前内核.</returns>
    public Kernel? GetCurrentKernel()
        => Kernel;

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

        return models.Distinct().OrderByDescending(p => p.IsCustomModel).ToList();
    }

    /// <summary>
    /// 释放资源.
    /// </summary>
    public void Release()
        => Kernel = default;

    /// <summary>
    /// 转换执行设置.
    /// </summary>
    /// <param name="sessionData">会话.</param>
    /// <returns>执行设置.</returns>
    public virtual PromptExecutionSettings ConvertExecutionSettings(ChatSession sessionData)
        => new OpenAIPromptExecutionSettings
        {
            PresencePenalty = sessionData.Parameters.GetValueOrDefault<double>(nameof(OpenAIChatParameters.FrequencyPenalty)),
            FrequencyPenalty = sessionData.Parameters.GetValueOrDefault<double>(nameof(OpenAIChatParameters.PresencePenalty)),
            MaxTokens = sessionData.Parameters.GetValueOrDefault<int>(nameof(OpenAIChatParameters.MaxTokens)),
            Temperature = sessionData.Parameters.GetValueOrDefault<double>(nameof(OpenAIChatParameters.Temperature)),
            TopP = sessionData.Parameters.GetValueOrDefault<double>(nameof(OpenAIChatParameters.TopP)),
            Seed = sessionData.Parameters.GetValueOrDefault<long>(nameof(OpenAIChatParameters.Seed)),
            ResponseFormat = sessionData.Parameters.GetValueOrDefault<string>(nameof(OpenAIChatParameters.ResponseFormat)),
            ChatSystemPrompt = sessionData.SystemInstruction,
            ModelId = sessionData.Model,
            StopSequences = sessionData.StopSequences,
            ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
        };

    internal static string GetKernelModelId(Kernel? kernel)
    {
        if (kernel == null)
        {
            return null;
        }

        var chatService = kernel.GetRequiredService<IChatCompletionService>();
        return chatService?.Attributes?.GetValueOrDefault(AIServiceExtensions.ModelIdKey)?.ToString() ?? default;
    }

    internal void RaiseToolInvoking(ToolInvokingEventArgs args)
        => ToolInvoking?.Invoke(this, args);

    internal void RaiseToolInvoked(ToolInvokedEventArgs args)
        => ToolInvoked?.Invoke(this, args);

    /// <summary>
    /// 是否需要重新创建内核.
    /// </summary>
    /// <returns>是否需要.</returns>
    protected bool ShouldRecreateKernel(string modelId)
        => Kernel == null || GetCurrentModelId() != modelId;

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
    /// 注册内核过滤器.
    /// </summary>
    protected virtual void RegisterKernelFilters()
    {
        if (Kernel == null)
        {
            return;
        }

        Kernel.FunctionInvocationFilters.Clear();
        Kernel.FunctionInvocationFilters.Add(new ProviderFunctionInvocationFilter(this));
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

    internal class ProviderFunctionInvocationFilter(ProviderBase provider) : IFunctionInvocationFilter
    {
        public async Task OnFunctionInvocationAsync(FunctionInvocationContext context, Func<FunctionInvocationContext, Task> next)
        {
            var modelId = GetKernelModelId(provider.Kernel);
            var invokingArgs = new ToolInvokingEventArgs(context, modelId);
            provider.RaiseToolInvoking(invokingArgs);
            await next(context);
            var invokedArgs = new ToolInvokedEventArgs(context, modelId);
            provider.RaiseToolInvoked(invokedArgs);
        }
    }
}
