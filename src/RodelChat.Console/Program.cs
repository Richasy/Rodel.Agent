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
var plugins = new List<object>() { new TestPlugin() };
_chatClient = new RodelChat.Core.ChatClient(plugins);

var provider = AnsiConsole.Prompt(
    new SelectionPrompt<ProviderType>()
    .Title(GetString("SelectProvider"))
    .PageSize(20)
    .UseConverter(ConvertProviderTypeToString)
    .AddChoices(
        ProviderType.OpenAI,
        ProviderType.AzureOpenAI,
        ProviderType.Zhipu,
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
    else if (provider == ProviderType.SparkDesk)
    {
        await RunSparkDeskAsync(config.SparkDesk);
    }
    else if (provider == ProviderType.Gemini)
    {
        await RunGeminiAsync(config.Gemini);
    }
    else if (provider == ProviderType.Groq)
    {
        await RunGroqAsync(config.Groq);
    }
    else if (provider == ProviderType.MistralAI)
    {
        await RunMistralAIAsync(config.MistralAI);
    }
    else if (provider == ProviderType.Perplexity)
    {
        await RunPerplexityAsync(config.Perplexity);
    }
    else if (provider == ProviderType.TogetherAI)
    {
        await RunTogetherAIAsync(config.TogetherAI);
    }
    else if (provider == ProviderType.OpenRouter)
    {
        await RunOpenRouterAsync(config.OpenRouter);
    }
    else if (provider == ProviderType.Anthropic)
    {
        await RunAnthropicAsync(config.Anthropic);
    }
    else if (provider == ProviderType.Ollama)
    {
        await RunOllamaAsync(config.Ollama);
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
