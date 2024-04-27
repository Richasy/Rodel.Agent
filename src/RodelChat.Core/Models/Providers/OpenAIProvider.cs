// Copyright (c) Rodel. All rights reserved.

using RodelChat.Core.Models.Constants;

namespace RodelChat.Core.Models.Providers;

/// <summary>
/// Open AI 服务商.
/// </summary>
public sealed class OpenAIProvider : ProviderBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OpenAIProvider"/> class.
    /// </summary>
    public OpenAIProvider()
    {
        Id = ProviderType.OpenAI.ToString();
        ServerModels = PredefinedModels.OpenAIModels;
    }

    /// <summary>
    /// 组织标识符.
    /// </summary>
    public string OrganizationId { get; set; }
}
