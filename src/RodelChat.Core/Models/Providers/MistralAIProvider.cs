// Copyright (c) Rodel. All rights reserved.

using RodelChat.Core.Models.Constants;

namespace RodelChat.Core.Models.Providers;

/// <summary>
/// Mistral AI 服务商.
/// </summary>
public sealed class MistralAIProvider : ProviderBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MistralAIProvider"/> class.
    /// </summary>
    public MistralAIProvider()
    {
        Id = ProviderType.MistralAI.ToString();
        BaseUrl = ProviderConstants.MistralAIApi;
        ServerModels = PredefinedModels.MistralAIModels;
    }
}
