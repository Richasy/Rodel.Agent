// Copyright (c) Richasy. All rights reserved.

using RodelDraw.Interfaces.Client;
using RodelDraw.Models.Client;
using RodelDraw.Models.Constants;

namespace RodelDraw.Core.Factories;

/// <summary>
/// 创建绘图服务商的工厂.
/// </summary>
public sealed partial class DrawProviderFactory : IDrawProviderFactory
{
    private readonly Dictionary<ProviderType, IProvider> _providers;
    private readonly Dictionary<ProviderType, Func<IProvider>> _functions;

    /// <summary>
    /// Initializes a new instance of the <see cref="DrawProviderFactory"/> class.
    /// </summary>
    public DrawProviderFactory(
        DrawClientConfiguration configuration)
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
    public void ResetConfiguration(DrawClientConfiguration configuration)
        => Initialize(configuration);

    private void Initialize(DrawClientConfiguration config)
    {
        InjectOpenAI(config.OpenAI);
        InjectAzureOpenAI(config.AzureOpenAI);
        InjectQianFan(config.QianFan);
        InjectSparkDesk(config.SparkDesk);
        InjectHunYuan(config.HunYuan);
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
