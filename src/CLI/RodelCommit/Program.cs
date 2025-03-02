// Copyright (c) Richasy. All rights reserved.

using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Richasy.AgentKernel;
using RichasyKernel;
using RodelCommit;
using Spectre.Console;
using System.Diagnostics;
using System.Text;

Console.OutputEncoding = Encoding.UTF8;

if (args.Length > 0)
{
    var shouldExit = true;
    Parser.Default.ParseArguments<Options>(args)
        .WithParsed<Options>(opt =>
        {
            var directory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var configDirectory = Path.Combine(directory, ".rodel-commit");
            if (!Directory.Exists(configDirectory))
            {
                Directory.CreateDirectory(configDirectory);
            }

            if (opt.OpenConfig)
            {
                var path = Path.Combine(configDirectory, "config.json");
                if (!File.Exists(path))
                {
                    var exampleFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.example.json");
                    File.Copy(exampleFile, path);
                }

                Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
            }
            else if (!string.IsNullOrEmpty(opt.RepoConfigName))
            {
                // 如果名称中包含标点符号或空格，则抛出异常.
                if (opt.RepoConfigName.Any(p => char.IsPunctuation(p) || char.IsWhiteSpace(p)))
                {
                    AnsiConsole.WriteException(new ArgumentException("The name of repository descriptor cannot contain punctuation or whitespace."));
                    return;
                }

                var filePath = Path.Combine(configDirectory, $"{opt.RepoConfigName}.txt");
                if (File.Exists(filePath))
                {
                    AnsiConsole.WriteException(new ArgumentException("The repository descriptor already exists."));
                    return;
                }

                var currentDirectory = Environment.CurrentDirectory;
                File.WriteAllText(filePath, $"{currentDirectory}\n\n// Repository Description");
                AnsiConsole.MarkupLine($"[green]The repository descriptor has been created successfully. Path: {filePath}[/]");
                Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
                return;
            }
            else if (opt.Manual)
            {
                ChatConfigManager.ShouldManual = true;
                shouldExit = false;
            }
        });

    if (shouldExit)
    {
        return;
    }
}

var builder = Host.CreateApplicationBuilder(args);
builder.Environment.ContentRootPath = AppDomain.CurrentDomain.BaseDirectory;

var kernel = Kernel.CreateBuilder()
    .AddOpenAIChatService()
    .AddAzureOpenAIChatService()
    .AddAzureAIChatService()
    .AddXAIChatService()
    .AddZhiPuChatService()
    .AddLingYiChatService()
    .AddAnthropicChatService()
    .AddMoonshotChatService()
    .AddGeminiChatService()
    .AddDeepSeekChatService()
    .AddQwenChatService()
    .AddErnieChatService()
    .AddHunyuanChatService()
    .AddSparkChatService()
    .AddDoubaoChatService()
    .AddSiliconFlowChatService()
    .AddOpenRouterChatService()
    .AddTogetherAIChatService()
    .AddGroqChatService()
    .AddOllamaChatService()
    .AddPerplexityChatService()
    .AddMistralChatService()
    .AddOnnxChatService()
    .Build();

builder.Services.AddSingleton(kernel);
builder.Services.AddSingleton<IChatConfigManager, ChatConfigManager>();
builder.Services.AddHostedService<CommitService>();

using var host = builder.Build();
await host.RunAsync().ConfigureAwait(true);