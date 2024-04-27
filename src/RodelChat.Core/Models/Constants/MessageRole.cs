// Copyright (c) Rodel. All rights reserved.

namespace RodelChat.Core.Models.Constants;

/// <summary>
/// 消息角色.
/// </summary>
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
    /// 工具消息，工具调用的消息.
    /// </summary>
    Tool,

    /// <summary>
    /// 客户端消息，客户端发送的消息.
    /// </summary>
    Client,
}
