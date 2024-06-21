// Copyright (c) Rodel. All rights reserved.

using System;
using System.Text.Json.Serialization;
using RodelAgent.Models.Abstractions;
using RodelDraw.Models.Constants;

namespace RodelDraw.Models.Client;

/// <summary>
/// 绘图会话.
/// </summary>
public sealed class DrawSession
{
    /// <summary>
    /// 会话标识符.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; }

    /// <summary>
    /// 会话参数.
    /// </summary>
    [JsonPropertyName("parameters")]
    public BaseFieldParameters? Parameters { get; set; }

    /// <summary>
    /// 服务商.
    /// </summary>
    [JsonPropertyName("provider")]
    public ProviderType Provider { get; set; }

    /// <summary>
    /// 指定的模型.
    /// </summary>
    [JsonPropertyName("model")]
    public string? Model { get; set; }

    /// <summary>
    /// 绘图请求.
    /// </summary>
    [JsonPropertyName("request")]
    public DrawRequest Request { get; set; }

    /// <summary>
    /// 绘图时间.
    /// </summary>
    [JsonPropertyName("time")]
    public DateTimeOffset? Time { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is DrawSession session && Id == session.Id;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Id);
}
