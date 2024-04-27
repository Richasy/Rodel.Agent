// Copyright (c) Rodel. All rights reserved.

using System.Globalization;
using System.Resources;
using System.Text;
using OpenAI;
using RodelChat.Core;
using RodelChat.Core.Models.Chat;
using RodelChat.Core.Models.Constants;
using Spectre.Console;

/// <summary>
/// 程序帮助方法.
/// </summary>
public partial class Program
{
    private static readonly List<Tool> _testTools = new();
    private static ChatClient _chatClient;
    private static ResourceManager _resourceManager;
    private static CultureInfo _currentCulture;

    private static void ConfigureConsole()
    {
        Console.OutputEncoding = Encoding.UTF8;
        var culture = CultureInfo.CurrentCulture;
        _currentCulture = culture.TwoLetterISOLanguageName == "zh"
           ? new CultureInfo("zh-CN")
           : new CultureInfo("en-US");

        AppDomain.CurrentDomain.ProcessExit += (s, e) =>
        {
            ReleaseResources();
        };

        AppDomain.CurrentDomain.UnhandledException += (s, e) =>
        {
            ReleaseResources();
        };

        Console.CancelKeyPress += (s, e) =>
        {
            ReleaseResources();
            Environment.Exit(0);
        };
    }

    private static string GetString(string name)
    {
        _resourceManager ??= new ResourceManager("RodelChat.Console.Properties.Resource", typeof(Program).Assembly);
        return _resourceManager.GetString(name, _currentCulture) ?? name;
    }

    private static void ReleaseResources()
        => _chatClient?.Dispose();

    private static string GetConfigPath()
    {
        var current = AppDomain.CurrentDomain.BaseDirectory;
        return Path.Combine(current, "config.json");
    }

    private static bool IsConfigExist()
        => File.Exists(GetConfigPath());

    private static async Task LoopMessageAsync(ChatSession session)
    {
        var sysPrompt = AnsiConsole.Ask<string>(GetString("SystemInstruction"), GetString("Optional"));

        if (sysPrompt != GetString("Optional"))
        {
            session.SystemInstruction = sysPrompt;
        }

        session.UseStreamOutput = false;

        while (true)
        {
            var message = AnsiConsole.Ask<string>(GetString("UserInput") + ": ");

            if (string.IsNullOrWhiteSpace(message) || message.Equals("/exit", StringComparison.InvariantCultureIgnoreCase))
            {
                break;
            }

            ChatResponse? response = default;
            await AnsiConsole.Status()
                .StartAsync(GetString("Processing"), async ctx =>
                {
                    response = await _chatClient.SendMessageAsync(session.Id, message);
                });

            var result = await HandleMessageResponseAsync(session, response);
            if (!result)
            {
                break;
            }
        }
    }

    private static async Task<bool> HandleMessageResponseAsync(ChatSession session, ChatResponse response)
    {
        if (response == null)
        {
            return false;
        }

        if (response.Tools != null && response.Tools.Count > 0)
        {
            await AnsiConsole.Status()
                .StartAsync(GetString("ToolHandling"), async ctx =>
                {
                    var toolMessages = new List<ChatMessage>();
                    foreach (var tool in response.Tools)
                    {
                        var toolName = tool.Function.Name;
                        var toolResult = tool.InvokeFunction<string>();
                        toolMessages.Add(ChatMessage.CreateToolMessage(toolName, toolResult, tool.Id));
                    }

                    response = await _chatClient.SendMessageAsync(session.Id, toolCallbacks: toolMessages);
                });

            return await HandleMessageResponseAsync(session, response);
        }
        else if (response.Message.Role == MessageRole.Assistant)
        {
            var msg = response.Message;
            AnsiConsole.MarkupLine($"[bold green]{GetString("AssistantOutput")}[/]: {msg.Content}");
            return true;
        }
        else if (response.Message.Role == MessageRole.Client)
        {
            var oldFore = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine($"Client: {response.Message.Content}");
            Console.ForegroundColor = oldFore;
        }

        return false;
    }
}
