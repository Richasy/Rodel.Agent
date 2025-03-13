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
        var author = message.AdditionalProperties!.GetValueOrDefault("agentId", string.Empty)!.ToString() ?? string.Empty;
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
    public static ChatMessage ToChatMessage(this ChatInteropMessage message, Func<string, string>? getAuthorName = null)
    {
        var role = new ChatRole(message.Role);
        var text = message.Message;
        var author = string.IsNullOrEmpty(message.AgentId) ? null : getAuthorName!(message.AgentId);
        var time = DateTimeOffset.FromUnixTimeSeconds(message.Time);
        var id = message.Id;
        return new ChatMessage
        {
            AuthorName = author,
            Contents = [new TextContent(text)],
            Role = role,
            AdditionalProperties = new()
            {
                ["agentId"] = message.AgentId,
                ["time"] = time.ToUnixTimeSeconds(),
                ["id"] = id,
            },
        };
    }
}
