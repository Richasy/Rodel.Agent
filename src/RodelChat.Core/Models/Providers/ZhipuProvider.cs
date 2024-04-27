// Copyright (c) Rodel. All rights reserved.

using RodelChat.Core.Models.Constants;

namespace RodelChat.Core.Models.Providers;

/// <summary>
/// 智谱服务商.
/// </summary>
public sealed class ZhipuProvider : ProviderBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ZhipuProvider"/> class.
    /// </summary>
    public ZhipuProvider() => Id = ProviderType.Zhipu.ToString();
}
