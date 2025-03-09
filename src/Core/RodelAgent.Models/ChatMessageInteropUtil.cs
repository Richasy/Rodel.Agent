// Copyright (c) Richasy. All rights reserved.

using Microsoft.Extensions.AI;
using RodelAgent.Models.Feature;

namespace RodelAgent.Models;

/// <summary>
/// 对话消息交互工具.
/// </summary>
public static class ChatMessageInteropUtil
{
    /// <summary>
    /// 创建交互消息对象.
    /// </summary>
    public static ChatInteropMessage ToInteropMessage(this ChatMessage message)
    {
        var role = message.Role.ToString().ToLowerInvariant();
        var text = message.Text ?? string.Empty;
        var author = message.AuthorName ?? string.Empty;
        var time = Convert.ToInt64(message.AdditionalProperties!.GetValueOrDefault("time", DateTimeOffset.Now.ToUnixTimeSeconds()));
        var id = message.AdditionalProperties!.GetValueOrDefault("id", Guid.NewGuid().ToString("N"))!.ToString();
        return new ChatInteropMessage
        {
            AgentId = author,
            Message = text,
            Role = role,
            Time = time,
            Id = id!,
        };
    }

    /// <summary>
    /// 转换为对话消息对象.
    /// </summary>
    public static ChatMessage ToChatMessage(this ChatInteropMessage message)
    {
        var role = new ChatRole(message.Role);
        var text = message.Message;
        var author = message.AgentId;
        var time = DateTimeOffset.FromUnixTimeSeconds(message.Time);
        var id = message.Id;
        return new ChatMessage
        {
            AuthorName = author,
            Text = text,
            Role = role,
            AdditionalProperties = new()
            {
                ["time"] = time.ToUnixTimeSeconds(),
                ["id"] = id,
            },
        };
    }
}
