// Copyright (c) Rodel. All rights reserved.

using RodelChat.Core.Models.Constants;

namespace RodelChat.Core.Models.Providers;

/// <summary>
/// 月之暗面服务商.
/// </summary>
public sealed class MoonshotProvider : ProviderBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MoonshotProvider"/> class.
    /// </summary>
    public MoonshotProvider()
    {
        Id = ProviderType.Moonshot.ToString();
        ServerModels = PredefinedModels.MoonshotModels;
    }
}
