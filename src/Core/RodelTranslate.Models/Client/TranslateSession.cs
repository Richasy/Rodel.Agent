// Copyright (c) Rodel. All rights reserved.

using System;
using System.Text.Json.Serialization;
using RodelAgent.Models.Abstractions;
using RodelTranslate.Models.Constants;

namespace RodelTranslate.Models.Client;

/// <summary>
/// 翻译会话.
/// </summary>
public sealed class TranslateSession
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
    /// 源语言.
    /// </summary>
    [JsonPropertyName("source")]
    public Language? SourceLanguage { get; set; }

    /// <summary>
    /// 目标语言.
    /// </summary>
    [JsonPropertyName("target")]
    public Language? TargetLanguage { get; set; }

    /// <summary>
    /// 输入文本.
    /// </summary>
    [JsonPropertyName("input")]
    public string? InputText { get; set; }

    /// <summary>
    /// 输出的翻译文本.
    /// </summary>
    [JsonPropertyName("output")]
    public string? OutputText { get; set; }

    /// <summary>
    /// 时间.
    /// </summary>
    [JsonPropertyName("time")]
    public DateTimeOffset? Time { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is TranslateSession session && base.Equals(obj) && Id == session.Id;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), Id);
}
