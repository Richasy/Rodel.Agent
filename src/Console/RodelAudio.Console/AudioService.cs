// Copyright (c) Rodel. All rights reserved.

using Microsoft.Extensions.Hosting;
using RodelAgent.Interfaces;
using RodelAgent.Statics;
using RodelAudio.Interfaces.Client;
using RodelAudio.Models.Client;
using RodelAudio.Models.Constants;
using Spectre.Console;
using static System.Console;

namespace RodelAudio.Console;

/// <summary>
/// 音频服务.
/// </summary>
public sealed class AudioService : IHostedService
{
    private readonly IAudioClient _client;
    private readonly IStringResourceToolkit _localizer;
    private AudioSession _currentSession;

    /// <summary>
    /// Initializes a new instance of the <see cref="AudioService"/> class.
    /// </summary>
    public AudioService(
        IAudioClient drawClient,
        IHostApplicationLifetime lifetime,
        IStringResourceToolkit stringResourceToolkit)
    {
        _client = drawClient;
        _localizer = stringResourceToolkit;
        lifetime.ApplicationStopping.Register(_client.Dispose);
    }

    /// <inheritdoc/>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            var provider = AskProvider();
            await RunAsync(provider);
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex);
            Environment.Exit(1);
        }
    }

    /// <inheritdoc/>
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private async Task RunAsync(ProviderType provider)
    {
        if (provider == ProviderType.AzureSpeech)
        {
            await _client.InitialAzureSpeechAsync();
        }

        var model = AskModel(provider);
        _currentSession = new AudioSession
        {
            Id = Guid.NewGuid().ToString("N"),
            Provider = provider,
            Model = model.Id,
        };
        await AudioAsync();
    }

    private async Task AudioAsync()
    {
        WriteLine();
        var voice = AskVoice(_currentSession.Model);
        var prompt = AskInput();
        ReadOnlyMemory<byte> speech = default;
        await AnsiConsole.Status()
            .StartAsync(_localizer.GetString("Processing"), async ctx =>
            {
                _currentSession.Text = prompt;
                _currentSession.Voice = voice.Id;
                _currentSession.Speed = 1d;
                speech = await _client.TextToSpeechAsync(_currentSession, CancellationToken.None);
                var audioFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Speech", $"{_currentSession.Id}.mp3");
                if (!Directory.Exists(Path.GetDirectoryName(audioFile)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(audioFile));
                }

                await File.WriteAllBytesAsync(audioFile, speech.Span.ToArray());
                AnsiConsole.Markup($"[bold green]{_localizer.GetString("SpeechSaved")}[/] {audioFile}");
            });

        Environment.Exit(0);
    }

    private ProviderType AskProvider()
    {
        var providers = AudioStatics.GetOnlineAudioServices();
        var provider = AnsiConsole.Prompt(
             new SelectionPrompt<ProviderType>()
             .Title(_localizer.GetString("SelectProvider"))
             .PageSize(10)
             .AddChoices(providers.Keys)
             .UseConverter(p => providers[p]));
        return provider;
    }

    private AudioVoice AskVoice(string modelId)
    {
        var models = _client.GetModels(_currentSession.Provider);
        var currentModel = models.FirstOrDefault(m => m.Id == modelId);
        var voices = currentModel?.Voices;
        var voice = AnsiConsole.Prompt(
                new SelectionPrompt<AudioVoice>()
                    .Title(_localizer.GetString("SelectVoice"))
                    .PageSize(10)
                    .UseConverter(v => v.DisplayName)
                    .AddChoices(voices));
        return voice;
    }

    private AudioModel AskModel(ProviderType type)
    {
        var models = _client.GetModels(type);
        AudioModel? selectedModel = default;
        selectedModel ??= AnsiConsole.Prompt(
                new SelectionPrompt<AudioModel>()
                .Title(_localizer.GetString("SelectModel"))
                .PageSize(10)
                .AddChoices(models));

        return selectedModel;
    }

    private string AskInput()
    {
        _ = this;
    input:
        AnsiConsole.Markup("[grey]>>>[/] ");
        var message = ReadLine();

        if (string.IsNullOrWhiteSpace(message) || message.Equals("/exit", StringComparison.InvariantCultureIgnoreCase))
        {
            Environment.Exit(0);
        }
        else if (message.Equals("/clear", StringComparison.InvariantCultureIgnoreCase))
        {
            Clear();
            goto input;
        }
        else if (message.Equals("/back", StringComparison.InvariantCultureIgnoreCase))
        {
            Clear();
            RunAsync(_currentSession.Provider).Wait();
            Environment.Exit(0);
        }
        else if (message.Equals("/home", StringComparison.InvariantCultureIgnoreCase))
        {
            Clear();
            StartAsync(CancellationToken.None).Wait();
            Environment.Exit(0);
        }

        return message;
    }
}
