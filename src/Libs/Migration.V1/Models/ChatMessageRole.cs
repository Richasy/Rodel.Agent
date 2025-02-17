// Copyright (c) Richasy. All rights reserved.

namespace Migration.V1.Models;

/// <summary>
/// 聊天消息角色.
/// </summary>
internal enum ChatMessageRole
{
    /// <summary>
    /// 系统提示词.
    /// </summary>
    System,

    /// <summary>
    /// 助理.
    /// </summary>
    Assistant,

    /// <summary>
    /// 用户.
    /// </summary>
    User,

    /// <summary>
    /// 工具.
    /// </summary>
    Tool,
}
