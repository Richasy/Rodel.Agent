// Copyright (c) Rodel. All rights reserved.

using System.Collections.Generic;
using System.Text.Json.Serialization;
using RodelAgent.Models.Constants;

namespace RodelDraw.Models.Client;

/// <summary>
/// 绘图客户端配置.
/// </summary>
public sealed class DrawClientConfiguration
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
    /// 千帆客户端配置.
    /// </summary>
    [JsonPropertyName("qianfan")]
    public QianFanClientConfig? QianFan { get; set; }

    /// <summary>
    /// 讯飞星火客户端配置.
    /// </summary>
    [JsonPropertyName("spark_desk")]
    public SparkDeskClientConfig? SparkDesk { get; set; }

    /// <summary>
    /// 混元客户端配置.
    /// </summary>
    [JsonPropertyName("hunyuan")]
    public HunYuanClientConfig? HunYuan { get; set; }
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
    /// <summary>
    /// 版本.
    /// </summary>
    [JsonPropertyName("version")]
    public AzureOpenAIVersion Version { get; set; } = AzureOpenAIVersion.V2024_02_01;

    /// <inheritdoc/>
    public override bool IsValid()
    {
        return base.IsValid()
            && !string.IsNullOrEmpty(Endpoint)
            && CustomModels != null
            && CustomModels.Count > 0;
    }
}

/// <summary>
/// 千帆客户端配置.
/// </summary>
public class QianFanClientConfig : ClientConfigBase
{
    /// <summary>
    /// 密匙.
    /// </summary>
    [JsonPropertyName("secret")]
    public string Secret { get; set; }

    /// <inheritdoc/>
    public override bool IsValid()
        => base.IsValid() && !string.IsNullOrEmpty(Secret);
}

/// <summary>
/// 讯飞星火服务配置.
/// </summary>
public sealed class SparkDeskClientConfig : QianFanClientConfig
{
    /// <summary>
    /// 应用程序 ID.
    /// </summary>
    [JsonPropertyName("app_id")]
    public string AppId { get; set; }

    /// <inheritdoc/>
    public override bool IsValid()
        => base.IsValid() && !string.IsNullOrEmpty(AppId);
}

/// <summary>
/// 混元客户端配置.
/// </summary>
public sealed class HunYuanClientConfig : ClientConfigBase
{
    /// <summary>
    /// 密匙 ID.
    /// </summary>
    [JsonPropertyName("secret_id")]
    public string SecretId { get; set; }

    /// <inheritdoc/>
    public override bool IsValid()
        => base.IsValid() && !string.IsNullOrEmpty(SecretId);
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
    public List<DrawModel>? CustomModels { get; set; }

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
