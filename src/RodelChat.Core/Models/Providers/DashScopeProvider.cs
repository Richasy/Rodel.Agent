// Copyright (c) Rodel. All rights reserved.

using RodelChat.Core.Models.Constants;

namespace RodelChat.Core.Models.Providers;

/// <summary>
/// 阿里灵积（包含通义千问）.
/// </summary>
public sealed class DashScopeProvider : ProviderBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DashScopeProvider"/> class.
    /// </summary>
    public DashScopeProvider()
    {
        Id = ProviderType.DashScope.ToString();
        ServerModels = PredefinedModels.DashScopeModels;
    }
}
