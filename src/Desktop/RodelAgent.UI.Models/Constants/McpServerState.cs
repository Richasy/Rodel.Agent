// Copyright (c) Richasy. All rights reserved.

namespace RodelAgent.UI.Models.Constants;

/// <summary>
/// MCP 服务器状态.
/// </summary>
public enum McpServerState
{
    /// <summary>
    /// 正在连接.
    /// </summary>
    Connecting,

    /// <summary>
    /// 正在运行.
    /// </summary>
    Running,

    /// <summary>
    /// 停止状态，未运行.
    /// </summary>
    Stopped,

    /// <summary>
    /// 启动过程中发生错误.
    /// </summary>
    Error,
}
