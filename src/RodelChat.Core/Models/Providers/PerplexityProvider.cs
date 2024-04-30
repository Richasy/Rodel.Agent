// Copyright (c) Rodel. All rights reserved.

using RodelChat.Core.Models.Constants;

namespace RodelChat.Core.Models.Providers;

/// <summary>
/// Perplexity 服务商.
/// </summary>
public sealed class PerplexityProvider : ProviderBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PerplexityProvider"/> class.
    /// </summary>
    public PerplexityProvider()
    {
        Id = ProviderType.Perplexity.ToString();
        BaseUrl = ProviderConstants.PerplexityApi;
        ServerModels = PredefinedModels.PerplexityModels;
    }
}
