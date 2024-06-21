// Copyright (c) Rodel. All rights reserved.

using System.Globalization;
using Microsoft.Extensions.Hosting;
using Microsoft.SemanticKernel;
using RodelAgent.Interfaces;
using RodelAgent.Statics;
using RodelTranslate.Interfaces.Client;
using RodelTranslate.Models.Client;
using RodelTranslate.Models.Constants;
using Spectre.Console;

using static System.Console;

namespace RodelTranslate.Console;

/// <summary>
/// 翻译服务.
/// </summary>
public sealed class TranslateService : IHostedService
{
    private readonly ITranslateClient _client;
    private readonly IStringResourceToolkit _localizer;
    private TranslateSession _currentSession;

    /// <summary>
    /// Initializes a new instance of the <see cref="TranslateService"/> class.
    /// </summary>
    public TranslateService(
        ITranslateClient translateClient,
        IHostApplicationLifetime lifetime,
        IStringResourceToolkit stringResourceToolkit)
    {
        _client = translateClient;
        _localizer = stringResourceToolkit;
        lifetime.ApplicationStopping.Register(_client.Dispose);
    }

    /// <inheritdoc/>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            var provider = AskProvier();
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
        var language = AskLanguage(provider);
        _currentSession = new TranslateSession
        {
            Provider = provider,
            Id = Guid.NewGuid().ToString("N"),
            TargetLanguage = language,
        };
        await LoopTranslateAsync();
    }

    private async Task LoopTranslateAsync()
    {
        while (true)
        {
            WriteLine();
            var input = AskInput();
            TranslateTextContent? result = default;
            await AnsiConsole.Status()
                .StartAsync(_localizer.GetString("Processing"), async ctx =>
                {
                    result = await _client.TranslateTextAsync(_currentSession, input, CancellationToken.None);
                });

            if (result != null)
            {
                PrintTranslateResult(result);
            }
            else
            {
                break;
            }
        }
    }

    private ProviderType AskProvier()
    {
        var providers = TranslateStatics.GetOnlineTranslateServices();
        var provider = AnsiConsole.Prompt(
             new SelectionPrompt<ProviderType>()
             .Title(_localizer.GetString("SelectProvider"))
             .PageSize(10)
             .AddChoices(providers.Keys)
             .UseConverter(p => providers[p]));
        return provider;
    }

    private Language AskLanguage(ProviderType type)
    {
        var languages = _client.GetLanguageList(type);
        var language = AnsiConsole.Prompt(
               new SelectionPrompt<Language>()
               .Title(_localizer.GetString("SelectLanguage"))
               .PageSize(20)
               .AddChoices(languages)
               .UseConverter(p => new CultureInfo(p.ISOCode).DisplayName));
        return language;
    }

    private string AskInput()
    {
        _ = this;
    input:
        AnsiConsole.Markup("[grey]>>>[/] ");
        var input = ReadLine();
        if (string.IsNullOrWhiteSpace(input) || input.Equals("/exit", StringComparison.InvariantCultureIgnoreCase))
        {
            Environment.Exit(0);
        }
        else if (input.Equals("/clear", StringComparison.InvariantCultureIgnoreCase))
        {
            Clear();
            goto input;
        }
        else if (input.Equals("/home", StringComparison.InvariantCultureIgnoreCase))
        {
            Clear();
            StartAsync(CancellationToken.None).Wait();
            Environment.Exit(0);
        }

        return input;
    }

    private void PrintTranslateResult(TranslateTextContent result)
    {
        _ = this;
        var text = result.Text;
        var panel = new Panel(text.EscapeMarkup())
        {
            Border = BoxBorder.Rounded,
            Expand = true,
            Padding = new Padding(2, 2, 2, 2),
        };

        AnsiConsole.Write(panel);
    }
}
