// Copyright (c) Rodel. All rights reserved.

using System.Text.Json;
using RodelChat.Core.Factories;
using RodelChat.Models.Chat;
using RodelChat.Models.Client;
using RodelChat.Models.Constants;
using Spectre.Console;

ConfigureConsole();

ChatClientConfiguration config = default;
if (IsConfigExist())
{
    var configContent = File.ReadAllText(GetConfigPath());
    config = JsonSerializer.Deserialize<ChatClientConfiguration>(configContent, new JsonSerializerOptions(JsonSerializerDefaults.Web));
}

config ??= new ChatClientConfiguration();
var factory = new ChatProviderFactory(config);
_chatClient = new RodelChat.Core.ChatClient(factory);

var provider = AnsiConsole.Prompt(
    new SelectionPrompt<ProviderType>()
    .Title(GetString("SelectProvider"))
    .PageSize(20)
    .UseConverter(ConvertProviderTypeToString)
    .AddChoices(
        ProviderType.OpenAI,
        ProviderType.AzureOpenAI,
        ProviderType.ZhiPu,
        ProviderType.LingYi,
        ProviderType.Moonshot,
        ProviderType.DashScope,
        ProviderType.QianFan,
        ProviderType.SparkDesk,
        ProviderType.Gemini,
        ProviderType.Groq,
        ProviderType.MistralAI,
        ProviderType.Perplexity,
        ProviderType.TogetherAI,
        ProviderType.OpenRouter,
        ProviderType.Anthropic,
        ProviderType.Ollama));

try
{
    await RunAIAsync(provider);
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
        ProviderType.ZhiPu => GetString("Zhipu"),
        ProviderType.LingYi => GetString("LingYi"),
        ProviderType.Moonshot => GetString("Moonshot"),
        ProviderType.DashScope => GetString("DashScope"),
        ProviderType.QianFan => GetString("QianFan"),
        ProviderType.SparkDesk => GetString("SparkDesk"),
        ProviderType.Gemini => "Gemini",
        ProviderType.Groq => "Groq",
        ProviderType.MistralAI => "Mistral AI",
        ProviderType.Perplexity => "Perplexity",
        ProviderType.TogetherAI => "Together AI",
        ProviderType.OpenRouter => "Open Router",
        ProviderType.Anthropic => "Anthropic",
        ProviderType.Ollama => "Ollama",
        _ => "Unknown"
    };
}

async Task RunAIAsync(ProviderType type)
{
    var model = AskModel(type);
    var session = _chatClient.CreateSession(type, ChatParameters.Create(), model.Id);
    await LoopMessageAsync(session);
}
