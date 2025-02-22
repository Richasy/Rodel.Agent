// Copyright (c) Richasy. All rights reserved.

using Richasy.AgentKernel;
using System.Text.Json.Serialization;

namespace RodelAgent.Models.Common;

/// <summary>
/// 音频记录.
/// </summary>
public sealed class AudioRecord
{
    /// <summary>
    /// 会话标识符.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; }

    /// <summary>
    /// 服务商.
    /// </summary>
    [JsonPropertyName("provider")]
    public AudioProviderType Provider { get; set; }

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
