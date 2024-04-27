﻿// Copyright (c) Rodel. All rights reserved.

namespace RodelChat.Core.Models.Constants;

/// <summary>
/// 客户端消息类型.
/// </summary>
public enum ClientMessageType
{
    /// <summary>
    /// 正常消息.
    /// </summary>
    Normal,

    /// <summary>
    /// 响应消息为空.
    /// </summary>
    EmptyResponseContent,

    /// <summary>
    /// 取消生成内容.
    /// </summary>
    GenerateCancelled,

    /// <summary>
    /// 常规错误.
    /// </summary>
    GeneralFailed,
}
