// Copyright (c) Rodel. All rights reserved.

using System;
using System.Text.Json.Serialization;

namespace RodelChat.Models.Client;

/// <summary>
/// 模型信息.
/// </summary>
public sealed class ChatModel
{
    /// <summary>
    /// 获取或设置模型 ID.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; }

    /// <summary>
    /// 获取或设置模型的显示名称.
    /// </summary>
    [JsonPropertyName("name")]
    public string? DisplayName { get; set; }

    /// <summary>
    /// 获取或设置模型是否支持上传文件.
    /// </summary>
    [JsonPropertyName("support_file")]
    public bool IsSupportFileUpload { get; set; }

    /// <summary>
    /// 获取或设置模型是否支持工具.
    /// </summary>
    [JsonPropertyName("support_tool")]
    public bool IsSupportTool { get; set; }

    /// <summary>
    /// 获取或设置模型是否支持视觉功能.
    /// </summary>
    [JsonPropertyName("support_vision")]
    public bool IsSupportVision { get; set; }

    /// <summary>
    /// 获取或设置模型是否支持 Base64 图片.
    /// </summary>
    [JsonPropertyName("support_base64")]
    public bool? IsSupportBase64Image { get; set; }

    /// <summary>
    /// 获取或设置该模型是否为自定义模型.
    /// </summary>
    [JsonPropertyName("custom")]
    public bool IsCustomModel { get; set; }

    /// <summary>
    /// 该模型是否已弃用或不推荐使用.
    /// </summary>
    [JsonPropertyName("deprecated")]
    public bool IsDeprecated { get; set; }

    /// <summary>
    /// 模型支持的最大输出长度.
    /// </summary>
    [JsonPropertyName("max_output")]
    public long MaxOutput { get; set; }

    /// <summary>
    /// 模型的令牌数.
    /// </summary>
    [JsonPropertyName("max_tokens")]
    public long ContextLength { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is ChatModel model && Id == model.Id;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Id);

    /// <inheritdoc/>
    public override string ToString() => DisplayName ?? Id;
}
