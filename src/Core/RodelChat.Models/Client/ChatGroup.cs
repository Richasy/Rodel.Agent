// Copyright (c) Richasy. All rights reserved.

using Microsoft.Extensions.AI;
using System.Text.Json.Serialization;

namespace RodelChat.Models.Client;

/// <summary>
/// 聊天群组.
/// </summary>
public sealed class ChatGroup : ChatGroupPreset
{
    /// <summary>
    /// 群组标题.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; }

    /// <summary>
    /// 群组预设标识.
    /// </summary>
    [JsonPropertyName("preset_id")]
    public string PresetId { get; set; }

    /// <summary>
    /// 获取或设置历史记录.
    /// </summary>
    [JsonPropertyName("messages")]
    public List<ChatMessage>? Messages { get; set; }

    /// <summary>
    /// 创建会话.
    /// </summary>
    /// <param name="id">标识符.</param>
    /// <param name="preset">预设.</param>
    /// <returns>群组会话.</returns>
    public static ChatGroup CreateGroup(string id, ChatGroupPreset preset)
    {
        return new ChatGroup
        {
            Id = id,
            PresetId = preset.Id,
            Messages = [],
            Agents = preset.Agents,
            Emoji = preset.Emoji,
            Name = preset.Name,
            MaxRounds = preset.MaxRounds,
            TerminateText = preset.TerminateText,
        };
    }
}
