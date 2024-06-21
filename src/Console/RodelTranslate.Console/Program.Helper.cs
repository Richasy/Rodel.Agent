// Copyright (c) Rodel. All rights reserved.

using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RodelTranslate.Core.Factories;
using RodelTranslate.Interfaces.Client;
using RodelTranslate.Models.Client;

/// <summary>
/// 程序帮助方法.
/// </summary>
public partial class Program
{
    private static void ConfigureConsole() => Console.OutputEncoding = Encoding.UTF8;

    private static ITranslateProviderFactory GetChatProviderFactory(IServiceProvider provider)
    {
        var env = provider.GetRequiredService<IHostEnvironment>();
        var configPath = Path.Combine(env.ContentRootPath, "config.json");
        if (!File.Exists(configPath))
        {
            throw new Exception("Config file not found.");
        }

        var configContent = File.ReadAllText(configPath);
        var config = JsonSerializer.Deserialize<TranslateClientConfiguration>(configContent);
        return new TranslateProviderFactory(config);
    }
}
