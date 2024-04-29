// Copyright (c) Rodel. All rights reserved.

namespace RodelChat.Core.Models.Chat;

/// <summary>
/// 聊天响应.
/// </summary>
public sealed class ChatResponse
{
    /// <summary>
    /// 消息.
    /// </summary>
    public ChatMessage Message { get; set; }
}
