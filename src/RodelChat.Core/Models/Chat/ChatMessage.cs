// Copyright (c) Rodel. All rights reserved.

using System.Text.Encodings.Web;
using System.Text.Json;
using RodelChat.Core.Models.Constants;

namespace RodelChat.Core.Models.Chat;

/// <summary>
/// 聊天消息.
/// </summary>
public sealed class ChatMessage
{
    /// <summary>
    /// 消息角色.
    /// </summary>
    public MessageRole Role { get; set; }

    /// <summary>
    /// 消息内容.
    /// </summary>
    public string Content { get; set; }

    /// <summary>
    /// 获取或设置消息名称.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 消息发送的时间.
    /// </summary>
    public DateTimeOffset Time { get; set; }

    /// <summary>
    /// 获取或设置客户端消息类型.
    /// </summary>
    public ClientMessageType ClientMessageType { get; set; } = ClientMessageType.Normal;

    /// <summary>
    /// 工具调用信息.
    /// </summary>
    public string ToolCalls { get; set; }

    /// <summary>
    /// 工具 ID.
    /// </summary>
    public string ToolId { get; set; }

    /// <summary>
    /// 创建系统消息.
    /// </summary>
    /// <param name="content">系统消息内容.</param>
    /// <returns><see cref="ChatMessage"/>.</returns>
    public static ChatMessage CreateSystemMessage(string content)
    {
        return new ChatMessage
        {
            Role = MessageRole.System,
            Content = content,
        };
    }

    /// <summary>
    /// 创建用户消息.
    /// </summary>
    /// <param name="content">用户消息.</param>
    /// <returns><see cref="ChatMessage"/>.</returns>
    public static ChatMessage CreateUserMessage(string content)
    {
        return new ChatMessage
        {
            Role = MessageRole.User,
            Content = content,
        };
    }

    /// <summary>
    /// 创建助手消息.
    /// </summary>
    /// <param name="content">助手消息.</param>
    /// <returns><see cref="ChatMessage"/>.</returns>
    public static ChatMessage CreateAssistantMessage(string content, List<OpenAI.Tool>? toolCalls = default)
    {
        var msg = new ChatMessage
        {
            Role = MessageRole.Assistant,
            Content = content,
        };

        if (toolCalls != null && toolCalls.Count > 0)
        {
            msg.ToolCalls = JsonSerializer.Serialize(toolCalls, new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            });
        }

        return msg;
    }

    /// <summary>
    /// 创建函数消息.
    /// </summary>
    /// <param name="name">函数名称.</param>
    /// <param name="content">函数消息.</param>
    /// <returns><see cref="ChatMessage"/>.</returns>
    public static ChatMessage CreateToolMessage(string name, string content, string toolId)
    {
        return new ChatMessage
        {
            Role = MessageRole.Tool,
            Name = name,
            Content = content,
            ToolId = toolId,
        };
    }

    /// <summary>
    /// 创建客户端消息.
    /// </summary>
    /// <returns><see cref="ChatMessage"/>.</returns>
    public static ChatMessage CreateClientMessage(ClientMessageType type, string content)
    {
        return new ChatMessage
        {
            Role = MessageRole.Client,
            Content = content,
            ClientMessageType = type,
        };
    }
}
