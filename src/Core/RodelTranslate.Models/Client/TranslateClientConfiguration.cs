// Copyright (c) Richasy. All rights reserved.

using System.Text.Json.Serialization;

namespace RodelTranslate.Models.Client;

/// <summary>
/// 翻译客户端的配置.
/// </summary>
public sealed class TranslateClientConfiguration
{
    /// <summary>
    /// Azure 翻译配置.
    /// </summary>
    [JsonPropertyName("azure")]
    public AzureClientConfig? Azure { get; set; }

    /// <summary>
    /// 百度翻译配置.
    /// </summary>
    [JsonPropertyName("baidu")]
    public BaiduClientConfig? Baidu { get; set; }

    /// <summary>
    /// 有道翻译配置.
    /// </summary>
    [JsonPropertyName("youdao")]
    public YoudaoClientConfig? Youdao { get; set; }

    /// <summary>
    /// 腾讯翻译配置.
    /// </summary>
    [JsonPropertyName("tencent")]
    public TencentClientConfig? Tencent { get; set; }

    /// <summary>
    /// 阿里翻译配置.
    /// </summary>
    [JsonPropertyName("ali")]
    public AliClientConfig? Ali { get; set; }

    /// <summary>
    /// 火山翻译配置.
    /// </summary>
    [JsonPropertyName("volcano")]
    public VolcanoClientConfig? Volcano { get; set; }
}

/// <summary>
/// 阿里翻译客户端配置.
/// </summary>
public class AliClientConfig : ClientConfigBase
{
    /// <summary>
    /// 密钥.
    /// </summary>
    [JsonPropertyName("secret")]
    public string? Secret { get; set; }
}

/// <summary>
/// Azure 翻译客户端配置.
/// </summary>
public class AzureClientConfig : ClientConfigBase
{
    /// <summary>
    /// 区域.
    /// </summary>
    [JsonPropertyName("region")]
    public string? Region { get; set; }

    /// <inheritdoc/>
    public override bool IsValid()
        => base.IsValid() && !string.IsNullOrEmpty(Region);
}

/// <summary>
/// 百度翻译客户端配置.
/// </summary>
public class BaiduClientConfig : AppClientConfigBase
{
}

/// <summary>
/// 有道翻译客户端配置.
/// </summary>
public class YoudaoClientConfig : AppClientConfigBase
{
}

/// <summary>
/// 腾讯翻译客户端配置.
/// </summary>
public class TencentClientConfig : ClientConfigBase
{
    /// <summary>
    /// 密钥标识.
    /// </summary>
    [JsonPropertyName("secret_id")]
    public string? SecretId { get; set; }

    /// <inheritdoc/>
    public override bool IsValid()
        => base.IsValid() && !string.IsNullOrEmpty(SecretId);
}

/// <summary>
/// 火山翻译客户端配置.
/// </summary>
public class VolcanoClientConfig : ClientConfigBase
{
    /// <summary>
    /// 密钥标识.
    /// </summary>
    [JsonPropertyName("key_id")]
    public string? KeyId { get; set; }

    /// <inheritdoc/>
    public override bool IsValid()
        => base.IsValid() && !string.IsNullOrEmpty(KeyId);
}

/// <summary>
/// 应用客户端配置基类.
/// </summary>
public abstract class AppClientConfigBase : ClientConfigBase
{
    /// <summary>
    /// 应用标识.
    /// </summary>
    [JsonPropertyName("app_id")]
    public string? AppId { get; set; }

    /// <inheritdoc/>
    public override bool IsValid()
        => base.IsValid() && !string.IsNullOrEmpty(AppId);
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
/// 配置基类.
/// </summary>
public abstract class ConfigBase
{
}
