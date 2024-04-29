// Copyright (c) Rodel. All rights reserved.

using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Text;
using RodelChat.Core;
using RodelChat.Core.Models.Chat;
using RodelChat.Core.Models.Constants;
using Spectre.Console;

/// <summary>
/// 程序帮助方法.
/// </summary>
public partial class Program
{
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

            ChatMessage? chatMsg = default;
            List<string>? images = default;

            while (true)
            {
                if (message.StartsWith("/img ", StringComparison.InvariantCultureIgnoreCase))
                {
                    var imgPath = message.Replace("/img ", string.Empty).Trim('"').Trim();
                    images ??= new List<string>();
                    if (!imgPath.StartsWith("http"))
                    {
                        var containPrefix = session.Provider == ProviderType.AzureOpenAI || session.Provider == ProviderType.OpenAI;
                        var base64 = ConvertToBase64(imgPath, containPrefix);
                        if (string.IsNullOrEmpty(base64))
                        {
                            AnsiConsole.MarkupLine($"[bold red]{GetString("ImageNotFound")}[/]");
                        }

                        images.Add(base64);
                    }
                }
                else
                {
                    chatMsg = images != null
                        ? ChatMessage.CreateUserMessage(message, images.ToArray())
                        : ChatMessage.CreateUserMessage(message);
                    break;
                }

                message = AnsiConsole.Ask<string>(GetString("UserInput") + ": ");
            }

            ChatMessage? response = default;
            await AnsiConsole.Status()
                .StartAsync(GetString("Processing"), async ctx =>
                {
                    response = await _chatClient.SendMessageAsync(session.Id, chatMsg);
                });

            var result = HandleMessageResponse(session, response);
            if (!result)
            {
                break;
            }
        }
    }

    private static bool HandleMessageResponse(ChatSession session, ChatMessage response)
    {
        if (response == null)
        {
            return false;
        }

        if (response.Role == MessageRole.Assistant)
        {
            var msg = response;
            AnsiConsole.MarkupLine($"[bold green]{GetString("AssistantOutput")}[/]: {msg.GetFirstTextContent()}");
            return true;
        }
        else if (response.Role == MessageRole.Client)
        {
            var oldFore = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine($"Client: {response.GetFirstTextContent()}");
            Console.ForegroundColor = oldFore;
        }

        return false;
    }

    private static string ConvertToBase64(string path, bool containPrefix = true)
    {
        if (!File.Exists(path))
        {
            return string.Empty;
        }

        using var img = Image.FromFile(path);
        using var ms = new MemoryStream();
        img.Save(ms, img.RawFormat);
        var imageBytes = ms.ToArray();
        var base64String = Convert.ToBase64String(imageBytes);
        return containPrefix
            ? $"data:image/jpeg;base64,{base64String}"
            : base64String;
    }

    private static ChatModel AskModel(List<ChatModel> serverModels, List<ChatModel>? customModels = default, string? defaultModelId = "")
    {
        var totalModels = new List<ChatModel>();
        totalModels.AddRange(serverModels);
        if (customModels != null)
        {
            totalModels.AddRange(customModels);
        }

        totalModels = totalModels.Distinct().ToList();

        ChatModel? selectedModel = default;
        if (!string.IsNullOrEmpty(defaultModelId))
        {
            selectedModel = totalModels.FirstOrDefault(m => m.Id == defaultModelId);
        }

        selectedModel ??= AnsiConsole.Prompt(
                new SelectionPrompt<ChatModel>()
                .Title(GetString("SelectModel"))
                .PageSize(10)
                .AddChoices(totalModels));

        return selectedModel;
    }
}
