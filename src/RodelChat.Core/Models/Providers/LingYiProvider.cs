// Copyright (c) Rodel. All rights reserved.

using RodelChat.Core.Models.Constants;

namespace RodelChat.Core.Models.Providers;

/// <summary>
/// 零一万物服务商.
/// </summary>
public sealed class LingYiProvider : ProviderBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LingYiProvider"/> class.
    /// </summary>
    public LingYiProvider()
    {
        Id = ProviderType.LingYi.ToString();
        ServerModels = PredefinedModels.LingYiModels;
    }
}
