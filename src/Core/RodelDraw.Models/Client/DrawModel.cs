// Copyright (c) Rodel. All rights reserved.

using System;
using System.Text.Json.Serialization;

namespace RodelDraw.Models.Client;

/// <summary>
/// 绘图模型.
/// </summary>
public sealed class DrawModel
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
    /// 获取或设置该模型是否为自定义模型.
    /// </summary>
    [JsonPropertyName("custom")]
    public bool IsCustomModel { get; set; }

    /// <summary>
    /// 获取或设置该模型是否支持负提示词.
    /// </summary>
    [JsonPropertyName("negative_support")]
    public bool IsNegativeSupport { get; set; }

    /// <summary>
    /// 支持的尺寸.
    /// </summary>
    [JsonPropertyName("sizes")]
    public string[]? SupportSizes { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is DrawModel model && Id == model.Id;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Id);

    /// <inheritdoc/>
    public override string ToString() => DisplayName;
}
