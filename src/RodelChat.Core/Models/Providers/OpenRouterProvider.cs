// Copyright (c) Rodel. All rights reserved.

using RodelChat.Core.Models.Constants;

namespace RodelChat.Core.Models.Providers;

/// <summary>
/// Open Router 服务商.
/// </summary>
public sealed class OpenRouterProvider : ProviderBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OpenRouterProvider"/> class.
    /// </summary>
    public OpenRouterProvider()
    {
        Id = ProviderType.OpenRouter.ToString();
        BaseUrl = ProviderConstants.OpenRouterApi;
        ServerModels = PredefinedModels.OpenRouterModels;
    }
}
