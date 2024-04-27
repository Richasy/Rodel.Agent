// Copyright (c) Rodel. All rights reserved.

using OpenAI;

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

    /// <summary>
    /// 调用的工具.
    /// </summary>
    public List<Tool>? Tools { get; set; }
}
