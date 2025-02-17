// Copyright (c) Richasy. All rights reserved.

using System.Text.Json.Serialization;

namespace RodelChat.Models.Client;

/// <summary>
/// 聊天群组预设.
/// </summary>
public class ChatGroupPreset
{
    /// <summary>
    /// 会话标识符.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; }

    /// <summary>
    /// 预设名称.
    /// </summary>
    [JsonPropertyName("name")]
    [JsonRequired]
    public string Name { get; set; } = null!;

    /// <summary>
    /// 群组成员.
    /// </summary>
    [JsonPropertyName("agents")]
    public IList<string>? Agents { get; set; }

    /// <summary>
    /// 表情头像.
    /// </summary>
    [JsonPropertyName("emoji")]
    public string? Emoji { get; set; }

    /// <summary>
    /// 最大会话轮次.
    /// </summary>
    [JsonPropertyName("max_rounds")]
    public int MaxRounds { get; set; }

    /// <summary>
    /// 终结文本.
    /// </summary>
    [JsonPropertyName("terminate_text")]
    public IList<string>? TerminateText { get; set; }

    /// <summary>
    /// 克隆当前实例.
    /// </summary>
    /// <returns><see cref="ChatGroupPreset"/>.</returns>
    public ChatGroupPreset Clone()
    {
        return new ChatGroupPreset
        {
            Id = Id,
            Name = Name,
            Agents = Agents,
            Emoji = Emoji,
            MaxRounds = MaxRounds,
            TerminateText = TerminateText,
        };
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is ChatGroupPreset preset && Id == preset.Id;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Id);
}
