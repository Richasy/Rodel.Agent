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
        var author = message.AdditionalProperties?.GetValueOrDefault("agentId", string.Empty)?.ToString() ?? string.Empty;
        var time = Convert.ToInt64(message.AdditionalProperties?.GetValueOrDefault("time", DateTimeOffset.Now.ToUnixTimeSeconds()));
        var id = message.AdditionalProperties?.GetValueOrDefault("id", Guid.NewGuid().ToString("N"))?.ToString();
        var toolClientId = message.AdditionalProperties?.GetValueOrDefault("clientId", string.Empty)?.ToString() ?? "Unknown server";
        var toolMethod = message.AdditionalProperties?.GetValueOrDefault("method", string.Empty)?.ToString() ?? "Unknown method";
        var msg = new ChatInteropMessage
        {
            AgentId = author,
            Role = role,
            Time = time,
            Id = id ?? Guid.NewGuid().ToString("N"),
        };

        if (role == "tool")
        {
            msg.ToolClientId = toolClientId;
            msg.ToolMethod = toolMethod;
            msg.ToolData = text;
        }
        else
        {
            msg.Message = text;
        }

        return msg;
    }

    /// <summary>
    /// 转换为对话消息对象.
    /// </summary>
    public static ChatMessage ToChatMessage(this ChatInteropMessage message, Func<string, string>? getAuthorName = null)
    {
        var role = new ChatRole(message.Role);
        var author = string.IsNullOrEmpty(message.AgentId) ? null : getAuthorName!(message.AgentId);
        var time = DateTimeOffset.FromUnixTimeSeconds(message.Time);
        var id = message.Id;
        var chatMsg = new ChatMessage
        {
            AuthorName = author,
            Role = role,
            AdditionalProperties = new()
            {
                ["time"] = time.ToUnixTimeSeconds(),
                ["id"] = id,
            },
        };

        if (role == ChatRole.Tool)
        {
            chatMsg.Contents = [new TextContent(message.ToolData)];
            chatMsg.AdditionalProperties["clientId"] = message.ToolClientId;
            chatMsg.AdditionalProperties["method"] = message.ToolMethod;
        }
        else
        {
            chatMsg.Contents = [new TextContent(message.Message)];
            if (!string.IsNullOrEmpty(message.AgentId))
            {
                chatMsg.AdditionalProperties["agentId"] = message.AgentId;
            }
        }

        return chatMsg;
    }
}
