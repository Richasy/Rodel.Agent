// Copyright (c) Rodel. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using RodelChat.Models.Constants;

namespace RodelChat.Models.Client;

/// <summary>
/// 聊天消息.
/// </summary>
public sealed class ChatMessage
{
    /// <summary>
    /// 消息角色.
    /// </summary>
    [JsonPropertyName("role")]
    public MessageRole Role { get; set; }

    /// <summary>
    /// 消息内容.
    /// </summary>
    [JsonPropertyName("content")]
    [JsonConverter(typeof(ChatMessageContentListConverter))]
    public List<ChatMessageContent> Content { get; set; }

    /// <summary>
    /// 消息发送的时间.
    /// </summary>
    [JsonPropertyName("time")]
    [JsonConverter(typeof(DateTimeOffsetConverter))]
    public DateTimeOffset? Time { get; set; }

    /// <summary>
    /// 获取或设置客户端消息类型.
    /// </summary>
    [JsonPropertyName("client_message_type")]
    public ClientMessageType ClientMessageType { get; set; } = ClientMessageType.Normal;

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

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is ChatMessage message && EqualityComparer<DateTimeOffset?>.Default.Equals(Time, message.Time);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Time);
}

internal sealed class DateTimeOffsetConverter : JsonConverter<DateTimeOffset>
{
    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => DateTimeOffset.FromUnixTimeSeconds(reader.GetInt64());

    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
        => writer.WriteNumberValue(value.ToUnixTimeSeconds());
}

internal sealed class ChatMessageContentListConverter : JsonConverter<List<ChatMessageContent>>
{
    public override List<ChatMessageContent> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var value = reader.GetString();
            return new List<ChatMessageContent> { new ChatMessageContent { Text = value, Type = ChatContentType.Text } };
        }
        else
        {
            return JsonSerializer.Deserialize<List<ChatMessageContent>>(ref reader, options);
        }
    }

    public override void Write(
        Utf8JsonWriter writer, List<ChatMessageContent> value, JsonSerializerOptions options)
    {
        if (value.Count == 1 && value[0].Type == ChatContentType.Text && value[0].Detail == null)
        {
            writer.WriteStringValue(value[0].Text);
        }
        else
        {
            JsonSerializer.Serialize(writer, value, options);
        }
    }
}
