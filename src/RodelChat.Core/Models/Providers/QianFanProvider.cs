// Copyright (c) Rodel. All rights reserved.

using RodelChat.Core.Models.Constants;

namespace RodelChat.Core.Models.Providers;

/// <summary>
/// 百度千帆提供程序.
/// </summary>
public sealed class QianFanProvider : ProviderBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="QianFanProvider"/> class.
    /// </summary>
    public QianFanProvider()
    {
        Id = ProviderType.QianFan.ToString();
        ServerModels = PredefinedModels.QianFanModels;
    }

    /// <summary>
    /// 获取或设置密钥.
    /// </summary>
    public string Secret { get; set; }
}
