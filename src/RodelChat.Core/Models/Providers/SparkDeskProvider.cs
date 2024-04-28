// Copyright (c) Rodel. All rights reserved.

using RodelChat.Core.Models.Constants;

namespace RodelChat.Core.Models.Providers;

/// <summary>
/// 百度千帆提供程序.
/// </summary>
public sealed class SparkDeskProvider : ProviderBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SparkDeskProvider"/> class.
    /// </summary>
    public SparkDeskProvider()
    {
        Id = ProviderType.SparkDesk.ToString();
        ServerModels = PredefinedModels.SparkDeskModels;
    }

    /// <summary>
    /// 获取或设置应用程序 ID.
    /// </summary>
    public string AppId { get; set; }

    /// <summary>
    /// 获取或设置密钥.
    /// </summary>
    public string Secret { get; set; }
}
