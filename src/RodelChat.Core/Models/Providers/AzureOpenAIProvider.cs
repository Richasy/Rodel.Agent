// Copyright (c) Rodel. All rights reserved.

using RodelChat.Core.Models.Constants;

namespace RodelChat.Core.Models.Providers;

/// <summary>
/// Azure Open AI 服务商.
/// </summary>
public sealed class AzureOpenAIProvider : ProviderBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AzureOpenAIProvider"/> class.
    /// </summary>
    public AzureOpenAIProvider()
        => Id = ProviderType.AzureOpenAI.ToString();

    /// <summary>
    /// 获取或设置 API 版本.
    /// </summary>
    public AzureOpenAIVersion Version { get; set; }
}
