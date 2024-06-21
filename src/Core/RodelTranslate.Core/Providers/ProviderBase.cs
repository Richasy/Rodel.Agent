// Copyright (c) Rodel. All rights reserved.

using Microsoft.SemanticKernel;
using RodelTranslate.Models.Client;
using RodelTranslate.Models.Constants;

namespace RodelTranslate.Core.Providers;

/// <summary>
/// 服务提供商基类.
/// </summary>
public abstract class ProviderBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProviderBase"/> class.
    /// </summary>
    protected ProviderBase(string key)
        => AccessKey = key;

    /// <summary>
    /// 服务端模型列表.
    /// </summary>
    public List<Language>? Languages { get; set; }

    /// <summary>
    /// 访问密钥.
    /// </summary>
    protected string? AccessKey { get; set; }

    /// <summary>
    /// 内核.
    /// </summary>
    protected Kernel? Kernel { get; set; }

    /// <summary>
    /// 服务商类型.
    /// </summary>
    protected ProviderType Type { get; init; }

    /// <summary>
    /// 释放资源.
    /// </summary>
    public void Release()
        => Kernel = default;

    /// <summary>
    /// 获取语言列表.
    /// </summary>
    /// <returns>语言列表.</returns>
    public List<Language> GetLanguages() => Languages ?? new List<Language>();

    /// <summary>
    /// 转换执行设置.
    /// </summary>
    /// <param name="sessionData">会话.</param>
    /// <returns>执行设置.</returns>
    public virtual TranslateExecutionSettings ConvertExecutionSettings(TranslateSession sessionData)
        => new TranslateExecutionSettings();

    /// <summary>
    /// 获取最大文本长度.
    /// </summary>
    /// <returns>最大文本长度.</returns>
    public virtual long GetMaxTextLength() => 5000;
}
