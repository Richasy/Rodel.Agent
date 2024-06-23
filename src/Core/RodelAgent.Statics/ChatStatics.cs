// Copyright (c) Rodel. All rights reserved.

using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using RodelAgent.Interfaces;
using RodelChat.Models.Constants;

namespace RodelAgent.Statics;

/// <summary>
/// 聊天静态类.
/// </summary>
public static class ChatStatics
{
    /// <summary>
    /// 获取在线聊天服务.
    /// </summary>
    /// <returns>服务列表.</returns>
    public static Dictionary<ProviderType, string> GetOnlineChatServices()
    {
        var resourceToolkit = GlobalStatics.ServiceProvider.GetRequiredService<IStringResourceToolkit>();
        return new Dictionary<ProviderType, string>
        {
            { ProviderType.OpenAI, "Open AI" },
            { ProviderType.AzureOpenAI, "Azure Open AI" },
            { ProviderType.Gemini, "Gemini" },
            { ProviderType.Anthropic, "Anthropic" },
            { ProviderType.Moonshot, resourceToolkit.GetString("Moonshot") },
            { ProviderType.ZhiPu, resourceToolkit.GetString("ZhiPu") },
            { ProviderType.LingYi, resourceToolkit.GetString("LingYi") },
            { ProviderType.DeepSeek, "Deep Seek" },
            { ProviderType.DashScope, resourceToolkit.GetString("DashScope") },
            { ProviderType.QianFan, resourceToolkit.GetString("QianFan") },
            { ProviderType.HunYuan, resourceToolkit.GetString("HunYuan") },
            { ProviderType.SparkDesk, resourceToolkit.GetString("SparkDesk") },
            { ProviderType.SiliconFlow, "Silicon Cloud" },
            { ProviderType.OpenRouter, "Open Router" },
            { ProviderType.TogetherAI, "Together AI" },
            { ProviderType.Groq, "Groq" },
            { ProviderType.Perplexity, "Perplexity" },
            { ProviderType.MistralAI, "Mistral AI" },
            { ProviderType.Ollama, "Ollama" },
        };
    }
}
