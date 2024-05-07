// Copyright (c) Rodel. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using RodelChat.Models.Constants;

namespace RodelChat.Models.Chat;

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
    public List<ChatMessageContent> Content { get; set; }

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
            Content = new List<ChatMessageContent> { ChatMessageContent.CreateTextContent(content) },
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
            Content = new List<ChatMessageContent> { ChatMessageContent.CreateTextContent(content) },
        };
    }

    /// <summary>
    /// 创建用户消息.
    /// </summary>
    /// <param name="content">文本内容.</param>
    /// <param name="imageDatas">图像数据.</param>
    /// <returns><see cref="ChatMessage"/>.</returns>
    public static ChatMessage CreateUserMessage(string content, params string[] imageDatas)
    {
        var message = new ChatMessage
        {
            Role = MessageRole.User,
            Content = new List<ChatMessageContent> { ChatMessageContent.CreateTextContent(content) },
        };

        foreach (var imageData in imageDatas)
        {
            message.Content.Add(ChatMessageContent.CreateImageContent(imageData));
        }

        return message;
    }

    /// <summary>
    /// 创建助手消息.
    /// </summary>
    /// <param name="content">助手消息.</param>
    /// <returns><see cref="ChatMessage"/>.</returns>
    public static ChatMessage CreateAssistantMessage(string content)
    {
        var msg = new ChatMessage
        {
            Role = MessageRole.Assistant,
            Content = new List<ChatMessageContent> { ChatMessageContent.CreateTextContent(content) },
        };

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
            Content = new List<ChatMessageContent> { ChatMessageContent.CreateTextContent(content) },
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
            Content = new List<ChatMessageContent> { ChatMessageContent.CreateTextContent(content) },
            ClientMessageType = type,
        };
    }

    /// <summary>
    /// 获取第一个文本内容.
    /// </summary>
    /// <returns>内容.</returns>
    public string GetFirstTextContent()
        => Content.FirstOrDefault()?.Text ?? string.Empty;
}
