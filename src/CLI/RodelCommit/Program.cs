// Copyright (c) Richasy. All rights reserved.

using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Richasy.AgentKernel;
using RichasyKernel;
using RodelCommit;
using System.Diagnostics;
using System.Text;

Console.OutputEncoding = Encoding.UTF8;

if (args.Length > 0)
{
    Parser.Default.ParseArguments<Options>(args)
        .WithParsed<Options>(opt =>
        {
            if (opt.OpenConfig)
            {
                var directory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                var configDirectory = Path.Combine(directory, ".rodel-commit");
                if (!Directory.Exists(configDirectory))
                {
                    Directory.CreateDirectory(configDirectory);
                }

                var path = Path.Combine(configDirectory, "config.json");
                if (!File.Exists(path))
                {
                    var exampleFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.example.json");
                    File.Copy(exampleFile, path);
                }

                Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
            }
        });

    return;
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
    .Build();

builder.Services.AddSingleton(kernel);
builder.Services.AddSingleton<IChatConfigManager, ChatConfigManager>();
builder.Services.AddHostedService<CommitService>();

using var host = builder.Build();
await host.RunAsync().ConfigureAwait(true);