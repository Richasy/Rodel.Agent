// Copyright (c) Rodel. All rights reserved.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RodelChat.Models.Client;

/// <summary>
/// 聊天群组.
/// </summary>
public sealed class ChatGroup
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
}
