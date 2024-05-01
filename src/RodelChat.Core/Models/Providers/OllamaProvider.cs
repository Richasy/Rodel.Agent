// Copyright (c) Rodel. All rights reserved.

using RodelChat.Core.Models.Constants;

namespace RodelChat.Core.Models.Providers;

/// <summary>
/// Ollama 服务商.
/// </summary>
public sealed class OllamaProvider : ProviderBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OllamaProvider"/> class.
    /// </summary>
    public OllamaProvider()
    {
        Id = ProviderType.Ollama.ToString();
        BaseUrl = ProviderConstants.OllamaApi;
    }
}
