// Copyright (c) Rodel. All rights reserved.

using System.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel.TextToAudio;
using RodelAudio.Core.Providers;
using RodelAudio.Interfaces.Client;
using RodelAudio.Models.Client;
using RodelAudio.Models.Constants;

namespace RodelAudio.Core;

/// <summary>
/// 音频客户端.
/// </summary>
public sealed partial class AudioClient : IAudioClient
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AudioClient"/> class.
    /// </summary>
    public AudioClient(
        IAudioProviderFactory providerFactory,
        IAudioParametersFactory parameterFactory,
        ILogger<AudioClient> logger)
    {
        _logger = logger;
        _providerFactory = providerFactory;
        _parameterFactory = parameterFactory;
    }

    /// <inheritdoc/>
    public async Task<ReadOnlyMemory<byte>> TextToSpeechAsync(AudioSession session, CancellationToken cancellationToken = default)
    {
        var kernel = FindKernelProvider(session.Provider, session.Model)
            ?? throw new ArgumentException("Kernel not found.");

        try
        {
            session.Parameters ??= GetAudioParameters(session.Provider);
            var settings = GetExecutionSettings(session);
            var audioService = kernel.GetRequiredService<ITextToAudioService>();
            var audioContent = await audioService.GetAudioContentAsync(session.Text, settings, cancellationToken: cancellationToken).ConfigureAwait(false);
            session.Time = DateTimeOffset.Now;
            return audioContent.Data ?? default;
        }
        catch (TaskCanceledException)
        {
            _logger.LogInformation("Image generation task was canceled.");
        }
        catch (Exception)
        {
            throw;
        }

        return default;
    }

    /// <inheritdoc/>
    public List<AudioModel> GetModels(ProviderType type)
        => GetProvider(type).GetModelList();

    /// <inheritdoc/>
    public List<AudioModel> GetPredefinedModels(ProviderType type)
    {
        var preType = typeof(PredefinedModels);
        var properties = preType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
        foreach (var prop in properties)
        {
            if (prop.Name.StartsWith(type.ToString()))
            {
                return prop.GetValue(default) as List<AudioModel>
                    ?? throw new ArgumentException("Predefined models not found.");
            }
        }

        return new List<AudioModel>();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    public async Task InitialAzureSpeechAsync()
    {
        var provider = _providerFactory.GetOrCreateProvider(ProviderType.AzureSpeech);
        if (provider.GetModelList().Count == 0)
        {
            var isSuccess = await (provider as AzureSpeechProvider).InitializeSpeechVoicesAsync();
            if (!isSuccess)
            {
                throw new InvalidOperationException("Initialize Azure speech voices failed.");
            }
        }
    }
}
