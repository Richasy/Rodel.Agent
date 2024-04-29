// Copyright (c) Rodel. All rights reserved.

using RodelChat.Core.Models.Constants;

namespace RodelChat.Core.Models.Providers;

/// <summary>
/// Gemini 服务商.
/// </summary>
public sealed class GeminiProvider : ProviderBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GeminiProvider"/> class.
    /// </summary>
    public GeminiProvider()
    {
        Id = ProviderType.Gemini.ToString();
        BaseUrl = ProviderConstants.GeminiApi;
        ServerModels = PredefinedModels.GeminiModels;
    }
}
