// Copyright (c) Richasy. All rights reserved.

using System.Text.Json.Serialization;

namespace RodelAgent.Models.Feature;

/// <summary>
/// 聊天群组.
/// </summary>
public sealed class ChatGroup
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
    public List<string>? Agents { get; set; }

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
    [JsonPropertyName("terminate_sequence")]
    public List<string>? TerminateSequence { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is ChatGroup group && Id == group.Id;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Id);
}
