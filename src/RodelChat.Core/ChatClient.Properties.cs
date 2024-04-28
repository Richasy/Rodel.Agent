// Copyright (c) Rodel. All rights reserved.

using OpenAI;
using RodelChat.Core.Models.Chat;
using RodelChat.Core.Models.Constants;
using RodelChat.Core.Models.Providers;
using Sdcb.DashScope;
using Sdcb.SparkDesk;
using Sdcb.WenXinQianFan;

namespace RodelChat.Core;

/// <summary>
/// 聊天客户端的属性和字段.
/// </summary>
public sealed partial class ChatClient
{
    private bool _disposedValue;
    private ProviderType _defaultProvider;

    private OpenAIProvider? _openAIProvider;
    private AzureOpenAIProvider? _azureOpenAIProvider;
    private ZhipuProvider? _zhipuProvider;
    private LingYiProvider? _lingYiProvider;
    private MoonshotProvider? _moonshotProvider;
    private DashScopeProvider? _dashScopeProvider;
    private QianFanProvider? _qianFanProvider;
    private SparkDeskProvider? _sparkDeskProvider;

    private OpenAIClient? _openAIClient;
    private OpenAIClient? _azureOpenAIClient;
    private OpenAIClient? _zhipuAIClient;
    private OpenAIClient? _lingYiAIClient;
    private OpenAIClient? _moonshotAIClient;
    private DashScopeClient? _dashScopeClient;
    private QianFanClient? _qianFanClient;
    private SparkDeskClient? _sparkDeskClient;

    /// <summary>
    /// 会话列表.
    /// </summary>
    public List<ChatSession> Sessions { get; }

    /// <summary>
    /// 工具列表.
    /// </summary>
    public List<Tool> Tools { get; }
}
