// Copyright (c) Rodel. All rights reserved.

using System.Collections.Generic;
using RodelChat.Models.Chat;
using RodelChat.Models.Constants;

namespace RodelChat.Models.Client;

/// <summary>
/// 聊天客户端配置.
/// </summary>
public sealed class ChatClientConfiguration
{
    /// <summary>
    /// Open AI 客户端配置.
    /// </summary>
    public OpenAIClientConfig? OpenAI { get; set; }

    /// <summary>
    /// Azure Open AI 客户端配置.
    /// </summary>
    public AzureOpenAIClientConfig? AzureOpenAI { get; set; }

    /// <summary>
    /// 智谱客户端配置.
    /// </summary>
    public ZhiPuClientConfig? ZhiPu { get; set; }

    /// <summary>
    /// 零一万物客户端配置.
    /// </summary>
    public LingYiClientConfig? LingYi { get; set; }

    /// <summary>
    /// 月之暗面客户端配置.
    /// </summary>
    public MoonshotClientConfig? Moonshot { get; set; }

    /// <summary>
    /// 阿里灵积客户端配置.
    /// </summary>
    public DashScopeClientConfig? DashScope { get; set; }

    /// <summary>
    /// 千帆客户端配置.
    /// </summary>
    public QianFanClientConfig? QianFan { get; set; }

    /// <summary>
    /// 讯飞星火客户端配置.
    /// </summary>
    public SparkDeskClientConfig? SparkDesk { get; set; }

    /// <summary>
    /// Gemini 客户端配置.
    /// </summary>
    public GeminiClientConfig? Gemini { get; set; }

    /// <summary>
    /// Groq 客户端配置.
    /// </summary>
    public GroqClientConfig? Groq { get; set; }

    /// <summary>
    /// Mistral AI 客户端配置.
    /// </summary>
    public MistralAIClientConfig? MistralAI { get; set; }

    /// <summary>
    /// Perplexity 客户端配置.
    /// </summary>
    public PerplexityClientConfig? Perplexity { get; set; }

    /// <summary>
    /// Together AI 客户端配置.
    /// </summary>
    public TogetherAIClientConfig? TogetherAI { get; set; }

    /// <summary>
    /// Open Router 客户端配置.
    /// </summary>
    public OpenRouterClientConfig? OpenRouter { get; set; }

    /// <summary>
    /// Anthropic 客户端配置.
    /// </summary>
    public AnthropicClientConfig? Anthropic { get; set; }

    /// <summary>
    /// Ollama 客户端配置.
    /// </summary>
    public OllamaClientConfig? Ollama { get; set; }
}

/// <summary>
/// Open AI 客户端配置.
/// </summary>
public class OpenAIClientConfig : ClientEndpointConfigBase
{
    /// <summary>
    /// 组织 ID.
    /// </summary>
    public string? OrganizationId { get; set; }
}

/// <summary>
/// Azure Open AI 客户端配置.
/// </summary>
public class AzureOpenAIClientConfig : ClientEndpointConfigBase
{
    /// <summary>
    /// 版本.
    /// </summary>
    public AzureOpenAIVersion? Version { get; set; }
}

/// <summary>
/// 智谱客户端配置.
/// </summary>
public sealed class ZhiPuClientConfig : ClientConfigBase
{
}

/// <summary>
/// 零一万物客户端配置.
/// </summary>
public sealed class LingYiClientConfig : ClientConfigBase
{
}

/// <summary>
/// 月之暗面客户端配置.
/// </summary>
public sealed class MoonshotClientConfig : ClientConfigBase
{
}

/// <summary>
/// 阿里灵积客户端配置.
/// </summary>
public sealed class DashScopeClientConfig : ClientConfigBase
{
}

/// <summary>
/// Gemini 客户端配置.
/// </summary>
public sealed class GeminiClientConfig : ClientConfigBase
{
}

/// <summary>
/// Groq 客户端配置.
/// </summary>
public sealed class GroqClientConfig : ClientConfigBase
{
}

/// <summary>
/// Mistral AI 客户端配置.
/// </summary>
public sealed class MistralAIClientConfig : ClientConfigBase
{
}

/// <summary>
/// Perplexity 客户端配置.
/// </summary>
public sealed class PerplexityClientConfig : ClientConfigBase
{
}

/// <summary>
/// Together AI 客户端配置.
/// </summary>
public sealed class TogetherAIClientConfig : ClientConfigBase
{
}

/// <summary>
/// Open Router 客户端配置.
/// </summary>
public sealed class OpenRouterClientConfig : ClientConfigBase
{
}

/// <summary>
/// Anthropic 客户端配置.
/// </summary>
public sealed class AnthropicClientConfig : ClientEndpointConfigBase
{
}

/// <summary>
/// Ollama 客户端配置.
/// </summary>
public sealed class OllamaClientConfig : ClientEndpointConfigBase
{
}

/// <summary>
/// 千帆客户端配置.
/// </summary>
public sealed class QianFanClientConfig : ClientConfigBase
{
    /// <summary>
    /// 密匙.
    /// </summary>
    public string Secret { get; set; }
}

/// <summary>
/// 讯飞星火服务配置.
/// </summary>
public sealed class SparkDeskClientConfig : ClientConfigBase
{
    /// <summary>
    /// 应用程序 ID.
    /// </summary>
    public string AppId { get; set; }

    /// <summary>
    /// 密钥.
    /// </summary>
    public string Secret { get; set; }
}

/// <summary>
/// 客户端配置基类.
/// </summary>
public abstract class ClientConfigBase
{
    /// <summary>
    /// 访问密钥.
    /// </summary>
    public string? Key { get; set; }

    /// <summary>
    /// 自定义模型列表.
    /// </summary>
    public List<ChatModel>? CustomModels { get; set; }

    /// <summary>
    /// 自定义模型是否不为空.
    /// </summary>
    /// <returns>是否不为空.</returns>
    public bool IsCustomModelNotEmpty()
        => CustomModels != null && CustomModels.Count > 0;
}

/// <summary>
/// 客户端终结点配置基类.
/// </summary>
public abstract class ClientEndpointConfigBase : ClientConfigBase
{
    /// <summary>
    /// 终结点.
    /// </summary>
    public string? Endpoint { get; set; }
}
