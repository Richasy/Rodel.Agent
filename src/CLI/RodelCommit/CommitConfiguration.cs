// Copyright (c) Richasy. All rights reserved.

using Richasy.AgentKernel;
using System.Text.Json.Serialization;

namespace RodelCommit;

internal sealed class CommitConfiguration
{
    [JsonPropertyName("app")]
    public BasicConfiguration App { get; set; }

    [JsonPropertyName("services")]
    public ServiceConfiguration Services { get; set; }
}

internal sealed class BasicConfiguration
{
    [JsonPropertyName("default_service")]
    public string? DefaultService { get; set; }

    [JsonPropertyName("diff_chunk_size")]
    public int DiffChunkSize { get; set; }

    [JsonPropertyName("max_commit_length")]
    public int MaxCommitLength { get; set; }

    [JsonPropertyName("locale")]
    public string? Locale { get; set; }

    [JsonPropertyName("use_gitmoji")]
    public bool UseGitmoji { get; set; }
}

internal sealed class ServiceConfiguration
{
    [JsonPropertyName("openai")]
    public OpenAIConfig? OpenAI { get; set; }

    [JsonPropertyName("azure_openai")]
    public AzureOpenAIConfig? AzureOpenAI { get; set; }

    [JsonPropertyName("azure_ai")]
    public AzureAIConfig? AzureAI { get; set; }

    [JsonPropertyName("ollama")]
    public OllamaConfig? Ollama { get; set; }

    [JsonPropertyName("gemini")]
    public GeminiConfig? Gemini { get; set; }

    [JsonPropertyName("anthropic")]
    public AnthropicConfig? Anthropic { get; set; }

    [JsonPropertyName("moonshot")]
    public MoonshotConfig? Moonshot { get; set; }

    [JsonPropertyName("zhipu")]
    public ZhiPuConfig? ZhiPu { get; set; }

    [JsonPropertyName("lingyi")]
    public LingYiConfig? LingYi { get; set; }

    [JsonPropertyName("deepseek")]
    public DeepSeekConfig? DeepSeek { get; set; }

    [JsonPropertyName("qwen")]
    public QwenConfig? Qwen { get; set; }

    [JsonPropertyName("ernie")]
    public ErnieConfig? Ernie { get; set; }

    [JsonPropertyName("hunyuan")]
    public HunyuanConfig? Hunyuan { get; set; }

    [JsonPropertyName("spark")]
    public SparkConfig? Spark { get; set; }

    [JsonPropertyName("open_router")]
    public OpenRouterConfig? OpenRouter { get; set; }

    [JsonPropertyName("together_ai")]
    public TogetherAIConfig? TogetherAI { get; set; }

    [JsonPropertyName("groq")]
    public GroqConfig? Groq { get; set; }

    [JsonPropertyName("perplexity")]
    public PerplexityConfig? Perplexity { get; set; }

    [JsonPropertyName("mistral")]
    public MistralConfig? Mistral { get; set; }

    [JsonPropertyName("siliconflow")]
    public SiliconFlowConfig? SiliconFlow { get; set; }

    [JsonPropertyName("doubao")]
    public DoubaoConfig? Doubao { get; set; }

    [JsonPropertyName("xai")]
    public XAIConfig? XAI { get; set; }
}

internal sealed class OpenAIConfig : OpenAIChatConfig
{
    [JsonPropertyName("model")]
    public string? Model { get; set; }

    public override bool IsValid()
        => base.IsValid() && !string.IsNullOrEmpty(Model);
}

internal sealed class AzureOpenAIConfig : AzureOpenAIChatConfig
{
    [JsonPropertyName("model")]
    public string? Model { get; set; }

    public override bool IsValid()
        => !string.IsNullOrEmpty(Key) && !string.IsNullOrEmpty(Endpoint) && !string.IsNullOrEmpty(Model);
}

internal sealed class AzureAIConfig : AzureAIChatConfig
{
    [JsonPropertyName("model")]
    public string? Model { get; set; }

    public override bool IsValid()
        => !string.IsNullOrEmpty(Key) && !string.IsNullOrEmpty(Endpoint) && !string.IsNullOrEmpty(Model);
}

internal sealed class OllamaConfig : OllamaChatConfig
{
    [JsonPropertyName("model")]
    public string? Model { get; set; }

    public override bool IsValid()
        => base.IsValid() && !string.IsNullOrEmpty(Model);
}

internal sealed class GeminiConfig : GeminiChatConfig
{
    [JsonPropertyName("model")]
    public string? Model { get; set; }

    public override bool IsValid()
        => base.IsValid() && !string.IsNullOrEmpty(Model);
}

internal sealed class AnthropicConfig : AnthropicChatConfig
{
    [JsonPropertyName("model")]
    public string? Model { get; set; }

    public override bool IsValid()
        => base.IsValid() && !string.IsNullOrEmpty(Model);
}

internal sealed class MoonshotConfig : MoonshotChatConfig
{
    [JsonPropertyName("model")]
    public string? Model { get; set; }

    public override bool IsValid()
        => base.IsValid() && !string.IsNullOrEmpty(Model);
}

internal sealed class ZhiPuConfig : ZhiPuChatConfig
{
    [JsonPropertyName("model")]
    public string? Model { get; set; }

    public override bool IsValid()
        => base.IsValid() && !string.IsNullOrEmpty(Model);
}

internal sealed class LingYiConfig : LingYiChatConfig
{
    [JsonPropertyName("model")]
    public string? Model { get; set; }

    public override bool IsValid()
        => base.IsValid() && !string.IsNullOrEmpty(Model);
}

internal sealed class DeepSeekConfig : DeepSeekChatConfig
{
    [JsonPropertyName("model")]
    public string? Model { get; set; }

    public override bool IsValid()
        => base.IsValid() && !string.IsNullOrEmpty(Model);
}

internal sealed class QwenConfig : QwenChatConfig
{
    [JsonPropertyName("model")]
    public string? Model { get; set; }

    public override bool IsValid()
        => base.IsValid() && !string.IsNullOrEmpty(Model);
}

internal sealed class ErnieConfig : ErnieChatConfig
{
    [JsonPropertyName("model")]
    public string? Model { get; set; }

    public override bool IsValid()
        => base.IsValid() && !string.IsNullOrEmpty(Model);
}

internal sealed class HunyuanConfig : HunyuanChatConfig
{
    [JsonPropertyName("model")]
    public string? Model { get; set; }

    public override bool IsValid()
        => base.IsValid() && !string.IsNullOrEmpty(Model);
}

internal sealed class SparkConfig : SparkChatConfig
{
    [JsonPropertyName("model")]
    public string? Model { get; set; }

    public override bool IsValid()
        => base.IsValid() && !string.IsNullOrEmpty(Model);
}

internal sealed class OpenRouterConfig : OpenRouterChatConfig
{
    [JsonPropertyName("model")]
    public string? Model { get; set; }

    public override bool IsValid()
        => base.IsValid() && !string.IsNullOrEmpty(Model);
}

internal sealed class TogetherAIConfig : TogetherAIChatConfig
{
    [JsonPropertyName("model")]
    public string? Model { get; set; }

    public override bool IsValid()
        => base.IsValid() && !string.IsNullOrEmpty(Model);
}

internal sealed class GroqConfig : GroqChatConfig
{
    [JsonPropertyName("model")]
    public string? Model { get; set; }

    public override bool IsValid()
        => base.IsValid() && !string.IsNullOrEmpty(Model);
}

internal sealed class PerplexityConfig : PerplexityChatConfig
{
    [JsonPropertyName("model")]
    public string? Model { get; set; }

    public override bool IsValid()
        => base.IsValid() && !string.IsNullOrEmpty(Model);
}

internal sealed class MistralConfig : MistralChatConfig
{
    [JsonPropertyName("model")]
    public string? Model { get; set; }

    public override bool IsValid()
        => UseCodestral ? base.IsValid() : base.IsValid() && !string.IsNullOrEmpty(Model);
}

internal sealed class SiliconFlowConfig : SiliconFlowChatConfig
{
    [JsonPropertyName("model")]
    public string? Model { get; set; }

    public override bool IsValid()
        => base.IsValid() && !string.IsNullOrEmpty(Model);
}

internal sealed class DoubaoConfig : DoubaoChatConfig
{
    [JsonPropertyName("model")]
    public string? Model { get; set; }

    public override bool IsValid()
        => base.IsValid() && !string.IsNullOrEmpty(Model);
}

internal sealed class XAIConfig : XAIChatConfig
{
    [JsonPropertyName("model")]
    public string? Model { get; set; }

    public override bool IsValid()
        => base.IsValid() && !string.IsNullOrEmpty(Model);
}