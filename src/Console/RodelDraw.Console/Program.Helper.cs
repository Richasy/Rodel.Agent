// Copyright (c) Rodel. All rights reserved.

using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RodelDraw.Core.Factories;
using RodelDraw.Interfaces.Client;
using RodelDraw.Models.Client;

/// <summary>
/// 程序帮助方法.
/// </summary>
public partial class Program
{
    private static void ConfigureConsole() => Console.OutputEncoding = Encoding.UTF8;

    private static IDrawProviderFactory GetChatProviderFactory(IServiceProvider provider)
    {
        var env = provider.GetRequiredService<IHostEnvironment>();
        var configPath = Path.Combine(env.ContentRootPath, "config.json");
        if (!File.Exists(configPath))
        {
            throw new Exception("Config file not found.");
        }

        var configContent = File.ReadAllText(configPath);
        var config = JsonSerializer.Deserialize<DrawClientConfiguration>(configContent);
        return new DrawProviderFactory(config);
    }
}
