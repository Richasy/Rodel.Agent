// Copyright (c) Rodel. All rights reserved.

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Services;
using Microsoft.SemanticKernel.TextToAudio;
using RodelAudio.Models.Client;
using RodelAudio.Models.Constants;

namespace RodelAudio.Core.Providers;

/// <summary>
/// 服务商基类.
/// </summary>
public abstract class ProviderBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProviderBase"/> class.
    /// </summary>
    protected ProviderBase(string key, List<AudioModel>? customModels = null)
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
    public List<AudioModel>? CustomModels { get; set; }

    /// <summary>
    /// 服务端模型列表.
    /// </summary>
    public List<AudioModel>? ServerModels { get; set; }

    /// <summary>
    /// 访问密钥.
    /// </summary>
    protected string? AccessKey { get; set; }

    /// <summary>
    /// 内核.
    /// </summary>
    protected Kernel? Kernel { get; set; }

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
    public AudioModel GetModelOrDefault(string modelId)
        => CustomModels?.FirstOrDefault(m => m.Id == modelId)
            ?? ServerModels?.FirstOrDefault(m => m.Id == modelId)
            ?? default;

    /// <summary>
    /// 获取模型列表.
    /// </summary>
    /// <returns>模型列表.</returns>
    public List<AudioModel> GetModelList()
    {
        var models = new List<AudioModel>();
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

    internal static string GetKernelModelId(Kernel? kernel)
    {
        if (kernel == null)
        {
            return null;
        }

        var audoService = kernel.GetRequiredService<ITextToAudioService>();
        return audoService?.Attributes?.GetValueOrDefault(AIServiceExtensions.ModelIdKey)?.ToString() ?? default;
    }

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
}

