// Copyright (c) Rodel. All rights reserved.

using RodelTranslate.Interfaces.Client;
using RodelTranslate.Models.Client;
using RodelTranslate.Models.Constants;

namespace RodelTranslate.Core.Factories;

/// <summary>
/// 翻译提供程序工厂.
/// </summary>
public sealed partial class TranslateProviderFactory : ITranslateProviderFactory
{
    private readonly Dictionary<ProviderType, IProvider> _providers;
    private readonly Dictionary<ProviderType, Func<IProvider>> _functions;

    /// <summary>
    /// Initializes a new instance of the <see cref="TranslateProviderFactory"/> class.
    /// </summary>
    public TranslateProviderFactory(TranslateClientConfiguration configuration)
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

    /// <inheritdoc/>
    public void ResetConfiguration(TranslateClientConfiguration configuration)
        => Initialize(configuration);

    private void Initialize(TranslateClientConfiguration config)
    {
        InjectAzure(config.Azure);
        InjectBaidu(config.Baidu);
        InjectYoudao(config.Youdao);
        InjectTencent(config.Tencent);
        InjectAli(config.Ali);
        InjectVolcano(config.Volcano);
    }

    private void AddCreateMethod(ProviderType type, Func<IProvider> createFunc)
    {
        RemoveProvider(type);
        _functions[type] = createFunc;
    }

    private void RemoveProvider(ProviderType type)
    {
        if (_providers.TryGetValue(type, out var value))
        {
            value.Release();
            _providers.Remove(type);
        }
    }
}
