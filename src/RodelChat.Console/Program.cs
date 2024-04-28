// Copyright (c) Rodel. All rights reserved.

using System.Text.Json;
using RodelChat.Core.Models.Constants;
using Spectre.Console;

ConfigureConsole();

ConsoleConfig config = default;
if (IsConfigExist())
{
    var configContent = File.ReadAllText(GetConfigPath());
    config = JsonSerializer.Deserialize<ConsoleConfig>(configContent, new JsonSerializerOptions(JsonSerializerDefaults.Web));
}

config ??= new ConsoleConfig();
_chatClient = new RodelChat.Core.ChatClient();

_testTools.Add(new OpenAI.Tool(new OpenAI.Function("get_bilibili_hot_search", "Get BiliBili（B站）Hot search list.")));
_chatClient.Tools.AddRange(_testTools);

var provider = AnsiConsole.Prompt(
    new SelectionPrompt<ProviderType>()
    .Title(GetString("SelectProvider"))
    .PageSize(10)
    .UseConverter(ConvertProviderTypeToString)
    .AddChoices(
        ProviderType.OpenAI,
        ProviderType.AzureOpenAI,
        ProviderType.Zhipu,
        ProviderType.LingYi,
        ProviderType.Moonshot,
        ProviderType.DashScope,
        ProviderType.QianFan));

try
{
    if (provider == ProviderType.OpenAI)
    {
    }
    else if (provider == ProviderType.AzureOpenAI)
    {
        await RunAzureOpenAIAsync(config.AzureOpenAI);
    }
    else if (provider == ProviderType.Zhipu)
    {
        await RunZhipuAsync(config.Zhipu);
    }
    else if (provider == ProviderType.LingYi)
    {
        await RunLingYiAsync(config.LingYi);
    }
    else if (provider == ProviderType.Moonshot)
    {
        await RunMoonshotAsync(config.Moonshot);
    }
    else if (provider == ProviderType.DashScope)
    {
        await RunDashScopeAsync(config.DashScope);
    }
    else if (provider == ProviderType.QianFan)
    {
        await RunQianFanAsync(config.QianFan);
    }
    else
    {
        AnsiConsole.MarkupLine(GetString("UnknownProvider"));
    }
}
catch (Exception ex)
{
    AnsiConsole.WriteException(ex);
}

string ConvertProviderTypeToString(ProviderType provider)
{
    return provider switch
    {
        ProviderType.OpenAI => "Open AI",
        ProviderType.AzureOpenAI => "Azure Open AI",
        ProviderType.Zhipu => GetString("Zhipu"),
        ProviderType.LingYi => GetString("LingYi"),
        ProviderType.Moonshot => GetString("Moonshot"),
        ProviderType.DashScope => GetString("DashScope"),
        ProviderType.QianFan => GetString("QianFan"),
        _ => "Unknown"
    };
}
