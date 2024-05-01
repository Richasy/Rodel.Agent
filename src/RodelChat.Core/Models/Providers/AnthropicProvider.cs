// Copyright (c) Rodel. All rights reserved.

using RodelChat.Core.Models.Constants;

namespace RodelChat.Core.Models.Providers;

/// <summary>
/// Anthropic 服务商.
/// </summary>
public sealed class AnthropicProvider : ProviderBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AnthropicProvider"/> class.
    /// </summary>
    public AnthropicProvider()
    {
        Id = ProviderType.Anthropic.ToString();
        ServerModels = PredefinedModels.AnthropicModels;
        BaseUrl = ProviderConstants.AnthropicApi;
    }
}
