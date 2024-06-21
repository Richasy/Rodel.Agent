// Copyright (c) Rodel. All rights reserved.

using Microsoft.Extensions.Logging;
using RodelChat.Interfaces.Client;
using RodelChat.Models.Client;

namespace RodelChat.Core;

/// <summary>
/// 聊天客户端的属性和字段.
/// </summary>
public sealed partial class ChatClient
{
    private readonly IChatProviderFactory _providerFactory;
    private readonly IChatParametersFactory _parameterFactory;
    private readonly ILogger<ChatClient> _logger;
    private bool _disposedValue;
    private string _tempPluginPath;

    /// <summary>
    /// 会话列表.
    /// </summary>
    public List<ChatSession> Sessions { get; }
}
