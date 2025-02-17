// Copyright (c) Richasy. All rights reserved.

using System.Text.Json.Serialization;

namespace RodelAudio.Models.Client;

/// <summary>
/// 音频客户端配置.
/// </summary>
public sealed class AudioClientConfiguration
{
    /// <summary>
    /// Open AI 客户端配置.
    /// </summary>
    [JsonPropertyName("openai")]
    public OpenAIClientConfig? OpenAI { get; set; }

    /// <summary>
    /// Azure Open AI 客户端配置.
    /// </summary>
    [JsonPropertyName("azure_openai")]
    public AzureOpenAIClientConfig? AzureOpenAI { get; set; }

    /// <summary>
    /// Azure 语音客户端配置.
    /// </summary>
    [JsonPropertyName("azure_speech")]
    public AzureSpeechClientConfig? AzureSpeech { get; set; }
}

/// <summary>
/// Open AI 客户端配置.
/// </summary>
public class OpenAIClientConfig : ClientEndpointConfigBase
{
    /// <summary>
    /// 组织 ID.
    /// </summary>
    [JsonPropertyName("organization")]
    public string? OrganizationId { get; set; }
}

/// <summary>
/// Azure Open AI 客户端配置.
/// </summary>
public class AzureOpenAIClientConfig : ClientEndpointConfigBase
{
    /// <inheritdoc/>
    public override bool IsValid()
    {
        return base.IsValid()
            && !string.IsNullOrEmpty(Endpoint);
    }
}

/// <summary>
/// Azure 语音客户端配置.
/// </summary>
public class AzureSpeechClientConfig : ClientConfigBase
{
    /// <summary>
    /// 地区.
    /// </summary>
    [JsonPropertyName("region")]
    public string Region { get; set; }

    /// <inheritdoc/>
    public override bool IsValid()
        => base.IsValid() && !string.IsNullOrEmpty(Region);
}

/// <summary>
/// 配置基类.
/// </summary>
public abstract class ConfigBase
{
    /// <summary>
    /// 自定义模型列表.
    /// </summary>
    [JsonPropertyName("models")]
    public List<AudioModel>? CustomModels { get; set; }

    /// <summary>
    /// 自定义模型是否不为空.
    /// </summary>
    /// <returns>是否不为空.</returns>
    public bool IsCustomModelNotEmpty()
        => CustomModels != null && CustomModels.Count > 0;
}

/// <summary>
/// 客户端配置基类.
/// </summary>
public abstract class ClientConfigBase : ConfigBase
{
    /// <summary>
    /// 访问密钥.
    /// </summary>
    [JsonPropertyName("key")]
    public string? Key { get; set; }

    /// <summary>
    /// 是否有效.
    /// </summary>
    /// <returns>配置是否有效.</returns>
    public virtual bool IsValid()
        => !string.IsNullOrEmpty(Key);
}

/// <summary>
/// 客户端终结点配置基类.
/// </summary>
public abstract class ClientEndpointConfigBase : ClientConfigBase
{
    /// <summary>
    /// 终结点.
    /// </summary>
    [JsonPropertyName("endpoint")]
    public string? Endpoint { get; set; }
}

