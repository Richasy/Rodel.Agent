// Copyright (c) Richasy. All rights reserved.

using Connectors.DeepSeek.Models;
using Richasy.AgentKernel;
using Richasy.AgentKernel.Connectors.Ali.Models;
using Richasy.AgentKernel.Connectors.Anthropic.Models;
using Richasy.AgentKernel.Connectors.Azure.Models;
using Richasy.AgentKernel.Connectors.Baidu.Models;
using Richasy.AgentKernel.Connectors.Google.Models;
using Richasy.AgentKernel.Connectors.Groq.Models;
using Richasy.AgentKernel.Connectors.IFlyTek.Models;
using Richasy.AgentKernel.Connectors.LingYi.Models;
using Richasy.AgentKernel.Connectors.Mistral.Models;
using Richasy.AgentKernel.Connectors.Moonshot.Models;
using Richasy.AgentKernel.Connectors.Ollama.Models;
using Richasy.AgentKernel.Connectors.OpenAI.Models;
using Richasy.AgentKernel.Connectors.OpenRouter.Models;
using Richasy.AgentKernel.Connectors.SiliconFlow.Models;
using Richasy.AgentKernel.Connectors.Tencent.Models;
using Richasy.AgentKernel.Connectors.TogetherAI.Models;
using Richasy.AgentKernel.Connectors.Volcano.Models;
using Richasy.AgentKernel.Connectors.XAI.Models;
using Richasy.AgentKernel.Connectors.ZhiPu.Models;
using Richasy.AgentKernel.Models;
using RodelAgent.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace RodelAgent.UI.Extensions;

/// <summary>
/// Chat service configuration manager.
/// </summary>
internal sealed class ChatConfigManager : ChatConfigManagerBase
{
    /// <inheritdoc/>
    protected override AIServiceConfig? ConvertToConfig(ChatClientConfigBase? config)
    {
        return config switch
        {
            OpenAIChatConfig openAIConfig => openAIConfig.ToAIServiceConfig(),
            AzureOpenAIChatConfig azureOaiConfig => azureOaiConfig.ToAIServiceConfig<AzureOpenAIServiceConfig>(),
            AzureAIChatConfig azureConfig => azureConfig.ToAIServiceConfig<AzureOpenAIServiceConfig>(),
            XAIChatConfig xaiConfig => xaiConfig.ToAIServiceConfig<XAIServiceConfig>(),
            ZhiPuChatConfig zhiPuConfig => zhiPuConfig.ToAIServiceConfig<ZhiPuServiceConfig>(),
            LingYiChatConfig lingYiConfig => lingYiConfig.ToAIServiceConfig<LingYiServiceConfig>(),
            AnthropicChatConfig anthropicConfig => anthropicConfig.ToAIServiceConfig<AnthropicServiceConfig>(),
            MoonshotChatConfig moonshotConfig => moonshotConfig.ToAIServiceConfig<MoonshotServiceConfig>(),
            GeminiChatConfig geminiConfig => geminiConfig.ToAIServiceConfig<GeminiServiceConfig>(),
            DeepSeekChatConfig deepSeekConfig => deepSeekConfig.ToAIServiceConfig<DeepSeekServiceConfig>(),
            QwenChatConfig qwenConfig => qwenConfig.ToAIServiceConfig<QwenServiceConfig>(),
            ErnieChatConfig ernieConfig => ernieConfig.ToAIServiceConfig(),
            HunyuanChatConfig hunyuanConfig => hunyuanConfig.ToAIServiceConfig<HunyuanChatServiceConfig>(),
            SparkChatConfig sparkConfig => sparkConfig.ToAIServiceConfig<SparkChatServiceConfig>(),
            DoubaoChatConfig douBaoConfig => douBaoConfig.ToAIServiceConfig<DoubaoServiceConfig>(),
            SiliconFlowChatConfig siliconFlowConfig => siliconFlowConfig.ToAIServiceConfig<SiliconFlowServiceConfig>(),
            OpenRouterChatConfig openRouterConfig => openRouterConfig.ToAIServiceConfig<OpenRouterServiceConfig>(),
            TogetherAIChatConfig togetherAIConfig => togetherAIConfig.ToAIServiceConfig<TogetherAIServiceConfig>(),
            GroqChatConfig groqConfig => groqConfig.ToAIServiceConfig<GroqServiceConfig>(),
            MistralChatConfig mistralConfig => mistralConfig.ToAIServiceConfig(),
            OllamaChatConfig ollamaConfig => ollamaConfig.ToAIServiceConfig(),
            _ => default,
        };
    }

    /// <inheritdoc/>
    protected override async Task<ChatClientConfiguration> OnInitializeAsync()
    {
        var providers = Enum.GetValues<ChatProviderType>();
        var config = new ChatClientConfiguration();
        var storageService = this.Get<IStorageService>();
        foreach (var provider in providers)
        {
            switch (provider)
            {
                case ChatProviderType.OpenAI:
                    config.OpenAI = await storageService.GetChatConfigAsync(provider, JsonGenContext.Default.OpenAIChatConfig);
                    break;
                case ChatProviderType.AzureOpenAI:
                    config.AzureOpenAI = await storageService.GetChatConfigAsync(provider, JsonGenContext.Default.AzureOpenAIChatConfig);
                    break;
                case ChatProviderType.AzureAI:
                    config.AzureAI = await storageService.GetChatConfigAsync(provider, JsonGenContext.Default.AzureAIChatConfig);
                    break;
                case ChatProviderType.Gemini:
                    config.Gemini = await storageService.GetChatConfigAsync(provider, JsonGenContext.Default.GeminiChatConfig);
                    break;
                case ChatProviderType.Anthropic:
                    config.Anthropic = await storageService.GetChatConfigAsync(provider, JsonGenContext.Default.AnthropicChatConfig);
                    break;
                case ChatProviderType.Moonshot:
                    config.Moonshot = await storageService.GetChatConfigAsync(provider, JsonGenContext.Default.MoonshotChatConfig);
                    break;
                case ChatProviderType.ZhiPu:
                    config.ZhiPu = await storageService.GetChatConfigAsync(provider, JsonGenContext.Default.ZhiPuChatConfig);
                    break;
                case ChatProviderType.LingYi:
                    config.LingYi = await storageService.GetChatConfigAsync(provider, JsonGenContext.Default.LingYiChatConfig);
                    break;
                case ChatProviderType.DeepSeek:
                    config.DeepSeek = await storageService.GetChatConfigAsync(provider, JsonGenContext.Default.DeepSeekChatConfig);
                    break;
                case ChatProviderType.Qwen:
                    config.Qwen = await storageService.GetChatConfigAsync(provider, JsonGenContext.Default.QwenChatConfig);
                    break;
                case ChatProviderType.Ernie:
                    config.Ernie = await storageService.GetChatConfigAsync(provider, JsonGenContext.Default.ErnieChatConfig);
                    break;
                case ChatProviderType.Hunyuan:
                    config.Hunyuan = await storageService.GetChatConfigAsync(provider, JsonGenContext.Default.HunyuanChatConfig);
                    break;
                case ChatProviderType.Doubao:
                    config.Doubao = await storageService.GetChatConfigAsync(provider, JsonGenContext.Default.DoubaoChatConfig);
                    break;
                case ChatProviderType.Spark:
                    config.Spark = await storageService.GetChatConfigAsync(provider, JsonGenContext.Default.SparkChatConfig);
                    break;
                case ChatProviderType.OpenRouter:
                    config.OpenRouter = await storageService.GetChatConfigAsync(provider, JsonGenContext.Default.OpenRouterChatConfig);
                    break;
                case ChatProviderType.TogetherAI:
                    config.TogetherAI = await storageService.GetChatConfigAsync(provider, JsonGenContext.Default.TogetherAIChatConfig);
                    break;
                case ChatProviderType.Groq:
                    config.Groq = await storageService.GetChatConfigAsync(provider, JsonGenContext.Default.GroqChatConfig);
                    break;
                case ChatProviderType.Perplexity:
                    config.Perplexity = await storageService.GetChatConfigAsync(provider, JsonGenContext.Default.PerplexityChatConfig);
                    break;
                case ChatProviderType.Mistral:
                    config.Mistral = await storageService.GetChatConfigAsync(provider, JsonGenContext.Default.MistralChatConfig);
                    break;
                case ChatProviderType.SiliconFlow:
                    config.SiliconFlow = await storageService.GetChatConfigAsync(provider, JsonGenContext.Default.SiliconFlowChatConfig);
                    break;
                case ChatProviderType.Ollama:
                    config.Ollama = await storageService.GetChatConfigAsync(provider, JsonGenContext.Default.OllamaChatConfig);
                    break;
                case ChatProviderType.XAI:
                    config.XAI = await storageService.GetChatConfigAsync(provider, JsonGenContext.Default.XAIChatConfig);
                    break;
                default:
                    break;
            }
        }

        return config;
    }

    /// <inheritdoc/>
    protected override async Task OnSaveAsync(ChatClientConfiguration configuration)
    {
        var providers = Enum.GetValues<ChatProviderType>();
        var storageService = this.Get<IStorageService>();
        foreach (var provider in providers)
        {
            switch (provider)
            {
                case ChatProviderType.OpenAI:
                    await storageService.SetChatConfigAsync(provider, configuration.OpenAI ?? new(), JsonGenContext.Default.OpenAIChatConfig);
                    break;
                case ChatProviderType.AzureOpenAI:
                    await storageService.SetChatConfigAsync(provider, configuration.AzureOpenAI ?? new(), JsonGenContext.Default.AzureOpenAIChatConfig);
                    break;
                case ChatProviderType.AzureAI:
                    await storageService.SetChatConfigAsync(provider, configuration.AzureAI ?? new(), JsonGenContext.Default.AzureAIChatConfig);
                    break;
                case ChatProviderType.Gemini:
                    await storageService.SetChatConfigAsync(provider, configuration.Gemini ?? new(), JsonGenContext.Default.GeminiChatConfig);
                    break;
                case ChatProviderType.Anthropic:
                    await storageService.SetChatConfigAsync(provider, configuration.Anthropic ?? new(), JsonGenContext.Default.AnthropicChatConfig);
                    break;
                case ChatProviderType.Moonshot:
                    await storageService.SetChatConfigAsync(provider, configuration.Moonshot ?? new(), JsonGenContext.Default.MoonshotChatConfig);
                    break;
                case ChatProviderType.ZhiPu:
                    await storageService.SetChatConfigAsync(provider, configuration.ZhiPu ?? new(), JsonGenContext.Default.ZhiPuChatConfig);
                    break;
                case ChatProviderType.LingYi:
                    await storageService.SetChatConfigAsync(provider, configuration.LingYi ?? new(), JsonGenContext.Default.LingYiChatConfig);
                    break;
                case ChatProviderType.DeepSeek:
                    await storageService.SetChatConfigAsync(provider, configuration.DeepSeek ?? new(), JsonGenContext.Default.DeepSeekChatConfig);
                    break;
                case ChatProviderType.Qwen:
                    await storageService.SetChatConfigAsync(provider, configuration.Qwen ?? new(), JsonGenContext.Default.QwenChatConfig);
                    break;
                case ChatProviderType.Ernie:
                    await storageService.SetChatConfigAsync(provider, configuration.Ernie ?? new(), JsonGenContext.Default.ErnieChatConfig);
                    break;
                case ChatProviderType.Hunyuan:
                    await storageService.SetChatConfigAsync(provider, configuration.Hunyuan ?? new(), JsonGenContext.Default.HunyuanChatConfig);
                    break;
                case ChatProviderType.Doubao:
                    await storageService.SetChatConfigAsync(provider, configuration.Doubao ?? new(), JsonGenContext.Default.DoubaoChatConfig);
                    break;
                case ChatProviderType.Spark:
                    await storageService.SetChatConfigAsync(provider, configuration.Spark ?? new(), JsonGenContext.Default.SparkChatConfig);
                    break;
                case ChatProviderType.OpenRouter:
                    await storageService.SetChatConfigAsync(provider, configuration.OpenRouter ?? new(), JsonGenContext.Default.OpenRouterChatConfig);
                    break;
                case ChatProviderType.TogetherAI:
                    await storageService.SetChatConfigAsync(provider, configuration.TogetherAI ?? new(), JsonGenContext.Default.TogetherAIChatConfig);
                    break;
                case ChatProviderType.Groq:
                    await storageService.SetChatConfigAsync(provider, configuration.Groq ?? new(), JsonGenContext.Default.GroqChatConfig);
                    break;
                case ChatProviderType.Perplexity:
                    await storageService.SetChatConfigAsync(provider, configuration.Perplexity ?? new(), JsonGenContext.Default.PerplexityChatConfig);
                    break;
                case ChatProviderType.Mistral:
                    await storageService.SetChatConfigAsync(provider, configuration.Mistral ?? new(), JsonGenContext.Default.MistralChatConfig);
                    break;
                case ChatProviderType.SiliconFlow:
                    await storageService.SetChatConfigAsync(provider, configuration.SiliconFlow ?? new(), JsonGenContext.Default.SiliconFlowChatConfig);
                    break;
                case ChatProviderType.Ollama:
                    await storageService.SetChatConfigAsync(provider, configuration.Ollama ?? new(), JsonGenContext.Default.OllamaChatConfig);
                    break;
                case ChatProviderType.XAI:
                    await storageService.SetChatConfigAsync(provider, configuration.XAI ?? new(), JsonGenContext.Default.XAIChatConfig);
                    break;
                default:
                    break;
            }
        }
    }
}

internal static partial class ConfigExtensions
{
    public static AIServiceConfig? ToAIServiceConfig(this OpenAIChatConfig? config)
    {
        var endpoint = string.IsNullOrEmpty(config?.Endpoint) ? null : new Uri(config.Endpoint);
        return config is null || string.IsNullOrWhiteSpace(config.Key)
            ? default
            : new OpenAIServiceConfig(config.Key, string.Empty, endpoint, config.OrganizationId);
    }

    public static AIServiceConfig? ToAIServiceConfig<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TAIServiceConfig>(this ChatEndpointConfigBase? config)
        where TAIServiceConfig : AIServiceConfig
    {
        return config is null || string.IsNullOrWhiteSpace(config.Key)
            ? default
            : Activator.CreateInstance(typeof(TAIServiceConfig), config.Key, string.Empty, string.IsNullOrEmpty(config.Endpoint) ? default : new Uri(config.Endpoint)) as TAIServiceConfig;
    }

    public static AIServiceConfig? ToAIServiceConfig<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TAIServiceConfig>(this ChatClientConfigBase? config)
        where TAIServiceConfig : AIServiceConfig
    {
        return config is null || string.IsNullOrWhiteSpace(config.Key)
            ? default
            : Activator.CreateInstance(typeof(TAIServiceConfig), config.Key, string.Empty) as TAIServiceConfig;
    }

    public static AIServiceConfig? ToAIServiceConfig(this OllamaChatConfig? config)
    {
        return config is null || string.IsNullOrWhiteSpace(config.Endpoint)
             ? default
             : new OllamaServiceConfig(string.Empty, new Uri(config.Endpoint));
    }

    public static AIServiceConfig? ToAIServiceConfig(this ErnieChatConfig? config)
    {
        return config is null || string.IsNullOrWhiteSpace(config.Key)
            ? default
            : new ErnieServiceConfig(config.Key, config.Secret!, string.Empty);
    }

    public static AIServiceConfig? ToAIServiceConfig(this MistralChatConfig? config)
    {
        return config is null || string.IsNullOrWhiteSpace(config.Key)
            ? default
            : new MistralServiceConfig(config.UseCodestral ? config.CodestralKey! : config.Key!, string.Empty, config.UseCodestral);
    }
}
