// Copyright (c) Rodel. All rights reserved.

using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RodelChat.Core.Factories;
using RodelChat.Interfaces.Client;
using RodelChat.Models.Client;

/// <summary>
/// 程序帮助方法.
/// </summary>
public partial class Program
{
    private static void ConfigureConsole() => Console.OutputEncoding = Encoding.UTF8;

    private static IChatProviderFactory GetChatProviderFactory(IServiceProvider provider)
    {
        var env = provider.GetRequiredService<IHostEnvironment>();
        var configPath = Path.Combine(env.ContentRootPath, "config.json");
        if (!File.Exists(configPath))
        {
            throw new Exception("Config file not found.");
        }

        var configContent = File.ReadAllText(configPath);
        var config = JsonSerializer.Deserialize<ChatClientConfiguration>(configContent);
        return new ChatProviderFactory(config);
    }
}
