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
using Richasy.AgentKernel.Connectors.Onnx.Models;
using Richasy.AgentKernel.Connectors.OpenAI.Models;
using Richasy.AgentKernel.Connectors.OpenRouter.Models;
using Richasy.AgentKernel.Connectors.SiliconFlow.Models;
using Richasy.AgentKernel.Connectors.Tencent.Models;
using Richasy.AgentKernel.Connectors.TogetherAI.Models;
using Richasy.AgentKernel.Connectors.Volcano.Models;
using Richasy.AgentKernel.Connectors.XAI.Models;
using Richasy.AgentKernel.Connectors.ZhiPu.Models;
using Richasy.AgentKernel.Models;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace RodelCommit;

/// <summary>
/// The configuration manager for chat services.
/// </summary>
internal sealed class ChatConfigManager : IChatConfigManager
{
    public static CommitConfiguration AppConfiguration { get; private set; }

    public static bool ShouldManual { get; set; }

    /// <inheritdoc/>
    public ChatClientConfiguration? Configuration { get; private set; }

    public async Task<ChatClientConfigBase?> GetChatConfigAsync(ChatProviderType provider)
    {
        await InitializeAsync().ConfigureAwait(false);
        return provider switch
        {
            ChatProviderType.Ollama => AppConfiguration?.Services.Ollama,
            ChatProviderType.OpenAI => AppConfiguration?.Services.OpenAI,
            ChatProviderType.AzureOpenAI => AppConfiguration?.Services.AzureOpenAI,
            ChatProviderType.AzureAI => AppConfiguration?.Services.AzureAI,
            ChatProviderType.Gemini => AppConfiguration?.Services.Gemini,
            ChatProviderType.Anthropic => AppConfiguration?.Services.Anthropic,
            ChatProviderType.Moonshot => AppConfiguration?.Services.Moonshot,
            ChatProviderType.ZhiPu => AppConfiguration?.Services.ZhiPu,
            ChatProviderType.LingYi => AppConfiguration?.Services.LingYi,
            ChatProviderType.DeepSeek => AppConfiguration?.Services.DeepSeek,
            ChatProviderType.Qwen => AppConfiguration?.Services.Qwen,
            ChatProviderType.Ernie => AppConfiguration?.Services.Ernie,
            ChatProviderType.Hunyuan => AppConfiguration?.Services.Hunyuan,
            ChatProviderType.Spark => AppConfiguration?.Services.Spark,
            ChatProviderType.OpenRouter => AppConfiguration?.Services.OpenRouter,
            ChatProviderType.TogetherAI => AppConfiguration?.Services.TogetherAI,
            ChatProviderType.Groq => AppConfiguration?.Services.Groq,
            ChatProviderType.Perplexity => AppConfiguration?.Services.Perplexity,
            ChatProviderType.Mistral => AppConfiguration?.Services.Mistral,
            ChatProviderType.SiliconFlow => AppConfiguration?.Services.SiliconFlow,
            ChatProviderType.Doubao => AppConfiguration?.Services.Doubao,
            ChatProviderType.XAI => AppConfiguration?.Services.XAI,
            ChatProviderType.Onnx => AppConfiguration?.Services.Onnx,
            _ => null,
        };
    }

    /// <inheritdoc/>
    public async Task<AIServiceConfig?> GetServiceConfigAsync(ChatProviderType provider, ChatModel model)
    {
        var config = await GetChatConfigAsync(provider).ConfigureAwait(false);
        var aiConfig = config switch
        {
            OpenAIConfig openAIConfig => openAIConfig.ToAIServiceConfig(),
            AzureOpenAIConfig azureOaiConfig => azureOaiConfig.ToAIServiceConfig<AzureOpenAIServiceConfig>(azureOaiConfig.Model),
            AzureAIConfig azureConfig => azureConfig.ToAIServiceConfig<AzureOpenAIServiceConfig>(azureConfig.Model),
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
            OnnxChatConfig onnxConfig => onnxConfig.ToAIServiceConfig(),
            _ => default,
        };
        if (aiConfig is not null)
        {
            aiConfig.Model = model.Id;
        }

        return aiConfig;
    }

    /// <inheritdoc/>
    public Task SaveChatConfigAsync(Dictionary<ChatProviderType, ChatClientConfigBase> configMap)
        => throw new NotImplementedException();

    public static ChatModel GetModel(ChatProviderType provider)
    {
        var modelId = provider switch
        {
            ChatProviderType.Ollama => AppConfiguration?.Services.Ollama!.Model,
            ChatProviderType.OpenAI => AppConfiguration?.Services.OpenAI!.Model,
            ChatProviderType.AzureOpenAI => AppConfiguration?.Services.AzureOpenAI!.Model,
            ChatProviderType.AzureAI => AppConfiguration?.Services.AzureAI!.Model,
            ChatProviderType.Gemini => AppConfiguration?.Services.Gemini!.Model,
            ChatProviderType.Anthropic => AppConfiguration?.Services.Anthropic!.Model,
            ChatProviderType.Moonshot => AppConfiguration?.Services.Moonshot!.Model,
            ChatProviderType.ZhiPu => AppConfiguration?.Services.ZhiPu!.Model,
            ChatProviderType.LingYi => AppConfiguration?.Services.LingYi!.Model,
            ChatProviderType.DeepSeek => AppConfiguration?.Services.DeepSeek!.Model,
            ChatProviderType.Qwen => AppConfiguration?.Services.Qwen!.Model,
            ChatProviderType.Ernie => AppConfiguration?.Services.Ernie!.Model,
            ChatProviderType.Hunyuan => AppConfiguration?.Services.Hunyuan!.Model,
            ChatProviderType.Spark => AppConfiguration?.Services.Spark!.Model,
            ChatProviderType.OpenRouter => AppConfiguration?.Services.OpenRouter!.Model,
            ChatProviderType.TogetherAI => AppConfiguration?.Services.TogetherAI!.Model,
            ChatProviderType.Groq => AppConfiguration?.Services.Groq!.Model,
            ChatProviderType.Perplexity => AppConfiguration?.Services.Perplexity!.Model,
            ChatProviderType.Mistral => AppConfiguration?.Services.Mistral!.Model,
            ChatProviderType.SiliconFlow => AppConfiguration?.Services.SiliconFlow!.Model,
            ChatProviderType.Doubao => AppConfiguration?.Services.Doubao!.Model,
            ChatProviderType.XAI => AppConfiguration?.Services.XAI!.Model,
            ChatProviderType.Onnx => AppConfiguration?.Services.Onnx!.Model,
            _ => null,
        };

        return new(modelId!, modelId!);
    }

    public static async Task InitializeAsync(bool force = false)
    {
        if (AppConfiguration is not null && !force)
        {
            return;
        }

        var directory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var configDirectory = Path.Combine(directory, ".rodel-commit");
        if (!Directory.Exists(configDirectory))
        {
            Directory.CreateDirectory(configDirectory);
        }
        var configPath = Path.Combine(configDirectory, "config.json");
        if (!File.Exists(configPath))
        {
            var examplePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.example.json");
            File.Copy(examplePath, configPath);
        }

        var content = await File.ReadAllTextAsync(configPath).ConfigureAwait(false);
        AppConfiguration = JsonSerializer.Deserialize(content, JsonGenContext.Default.CommitConfiguration)!;
    }
}

internal static class ConfigExtensions
{
    public static AIServiceConfig ToAIServiceConfig(this OpenAIChatConfig? config)
    {
        var endpoint = string.IsNullOrEmpty(config?.Endpoint) ? null : new Uri(config.Endpoint);
        return config is null || string.IsNullOrWhiteSpace(config.Key)
            ? throw new ArgumentException("The configuration is not valid.", nameof(config))
            : new OpenAIServiceConfig(config.Key, string.Empty, endpoint, config.OrganizationId);
    }

    public static AIServiceConfig? ToAIServiceConfig<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TAIServiceConfig>(this ChatEndpointConfigBase? config, string? model = null)
        where TAIServiceConfig : AIServiceConfig
    {
        return config is null || string.IsNullOrWhiteSpace(config.Key)
            ? throw new ArgumentException("The configuration is not valid.", nameof(config))
            : Activator.CreateInstance(typeof(TAIServiceConfig), config.Key, model, string.IsNullOrEmpty(config.Endpoint) ? default : new Uri(config.Endpoint)) as TAIServiceConfig;
    }

    public static AIServiceConfig? ToAIServiceConfig<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TAIServiceConfig>(this ChatClientConfigBase? config, string? model = null)
        where TAIServiceConfig : AIServiceConfig
    {
        return config is null || string.IsNullOrWhiteSpace(config.Key)
            ? throw new ArgumentException("The configuration is not valid.", nameof(config))
            : Activator.CreateInstance(typeof(TAIServiceConfig), config.Key, model) as TAIServiceConfig;
    }

    public static AIServiceConfig? ToAIServiceConfig(this OllamaChatConfig? config)
    {
        return config is null || string.IsNullOrWhiteSpace(config.Endpoint)
             ? throw new ArgumentException("The configuration is not valid.", nameof(config))
             : new OllamaServiceConfig(string.Empty, new Uri(config.Endpoint));
    }

    public static AIServiceConfig? ToAIServiceConfig(this ErnieChatConfig? config)
    {
        return config is null || string.IsNullOrWhiteSpace(config.Key)
            ? throw new ArgumentException("The configuration is not valid.", nameof(config))
            : new ErnieServiceConfig(config.Key, config.Secret!, string.Empty);
    }

    public static AIServiceConfig? ToAIServiceConfig(this MistralChatConfig? config)
    {
        return config is null || string.IsNullOrWhiteSpace(config.Key)
            ? throw new ArgumentException("The configuration is not valid.", nameof(config))
            : new MistralServiceConfig(config.UseCodestral ? config.CodestralKey! : config.Key!, string.Empty, config.UseCodestral);
    }

    public static AIServiceConfig? ToAIServiceConfig(this OnnxChatConfig? config)
    {
        return config is null
            ? throw new ArgumentException("The configuration is not valid.", nameof(config))
            : new OnnxServiceConfig(string.Empty, config.UseCuda);
    }
}