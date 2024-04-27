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
}
