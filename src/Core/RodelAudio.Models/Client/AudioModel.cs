// Copyright (c) Rodel. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RodelAudio.Models.Client;

/// <summary>
/// 音频模型.
/// </summary>
public sealed class AudioModel
{
    /// <summary>
    /// 标识符.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; }

    /// <summary>
    /// 显示名称.
    /// </summary>
    [JsonPropertyName("name")]
    public string DisplayName { get; set; }

    /// <summary>
    /// 是否为自定义模型.
    /// </summary>
    [JsonPropertyName("custom")]
    public bool IsCustomModel { get; set; }

    /// <summary>
    /// 声音列表.
    /// </summary>
    [JsonPropertyName("voices")]
    public IList<AudioVoice> Voices { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is AudioModel model && Id == model.Id;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Id);

    /// <inheritdoc/>
    public override string ToString() => DisplayName;
}
