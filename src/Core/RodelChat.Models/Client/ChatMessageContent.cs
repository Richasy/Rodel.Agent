// Copyright (c) Rodel. All rights reserved.

using System.Text.Json.Serialization;
using RodelChat.Models.Constants;

namespace RodelChat.Models.Client;

/// <summary>
/// 聊天消息内容.
/// </summary>
public sealed class ChatMessageContent
{
    /// <summary>
    /// 获取或设置消息内容类型.
    /// </summary>
    [JsonPropertyName("type")]
    public ChatContentType Type { get; set; }

    /// <summary>
    /// 获取或设置消息内容.
    /// </summary>
    [JsonPropertyName("text")]
    public string Text { get; set; }

    /// <summary>
    /// 图像解析参数.
    /// </summary>
    [JsonPropertyName("detail")]
    public ImageDetailType? Detail { get; set; }

    /// <summary>
    /// 创建文本消息内容.
    /// </summary>
    /// <param name="text">文本消息.</param>
    /// <returns><see cref="ChatMessageContent"/>.</returns>
    public static ChatMessageContent CreateTextContent(string text)
    {
        return new ChatMessageContent
        {
            Type = ChatContentType.Text,
            Text = text,
        };
    }

    /// <summary>
    /// 创建图片消息内容.
    /// </summary>
    /// <param name="content">图片链接或者 BASE64 字符串.</param>
    /// <returns><see cref="ChatMessageContent"/>.</returns>
    public static ChatMessageContent CreateImageContent(string content, ImageDetailType detailType = ImageDetailType.Auto)
    {
        return new ChatMessageContent
        {
            Type = ChatContentType.ImageUrl,
            Text = content,
            Detail = detailType,
        };
    }
}
