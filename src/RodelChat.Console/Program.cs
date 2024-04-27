// Copyright (c) Rodel. All rights reserved.

using System.Text.Json;
using RodelChat.Console;
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

var provider = AnsiConsole.Prompt(
    new SelectionPrompt<ProviderType>()
    .Title(GetString("SelectProvider"))
    .PageSize(10)
    .UseConverter(ConvertProviderTypeToString)
    .AddChoices(ProviderType.OpenAI, ProviderType.AzureOpenAI, ProviderType.Zhipu, ProviderType.LingYi, ProviderType.Moonshot));

try
{
    _chatClient.Tools.Add(OpenAI.Tool.GetOrCreateTool(typeof(TestTools), nameof(TestTools.GetWeather), "Get weather of the city"));
    _chatClient.Tools.Add(OpenAI.Tool.GetOrCreateTool(typeof(TestTools), nameof(TestTools.GetBiliBiliHotSearch), "Get BiliBili hot search"));

    if (provider == ProviderType.OpenAI)
    {
    }
    else if (provider == ProviderType.AzureOpenAI)
    {
        await RunAzureOpenAIAsync(config.AzureOpenAI);
    }
    else if (provider == ProviderType.Zhipu)
    {
    }
    else if (provider == ProviderType.LingYi)
    {
    }
    else if (provider == ProviderType.Moonshot)
    {
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
        _ => "Unknown"
    };
}
