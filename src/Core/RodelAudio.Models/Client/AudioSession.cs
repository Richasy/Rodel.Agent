// Copyright (c) Rodel. All rights reserved.

using System;
using System.Text.Json.Serialization;
using RodelAgent.Models.Abstractions;
using RodelAudio.Models.Constants;

namespace RodelAudio.Models.Client;

/// <summary>
/// 音频会话.
/// </summary>
public sealed class AudioSession
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
    /// 指定的声音.
    /// </summary>
    [JsonPropertyName("voice")]
    public string? Voice { get; set; }

    /// <summary>
    /// 文本.
    /// </summary>
    [JsonPropertyName("text")]
    public string? Text { get; set; }

    /// <summary>
    /// 语速.
    /// </summary>
    [JsonPropertyName("speed")]
    public double? Speed { get; set; }

    /// <summary>
    /// 生成时间.
    /// </summary>
    [JsonPropertyName("time")]
    public DateTimeOffset? Time { get; set; }
}
