// Copyright (c) Richasy. All rights reserved.

using System.Text.Json.Serialization;

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
            MaxRounds = preset.MaxRounds,
            UseStreamOutput = preset.UseStreamOutput,
            Provider = preset.Provider,
            Model = preset.Model,
            History = preset.History ?? [],
            SystemInstruction = preset.SystemInstruction,
            FilterCharacters = preset.FilterCharacters,
            Emoji = preset.Emoji,
            Plugins = preset.Plugins,
        };
    }
}
