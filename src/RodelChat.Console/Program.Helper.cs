// Copyright (c) Rodel. All rights reserved.

using System.Diagnostics;
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
        return new ChatProviderFactory(config, ToolInvoking, ToolInvoked);
    }

    private static void ToolInvoking(ToolInvokingEventArgs args)
    {
        Debug.WriteLine($"准备调用工具： {args.Function.PluginName}/{args.Function.Name}");
        Debug.WriteLine($"模型 ID： {args.ModelId}");
        Debug.WriteLine($"参数： {JsonSerializer.Serialize(args.Parameters)}");
    }

    private static void ToolInvoked(ToolInvokedEventArgs args)
    {
        Debug.WriteLine($"工具调用完成： {args.Function.PluginName}/{args.Function.Name}");
        Debug.WriteLine($"模型 ID： {args.ModelId}");
        Debug.WriteLine($"结果： {args.Result}");
    }
}
