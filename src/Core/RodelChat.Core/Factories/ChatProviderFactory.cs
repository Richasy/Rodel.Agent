// Copyright (c) Rodel. All rights reserved.

using RodelChat.Interfaces.Client;
using RodelChat.Models.Client;
using RodelChat.Models.Constants;

namespace RodelChat.Core.Factories;

/// <summary>
/// 聊天服务商工厂.
/// </summary>
public sealed partial class ChatProviderFactory : IChatProviderFactory
{
    private readonly Dictionary<ProviderType, IProvider> _providers;
    private readonly Dictionary<ProviderType, Func<IProvider>> _functions;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatProviderFactory"/> class.
    /// </summary>
    public ChatProviderFactory(ChatClientConfiguration configuration)
    {
        _providers = new Dictionary<ProviderType, IProvider>();
        _functions = new Dictionary<ProviderType, Func<IProvider>>();
        Initialize(configuration);
    }

    /// <inheritdoc/>
    public IProvider GetOrCreateProvider(ProviderType type)
    {
        var providerExist = _providers.TryGetValue(type, out var provider);
        if (!providerExist && _functions.TryGetValue(type, out var createFunc))
        {
            provider = createFunc();
            _providers.Add(type, provider);
        }

        return provider ?? throw new KeyNotFoundException("Provider not found and also not provide create method.");
    }

    /// <inheritdoc/>
    public void Clear()
    {
        var existTypes = _providers.Keys.ToList();
        foreach (var type in existTypes)
        {
            RemoveProvider(type);
        }
    }

    private void Initialize(ChatClientConfiguration config)
    {
        InjectOpenAI(config.OpenAI);
        InjectAzureOpenAI(config.AzureOpenAI);
        InjectZhiPu(config.ZhiPu);
        InjectLingYi(config.LingYi);
        InjectMoonshot(config.Moonshot);
        InjectDashScope(config.DashScope);
        InjectQianFan(config.QianFan);
        InjectSparkDesk(config.SparkDesk);
        InjectGemini(config.Gemini);
        InjectGroq(config.Groq);
        InjectMistralAI(config.MistralAI);
        InjectPerplexity(config.Perplexity);
        InjectTogetherAI(config.TogetherAI);
        InjectOpenRouter(config.OpenRouter);
        InjectAnthropic(config.Anthropic);
        InjectOllama(config.Ollama);
    }

    private void RemoveProvider(ProviderType type)
    {
        if (_providers.TryGetValue(type, out var value))
        {
            value.Release();
            _providers.Remove(type);
        }
    }

    private void AddCreateMethod(ProviderType type, Func<IProvider> createFunc)
    {
        RemoveProvider(type);
        _functions[type] = createFunc;
    }
}
