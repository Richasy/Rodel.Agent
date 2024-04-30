// Copyright (c) Rodel. All rights reserved.

using RodelChat.Core.Models.Constants;

namespace RodelChat.Core.Models.Providers;

/// <summary>
/// 阿里灵积（包含通义千问）.
/// </summary>
public sealed class GroqProvider : ProviderBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GroqProvider"/> class.
    /// </summary>
    public GroqProvider()
    {
        Id = ProviderType.Groq.ToString();
        BaseUrl = ProviderConstants.GroqApi;
        ServerModels = PredefinedModels.GroqModels;
    }
}
