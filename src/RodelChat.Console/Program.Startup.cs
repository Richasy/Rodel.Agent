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

        var session = _chatClient.CreateSession(ChatParameters.Create(), ProviderType.AzureOpenAI, "gpt-4-32k");
        await LoopMessageAsync(session);
    }
}
