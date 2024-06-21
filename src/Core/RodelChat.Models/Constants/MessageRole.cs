// Copyright (c) Rodel. All rights reserved.

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RodelChat.Models.Constants;

/// <summary>
/// 消息角色.
/// </summary>
[JsonConverter(typeof(MessageRoleConverter))]
public enum MessageRole
{
    /// <summary>
    /// 系统消息，系统提示词，通常不会显示在聊天记录中.
    /// </summary>
    System,

    /// <summary>
    /// 用户消息，用户输入的消息.
    /// </summary>
    User,

    /// <summary>
    /// 模型生成的消息.
    /// </summary>
    Assistant,

    /// <summary>
    /// 客户端消息，客户端发送的消息.
    /// </summary>
    Client,
}

internal sealed class MessageRoleConverter : JsonConverter<MessageRole>
{
    public override MessageRole Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        return value.ToLower() switch
        {
            "user" => MessageRole.User,
            "assistant" => MessageRole.Assistant,
            "system" => MessageRole.System,
            "client" => MessageRole.Client,
            _ => throw new JsonException(),
        };
    }

    public override void Write(Utf8JsonWriter writer, MessageRole value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToString().ToLower());
}
