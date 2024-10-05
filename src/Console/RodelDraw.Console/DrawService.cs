// Copyright (c) Rodel. All rights reserved.

using Microsoft.Extensions.Hosting;
using RodelAgent.Interfaces;
using RodelAgent.Statics;
using RodelDraw.Interfaces.Client;
using RodelDraw.Models.Client;
using RodelDraw.Models.Constants;
using Spectre.Console;
using static System.Console;

namespace RodelDraw.Console;

/// <summary>
/// 绘图服务.
/// </summary>
public sealed class DrawService : IHostedService
{
    private readonly IDrawClient _client;
    private readonly IStringResourceToolkit _localizer;
    private DrawSession _currentSession;

    /// <summary>
    /// Initializes a new instance of the <see cref="DrawService"/> class.
    /// </summary>
    public DrawService(
        IDrawClient drawClient,
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
        var model = AskModel(provider);
        _currentSession = new DrawSession
        {
            Id = Guid.NewGuid().ToString("N"),
            Provider = provider,
            Model = model.Id,
        };
        await DrawAsync();
    }

    private async Task DrawAsync()
    {
        WriteLine();
        var size = AskSize(_currentSession.Model);
        var prompt = AskInput();
        string? base64Content = default;
        await AnsiConsole.Status()
            .StartAsync(_localizer.GetString("Processing"), async ctx =>
            {
                var request = new DrawRequest
                {
                    Prompt = prompt,
                    Size = size,
                };
                _currentSession.Request = request;
                base64Content = await _client.DrawAsync(_currentSession, CancellationToken.None);
                var imageFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", $"{_currentSession.Id}.png");
                if (!Directory.Exists(Path.GetDirectoryName(imageFile)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(imageFile));
                }

                File.WriteAllBytes(imageFile, Convert.FromBase64String(base64Content));
                AnsiConsole.Markup($"[bold green]{_localizer.GetString("ImageSaved")}[/] {imageFile}");
            });

        Environment.Exit(0);
    }

    private ProviderType AskProvider()
    {
        var providers = DrawStatics.GetOnlineDrawServices();
        var provider = AnsiConsole.Prompt(
             new SelectionPrompt<ProviderType>()
             .Title(_localizer.GetString("SelectProvider"))
             .PageSize(10)
             .AddChoices(providers.Keys)
             .UseConverter(p => providers[p]));
        return provider;
    }

    private string AskSize(string modelId)
    {
        var models = _client.GetModels(_currentSession.Provider);
        var currentModel = models.FirstOrDefault(m => m.Id == modelId);
        var sizes = currentModel?.SupportSizes ?? ["1024x1024"];
        var size = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title(_localizer.GetString("SelectSize"))
                    .PageSize(10)
                    .AddChoices(sizes));
        return size;
    }

    private DrawModel AskModel(ProviderType type)
    {
        var models = _client.GetModels(type);
        DrawModel? selectedModel = default;
        selectedModel ??= AnsiConsole.Prompt(
                new SelectionPrompt<DrawModel>()
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
