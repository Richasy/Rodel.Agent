// Copyright (c) Richasy. All rights reserved.

using RodelAudio.Interfaces.Client;
using RodelAudio.Models.Client;
using RodelAudio.Models.Constants;

namespace RodelAudio.Core.Factories;

/// <summary>
/// 音频服务商工厂.
/// </summary>
public sealed partial class AudioProviderFactory : IAudioProviderFactory
{
    private readonly Dictionary<ProviderType, IProvider> _providers;
    private readonly Dictionary<ProviderType, Func<IProvider>> _functions;

    /// <summary>
    /// Initializes a new instance of the <see cref="AudioProviderFactory"/> class.
    /// </summary>
    public AudioProviderFactory(
        AudioClientConfiguration configuration)
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
    public void ResetConfiguration(AudioClientConfiguration configuration)
        => Initialize(configuration);

    private void Initialize(AudioClientConfiguration config)
    {
        InjectOpenAI(config.OpenAI);
        InjectAzureOpenAI(config.AzureOpenAI);
        InjectAzureSpeech(config.AzureSpeech);
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
