// Copyright (c) Rodel. All rights reserved.

using System.Text.Json;
using Microsoft.Extensions.Hosting;
using RodelAgent.Interfaces;
using RodelAgent.Statics;
using RodelChat.Interfaces.Client;
using RodelChat.Models.Client;
using RodelChat.Models.Constants;
using Spectre.Console;

/// <summary>
/// 聊天服务.
/// </summary>
public sealed class ChatService : IHostedService
{
    private readonly IChatClient _chatClient;
    private readonly IStringResourceToolkit _localizer;
    private ProviderType _currentType;
    private ChatSession _currentSession;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatService"/> class.
    /// </summary>
    public ChatService(
        IChatClient chatClient,
        IHostApplicationLifetime lifetime,
        IStringResourceToolkit localizer)
    {
        _chatClient = chatClient;
        _localizer = localizer;
        lifetime.ApplicationStopping.Register(_chatClient.Dispose);
    }

    /// <inheritdoc/>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
#if USE_PRESET
            var preset = AskSessionPreset();
            var session = _chatClient.CreateSession(preset);
            _currentType = session.Provider;
            await RunAIAsync(session);
#else
            var provider = AskProvider();
            await RunAIAsync(provider);
#endif
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
        _currentType = type;
        var model = AskModel(type);
        var session = _chatClient.CreateSession(type, modelId: model.Id);
        await RunAIAsync(session);
    }

    private async Task RunAIAsync(ChatSession session)
    {
        _currentSession = session;
        await LoopMessageAsync(session);
    }

    private async Task LoopMessageAsync(ChatSession session)
    {
#if USE_SYSTEM_PROMPT
        var sysPrompt = AnsiConsole.Prompt(
            new TextPrompt<string>($"{GetString("SystemInstruction")} [green]({GetString("Optional")})[/]: ")
            .AllowEmpty());

        if (!string.IsNullOrEmpty(sysPrompt))
        {
            session.SystemInstruction = sysPrompt;
        }
#endif

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
                    response = await _chatClient.SendMessageAsync(session.Id, chatMsg);
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
        var chatProviders = ChatStatics.GetOnlineChatServices();
        var provider = AnsiConsole.Prompt(
            new SelectionPrompt<ProviderType>()
            .Title(GetString("SelectProvider"))
            .PageSize(20)
            .UseConverter(p => chatProviders[p])
            .AddChoices(chatProviders.Keys));

        return provider;
    }

    private ChatSessionPreset AskSessionPreset()
    {
        // Get presets file list in Presets folder without extension name.
        var presetFiles = Directory.GetFiles(Path.Combine(AppContext.BaseDirectory, "Presets"), "*.json");
        var presetNames = presetFiles.Select(p => Path.GetFileNameWithoutExtension(p)!).ToList();

        var presetName = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                   .Title(GetString("SelectPreset"))
                   .PageSize(10)
                   .AddChoices(presetNames));

        var presetFile = Path.Combine(AppContext.BaseDirectory, "Presets", $"{presetName}.json");
        var presetContent = File.ReadAllText(presetFile);
        var preset = JsonSerializer.Deserialize<ChatSessionPreset>(presetContent);
        return preset;
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
    input:
        AnsiConsole.Markup("[grey]>>>[/] ");
        var message = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(message) || message.Equals("/exit", StringComparison.InvariantCultureIgnoreCase))
        {
            Environment.Exit(0);
        }
        else if (message.Equals("/clear", StringComparison.InvariantCultureIgnoreCase))
        {
            AnsiConsole.Clear();
            _currentSession.Messages.Clear();
            goto input;
        }
        else if (message.Equals("/back", StringComparison.InvariantCultureIgnoreCase))
        {
            Console.Clear();
            RunAIAsync(_currentType).Wait();
            Environment.Exit(0);
        }
        else if (message.Equals("/home", StringComparison.InvariantCultureIgnoreCase))
        {
            Console.Clear();
            StartAsync(CancellationToken.None).Wait();
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

        var panel = new Panel(text.EscapeMarkup())
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

    private string GetString(string name)
        => _localizer.GetString(name);
}
