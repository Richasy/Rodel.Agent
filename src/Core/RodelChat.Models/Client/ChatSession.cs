// Copyright (c) Rodel. All rights reserved.

using System.Collections.Generic;
using System.Text.Json.Serialization;
using RodelAgent.Models.Abstractions;

namespace RodelChat.Models.Client;

/// <summary>
/// 聊天会话.
/// </summary>
public sealed class ChatSession : ChatSessionPreset
{
    /// <summary>
    /// 获取或设置会话标题.
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>
    /// 预设标识.
    /// </summary>
    [JsonPropertyName("preset_id")]
    public string? PresetId { get; set; }

    /// <summary>
    /// 创建会话.
    /// </summary>
    /// <param name="id">标识符.</param>
    /// <param name="parameters">参数.</param>
    /// <returns>会话信息.</returns>
    public static ChatSession CreateSession(string id, BaseFieldParameters parameters)
    {
        return new ChatSession
        {
            Id = id,
            Parameters = parameters,
            Messages = new List<ChatMessage>(),
        };
    }

    /// <summary>
    /// 创建会话.
    /// </summary>
    /// <param name="newId">会话 ID.</param>
    /// <param name="preset">预设.</param>
    /// <returns>会话.</returns>
    public static ChatSession CreateSession(string newId, ChatSessionPreset preset)
    {
        return new ChatSession
        {
            Id = newId,
            PresetId = preset.Id,
            Name = preset.Name,
            Parameters = preset.Parameters,
            MaxRounds = preset.MaxRounds,
            UseStreamOutput = preset.UseStreamOutput,
            Provider = preset.Provider,
            Model = preset.Model,
            Messages = preset.Messages ?? new List<ChatMessage>(),
            SystemInstruction = preset.SystemInstruction,
            StopSequences = preset.StopSequences,
            FilterCharacters = preset.FilterCharacters,
            Emoji = preset.Emoji,
            Plugins = preset.Plugins,
        };
    }
}
