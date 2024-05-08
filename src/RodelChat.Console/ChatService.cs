// Copyright (c) Rodel. All rights reserved.

using System.Diagnostics;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using RodelChat.Interfaces.Client;
using RodelChat.Models.Chat;
using RodelChat.Models.Constants;
using Spectre.Console;

/// <summary>
/// 聊天服务.
/// </summary>
public sealed class ChatService : IHostedService
{
    private readonly IChatClient _chatClient;
    private readonly IStringLocalizer<ChatService> _localizer;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatService"/> class.
    /// </summary>
    public ChatService(
        IChatClient chatClient,
        IHostApplicationLifetime lifetime,
        IStringLocalizer<ChatService> localizer)
    {
        _chatClient = chatClient;
        _localizer = localizer;
        lifetime.ApplicationStopping.Register(_chatClient.Dispose);
    }

    /// <inheritdoc/>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var provider = AskProvider();

        try
        {
            await RunAIAsync(provider);
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex);
            Environment.Exit(1);
        }
    }

    /// <inheritdoc/>
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private async Task RunAIAsync(ProviderType type)
    {
        var model = AskModel(type);
        var session = _chatClient.CreateSession(type, ChatParameters.Create(maxTokens: 1200), model.Id);
        InitializePlugins("Microsoft.SemanticKernel.Plugins.Core.dll");

        await LoopMessageAsync(session);
    }

    private void InitializePlugins(string pluginDllName)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Plugins", pluginDllName);
        var plugins = _chatClient.RetrievePluginsFromDll(path);
        _chatClient.InjectPluginsToKernel(plugins);
    }

    private async Task LoopMessageAsync(ChatSession session)
    {
        var sysPrompt = AnsiConsole.Prompt(
            new TextPrompt<string>($"{GetString("SystemInstruction")} [green]({GetString("Optional")})[/]: ")
            .AllowEmpty());

        if (!string.IsNullOrEmpty(sysPrompt))
        {
            session.SystemInstruction = sysPrompt;
        }

        session.UseStreamOutput = false;

        while (true)
        {
            Console.WriteLine();
            ChatMessage? chatMsg = default;
            List<string>? images = default;
            var userInputFinished = false;

            while (!userInputFinished)
            {
                var message = AskInput();
                userInputFinished = HandleUserInput(message, session, ref chatMsg, ref images);
            }

            ChatMessage? response = default;
            await AnsiConsole.Status()
                .StartAsync(GetString("Processing"), async ctx =>
                {
                    var plugins = _chatClient.GetKernelPlugins();
                    response = await _chatClient.SendMessageAsync(session.Id, chatMsg, plugins: plugins);
                });

            var result = HandleMessageResponse(response);
            if (!result)
            {
                break;
            }
        }
    }

    private ProviderType AskProvider()
    {
        var provider = AnsiConsole.Prompt(
            new SelectionPrompt<ProviderType>()
            .Title(GetString("SelectProvider"))
            .PageSize(20)
            .UseConverter(ConvertProviderTypeToString)
            .AddChoices(
                ProviderType.OpenAI,
                ProviderType.AzureOpenAI,
                ProviderType.ZhiPu,
                ProviderType.LingYi,
                ProviderType.Moonshot,
                ProviderType.DashScope,
                ProviderType.QianFan,
                ProviderType.SparkDesk,
                ProviderType.Gemini,
                ProviderType.Groq,
                ProviderType.MistralAI,
                ProviderType.Perplexity,
                ProviderType.TogetherAI,
                ProviderType.OpenRouter,
                ProviderType.Anthropic,
                ProviderType.Ollama));

        return provider;
    }

    private ChatModel AskModel(ProviderType type)
    {
        var models = _chatClient.GetModels(type);
        ChatModel? selectedModel = default;
        selectedModel ??= AnsiConsole.Prompt(
                new SelectionPrompt<ChatModel>()
                .Title(GetString("SelectModel"))
                .PageSize(10)
                .AddChoices(models));

        return selectedModel;
    }

    private string AskInput()
    {
        _ = this;
        AnsiConsole.Markup("[grey]>>>[/] ");
        var message = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(message) || message.Equals("/exit", StringComparison.InvariantCultureIgnoreCase))
        {
            Environment.Exit(0);
        }

        return message;
    }

    private bool HandleUserInput(string message, ChatSession session, ref ChatMessage? container, ref List<string>? images)
    {
        if (HandleImageInput(message, session, images))
        {
            return false;
        }

        container = images != null
                ? ChatMessage.CreateUserMessage(message, images.ToArray())
                : ChatMessage.CreateUserMessage(message);
        return true;
    }

    private bool HandleImageInput(string message, ChatSession session, List<string>? images)
    {
        if (string.IsNullOrWhiteSpace(message) || !message.StartsWith("/img ", StringComparison.InvariantCultureIgnoreCase))
        {
            return false;
        }

        var imgPath = message.Replace("/img ", string.Empty).Trim('"').Trim();
        images ??= new List<string>();
        if (!imgPath.StartsWith("http"))
        {
            var containPrefix = session.Provider == ProviderType.AzureOpenAI || session.Provider == ProviderType.OpenAI;
            var base64 = Toolkits.ConvertToBase64(imgPath, containPrefix);
            if (string.IsNullOrEmpty(base64))
            {
                AnsiConsole.MarkupLine($"[bold red]{GetString("ImageNotFound")}[/]");
                return false;
            }

            images.Add(base64);
        }

        return true;
    }

    private bool HandleMessageResponse(ChatMessage response)
    {
        if (response == null)
        {
            return false;
        }

        if (response.Role == MessageRole.Assistant)
        {
            PrintAssistantMessage(response);
        }
        else if (response.Role == MessageRole.Client)
        {
            PrintClientMessage(response);
        }

        return response.Role == MessageRole.Assistant;
    }

    private void PrintAssistantMessage(ChatMessage response)
    {
        _ = this;
        var text = response.GetFirstTextContent();
        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        var panel = new Panel(text)
        {
            Border = BoxBorder.Rounded,
            Expand = true,
            Padding = new Padding(2, 2, 2, 2),
        };

        AnsiConsole.Write(panel);
    }

    private void PrintClientMessage(ChatMessage response)
    {
        var text = response.GetFirstTextContent();
        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        AnsiConsole.MarkupLine($"[yellow]{GetString("Reminder")}: {text.EscapeMarkup()}[/]");
    }

    private string ConvertProviderTypeToString(ProviderType provider)
    {
        return provider switch
        {
            ProviderType.OpenAI => "Open AI",
            ProviderType.AzureOpenAI => "Azure Open AI",
            ProviderType.ZhiPu => GetString("Zhipu"),
            ProviderType.LingYi => GetString("LingYi"),
            ProviderType.Moonshot => GetString("Moonshot"),
            ProviderType.DashScope => GetString("DashScope"),
            ProviderType.QianFan => GetString("QianFan"),
            ProviderType.SparkDesk => GetString("SparkDesk"),
            ProviderType.Gemini => "Gemini",
            ProviderType.Groq => "Groq",
            ProviderType.MistralAI => "Mistral AI",
            ProviderType.Perplexity => "Perplexity",
            ProviderType.TogetherAI => "Together AI",
            ProviderType.OpenRouter => "Open Router",
            ProviderType.Anthropic => "Anthropic",
            ProviderType.Ollama => "Ollama",
            _ => "Unknown"
        };
    }

    private string GetString(string name)
    {
        var str = _localizer.GetString(name);
        if (str.ResourceNotFound)
        {
            Debug.WriteLine($"Resource not found: {name}: {str.SearchedLocation}");
        }

        return str.ResourceNotFound ? string.Empty : str.Value;
    }
}
