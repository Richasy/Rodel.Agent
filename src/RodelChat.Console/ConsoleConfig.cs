// Copyright (c) Rodel. All rights reserved.

using RodelChat.Core.Models.Chat;

/// <summary>
/// 控制台配置.
/// </summary>
public sealed class ConsoleConfig
{
    /// <summary>
    /// Open AI 服务配置.
    /// </summary>
    public OpenAIServiceConfig OpenAI { get; set; }

    /// <summary>
    /// Azure Open AI 服务配置.
    /// </summary>
    public AzureOpenAIServiceConfig AzureOpenAI { get; set; }

    /// <summary>
    /// 智谱服务配置.
    /// </summary>
    public ServiceConfigBase Zhipu { get; set; }

    /// <summary>
    /// 零一万物服务配置.
    /// </summary>
    public ServiceConfigBase LingYi { get; set; }

    /// <summary>
    /// 月之暗面服务配置.
    /// </summary>
    public ServiceConfigBase Moonshot { get; set; }

    /// <summary>
    /// 阿里灵积服务配置.
    /// </summary>
    public ServiceConfigBase DashScope { get; set; }

    /// <summary>
    /// 千帆服务配置.
    /// </summary>
    public QianFanServiceConfig QianFan { get; set; }

    /// <summary>
    /// 讯飞星火服务配置.
    /// </summary>
    public SparkDeskServiceConfig SparkDesk { get; set; }

    /// <summary>
    /// Gemini 服务配置.
    /// </summary>
    public ServiceConfigBase Gemini { get; set; }

    /// <summary>
    /// Groq 服务配置.
    /// </summary>
    public ServiceConfigBase Groq { get; set; }

    /// <summary>
    /// Mistral AI 服务配置.
    /// </summary>
    public ServiceConfigBase MistralAI { get; set; }

    /// <summary>
    /// Perplexity 服务配置.
    /// </summary>
    public ServiceConfigBase Perplexity { get; set; }

    /// <summary>
    /// Together AI 服务配置.
    /// </summary>
    public ServiceConfigBase TogetherAI { get; set; }

    /// <summary>
    /// Open Router 服务配置.
    /// </summary>
    public ServiceConfigBase OpenRouter { get; set; }

    /// <summary>
    /// Anthropic 服务配置.
    /// </summary>
    public OpenAIServiceConfig Anthropic { get; set; }

    /// <summary>
    /// Ollama 服务配置.
    /// </summary>
    public OpenAIServiceConfig Ollama { get; set; }
}

/// <summary>
/// Open AI 服务配置.
/// </summary>
public sealed class OpenAIServiceConfig : ServiceConfigBase
{
    /// <summary>
    /// 代理地址.
    /// </summary>
    public string ProxyUrl { get; set; }
}

/// <summary>
/// Azure Open AI 服务配置.
/// </summary>
public sealed class AzureOpenAIServiceConfig : ServiceConfigBase
{
    /// <summary>
    /// 终结点.
    /// </summary>
    public string Endpoint { get; set; }
}

/// <summary>
/// 千帆服务配置.
/// </summary>
public sealed class QianFanServiceConfig : ServiceConfigBase
{
    /// <summary>
    /// 密匙.
    /// </summary>
    public string Secret { get; set; }
}

/// <summary>
/// 讯飞星火服务配置.
/// </summary>
public sealed class SparkDeskServiceConfig : ServiceConfigBase
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
/// 服务配置基类.
/// </summary>
public class ServiceConfigBase
{
    /// <summary>
    /// 访问密钥.
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    /// 默认模型标识符.
    /// </summary>
    public string DefaultModelId { get; set; }

    /// <summary>
    /// 自定义模型列表.
    /// </summary>
    public List<ChatModel> CustomModels { get; set; }
}
