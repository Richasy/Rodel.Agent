// Copyright (c) Rodel. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using RodelAudio.Models.Constants;

namespace RodelAudio.Models.Client;

/// <summary>
/// 声音.
/// </summary>
public sealed class AudioVoice
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AudioVoice"/> class.
    /// </summary>
    public AudioVoice()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AudioVoice"/> class.
    /// </summary>
    public AudioVoice(
        string id,
        string name,
        VoiceGender gender,
        params string[] languages)
    {
        Id = id;
        DisplayName = name;
        Gender = gender;
        Languages = languages.ToList();
    }

    /// <summary>
    /// 标识符.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; }

    /// <summary>
    /// 声音显示名称.
    /// </summary>
    [JsonPropertyName("name")]
    public string DisplayName { get; set; }

    /// <summary>
    /// 声音性别.
    /// </summary>
    [JsonPropertyName("gender")]
    public VoiceGender Gender { get; set; }

    /// <summary>
    /// 支持的语言列表.
    /// </summary>
    [JsonPropertyName("languages")]
    public IList<string> Languages { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is AudioVoice voice && Id == voice.Id;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Id);
}
