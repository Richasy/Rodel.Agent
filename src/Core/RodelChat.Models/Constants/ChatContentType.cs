// Copyright (c) Rodel. All rights reserved.

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RodelChat.Models.Constants;

/// <summary>
/// 聊天消息内容类型.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ChatContentType
{
    /// <summary>
    /// 文本.
    /// </summary>
    Text,

    /// <summary>
    /// 图片.
    /// </summary>
    ImageUrl,
}

internal sealed class ChatContentTypeConverter : JsonConverter<ChatContentType>
{
    public override ChatContentType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        return value switch
        {
            "text" => ChatContentType.Text,
            "image" => ChatContentType.ImageUrl,
            _ => throw new JsonException(),
        };
    }

    public override void Write(Utf8JsonWriter writer, ChatContentType value, JsonSerializerOptions options)
    {
        var text = value switch
        {
            ChatContentType.Text => "text",
            ChatContentType.ImageUrl => "image",
            _ => throw new JsonException(),
        };

        writer.WriteStringValue(text);
    }
}
