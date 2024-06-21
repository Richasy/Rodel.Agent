// Copyright (c) Rodel. All rights reserved.

using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using RodelAgent.Interfaces;
using RodelDraw.Models.Constants;

namespace RodelAgent.Statics;

/// <summary>
/// 绘图静态类.
/// </summary>
public static class DrawStatics
{
    /// <summary>
    /// 获取在线绘图服务.
    /// </summary>
    /// <returns>服务列表.</returns>
    public static Dictionary<ProviderType, string> GetOnlineDrawServices()
    {
        var resourceToolkit = GlobalStatics.ServiceProvider.GetRequiredService<IStringResourceToolkit>();
        return new Dictionary<ProviderType, string>
        {
            { ProviderType.OpenAI, "OpenAI" },
            { ProviderType.AzureOpenAI, "Azure OpenAI" },
            { ProviderType.QianFan, resourceToolkit.GetString("QianFan") },
            { ProviderType.HunYuan, resourceToolkit.GetString("HunYuan") },
            { ProviderType.SparkDesk, resourceToolkit.GetString("SparkDesk") },
        };
    }
}
