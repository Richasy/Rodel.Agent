// Copyright (c) Rodel. All rights reserved.

using RodelChat.Interfaces.Client;
using RodelChat.Models.Chat;

namespace RodelChat.Core;

/// <summary>
/// 聊天客户端的属性和字段.
/// </summary>
public sealed partial class ChatClient
{
    private readonly List<object>? _plugins;
    private readonly IChatProviderFactory _providerFactory;
    private bool _disposedValue;

    /// <summary>
    /// 会话列表.
    /// </summary>
    public List<ChatSession> Sessions { get; }
}
