// Copyright (c) Rodel. All rights reserved.

using Microsoft.SemanticKernel;
using RodelChat.Core.Models.Chat;
using RodelChat.Core.Models.Constants;
using RodelChat.Core.Models.Providers;

namespace RodelChat.Core;

/// <summary>
/// 聊天客户端的属性和字段.
/// </summary>
public sealed partial class ChatClient
{
    private readonly List<object>? _plugins;
    private bool _disposedValue;
    private ProviderType _defaultProvider;

    private OpenAIProvider? _openAIProvider;
    private AzureOpenAIProvider? _azureOpenAIProvider;
    private ZhipuProvider? _zhipuProvider;
    private LingYiProvider? _lingYiProvider;
    private MoonshotProvider? _moonshotProvider;
    private GeminiProvider? _geminiProvider;
    private DashScopeProvider? _dashScopeProvider;
    private QianFanProvider? _qianFanProvider;
    private SparkDeskProvider? _sparkDeskProvider;
    private GroqProvider? _groqProvider;
    private MistralAIProvider? _mistralAIProvider;
    private PerplexityProvider? _perplexityProvider;

    private Kernel? _openAIKernel;
    private Kernel? _azureOpenAIKernel;
    private Kernel? _zhipuKernel;
    private Kernel? _lingYiKernel;
    private Kernel? _moonshotKernel;
    private Kernel? _geminiKernel;
    private Kernel? _dashScopeKernel;
    private Kernel? _qianFanKernel;
    private Kernel? _sparkDeskKernel;
    private Kernel? _groqKernel;
    private Kernel? _mistralAIKernel;
    private Kernel? _perplexityKernel;

    /// <summary>
    /// 会话列表.
    /// </summary>
    public List<ChatSession> Sessions { get; }
}
