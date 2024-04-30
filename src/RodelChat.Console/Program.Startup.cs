// Copyright (c) Rodel. All rights reserved.

using RodelChat.Core.Models.Chat;
using RodelChat.Core.Models.Constants;

/// <summary>
/// 启动部分.
/// </summary>
public partial class Program
{
    private static async Task RunAzureOpenAIAsync(AzureOpenAIServiceConfig? config)
    {
        if (config == null)
        {
            // TODO: 询问用户是否要创建一个新的配置.
            return;
        }

        _chatClient.InitializeAzureOpenAI(
            config.Key,
            config.Endpoint,
            customModels: config.CustomModels);

        _chatClient.SetDefaultProvider(ProviderType.AzureOpenAI);

        var model = AskModel(_chatClient.GetServerModels(ProviderType.AzureOpenAI), config.CustomModels, config.DefaultModelId);
        var session = _chatClient.CreateSession(ChatParameters.Create(), ProviderType.AzureOpenAI, model.Id);
        await LoopMessageAsync(session);
    }

    private static async Task RunZhipuAsync(ServiceConfigBase? config)
    {
        if (config == null)
        {
            // TODO: 询问用户是否要创建一个新的配置.
            return;
        }

        _chatClient.InitializeZhipu(config.Key, config.CustomModels);
        _chatClient.SetDefaultProvider(ProviderType.Zhipu);

        var model = AskModel(_chatClient.GetServerModels(ProviderType.Zhipu), config.CustomModels, config.DefaultModelId);
        var session = _chatClient.CreateSession(ChatParameters.Create(), ProviderType.Zhipu, model.Id);
        await LoopMessageAsync(session);
    }

    private static async Task RunLingYiAsync(ServiceConfigBase? config)
    {
        if (config == null)
        {
            // TODO: 询问用户是否要创建一个新的配置.
            return;
        }

        _chatClient.InitializeLingYi(config.Key, config.CustomModels);
        _chatClient.SetDefaultProvider(ProviderType.LingYi);

        var model = AskModel(_chatClient.GetServerModels(ProviderType.LingYi), config.CustomModels, config.DefaultModelId);
        var session = _chatClient.CreateSession(ChatParameters.Create(), ProviderType.LingYi, model.Id);
        await LoopMessageAsync(session);
    }

    private static async Task RunMoonshotAsync(ServiceConfigBase? config)
    {
        if (config == null)
        {
            // TODO: 询问用户是否要创建一个新的配置.
            return;
        }

        _chatClient.InitializeMoonshot(config.Key, config.CustomModels);
        _chatClient.SetDefaultProvider(ProviderType.Moonshot);

        var model = AskModel(_chatClient.GetServerModels(ProviderType.Moonshot), config.CustomModels, config.DefaultModelId);
        var session = _chatClient.CreateSession(ChatParameters.Create(), ProviderType.Moonshot, model.Id);
        await LoopMessageAsync(session);
    }

    private static async Task RunDashScopeAsync(ServiceConfigBase? config)
    {
        if (config == null)
        {
            // TODO: 询问用户是否要创建一个新的配置.
            return;
        }

        _chatClient.InitializeDashScope(config.Key, config.CustomModels);
        _chatClient.SetDefaultProvider(ProviderType.DashScope);

        var model = AskModel(_chatClient.GetServerModels(ProviderType.DashScope), config.CustomModels, config.DefaultModelId);
        var session = _chatClient.CreateSession(ChatParameters.Create(), ProviderType.DashScope, model.Id);
        await LoopMessageAsync(session);
    }

    private static async Task RunQianFanAsync(QianFanServiceConfig? config)
    {
        if (config == null)
        {
            // TODO: 询问用户是否要创建一个新的配置.
            return;
        }

        _chatClient.InitializeQianFan(config.Key, config.Secret, config.CustomModels);
        _chatClient.SetDefaultProvider(ProviderType.QianFan);

        var model = AskModel(_chatClient.GetServerModels(ProviderType.QianFan), config.CustomModels, config.DefaultModelId);
        var session = _chatClient.CreateSession(ChatParameters.Create(), ProviderType.QianFan, model.Id);
        await LoopMessageAsync(session);
    }

    private static async Task RunSparkDeskAsync(SparkDeskServiceConfig? config)
    {
        if (config == null)
        {
            // TODO: 询问用户是否要创建一个新的配置.
            return;
        }

        _chatClient.InitializeSparkDesk(config.Key, config.Secret, config.AppId, config.CustomModels);
        _chatClient.SetDefaultProvider(ProviderType.SparkDesk);

        var model = AskModel(_chatClient.GetServerModels(ProviderType.SparkDesk), config.CustomModels, config.DefaultModelId);
        var session = _chatClient.CreateSession(ChatParameters.Create(), ProviderType.SparkDesk, model.Id);
        await LoopMessageAsync(session);
    }

    private static async Task RunGeminiAsync(ServiceConfigBase? config)
    {
        if (config == null)
        {
            // TODO: 询问用户是否要创建一个新的配置.
            return;
        }

        _chatClient.InitializeGemini(config.Key, config.CustomModels);
        _chatClient.SetDefaultProvider(ProviderType.Gemini);

        var model = AskModel(_chatClient.GetServerModels(ProviderType.Gemini), config.CustomModels, config.DefaultModelId);
        var session = _chatClient.CreateSession(ChatParameters.Create(), ProviderType.Gemini, model.Id);
        await LoopMessageAsync(session);
    }

    private static async Task RunGroqAsync(ServiceConfigBase? config)
    {
        if (config == null)
        {
            // TODO: 询问用户是否要创建一个新的配置.
            return;
        }

        _chatClient.InitializeGroq(config.Key, config.CustomModels);
        _chatClient.SetDefaultProvider(ProviderType.Groq);

        var model = AskModel(_chatClient.GetServerModels(ProviderType.Groq), config.CustomModels, config.DefaultModelId);
        var session = _chatClient.CreateSession(ChatParameters.Create(), ProviderType.Groq, model.Id);
        await LoopMessageAsync(session);
    }

    private static async Task RunMistralAIAsync(ServiceConfigBase? config)
    {
        if (config == null)
        {
            // TODO: 询问用户是否要创建一个新的配置.
            return;
        }

        _chatClient.InitializeMistralAI(config.Key, config.CustomModels);
        _chatClient.SetDefaultProvider(ProviderType.MistralAI);

        var model = AskModel(_chatClient.GetServerModels(ProviderType.MistralAI), config.CustomModels, config.DefaultModelId);
        var session = _chatClient.CreateSession(ChatParameters.Create(), ProviderType.MistralAI, model.Id);
        await LoopMessageAsync(session);
    }

    private static async Task RunPerplexityAsync(ServiceConfigBase? config)
    {
        if (config == null)
        {
            // TODO: 询问用户是否要创建一个新的配置.
            return;
        }

        _chatClient.InitializePerplexity(config.Key, config.CustomModels);
        _chatClient.SetDefaultProvider(ProviderType.Perplexity);

        var model = AskModel(_chatClient.GetServerModels(ProviderType.Perplexity), config.CustomModels, config.DefaultModelId);
        var session = _chatClient.CreateSession(ChatParameters.Create(), ProviderType.Perplexity, model.Id);
        await LoopMessageAsync(session);
    }
}
