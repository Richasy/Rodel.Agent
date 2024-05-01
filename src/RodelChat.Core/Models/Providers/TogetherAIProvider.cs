// Copyright (c) Rodel. All rights reserved.

using RodelChat.Core.Models.Constants;

namespace RodelChat.Core.Models.Providers;

/// <summary>
/// TogetherAI 服务商.
/// </summary>
public sealed class TogetherAIProvider : ProviderBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TogetherAIProvider"/> class.
    /// </summary>
    public TogetherAIProvider()
    {
        Id = ProviderType.TogetherAI.ToString();
        BaseUrl = ProviderConstants.TogetherAIApi;
        ServerModels = PredefinedModels.TogetherAIModels;
    }
}
