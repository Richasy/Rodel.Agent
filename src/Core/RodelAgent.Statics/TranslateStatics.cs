// Copyright (c) Rodel. All rights reserved.

using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using RodelAgent.Interfaces;
using RodelTranslate.Models.Constants;

namespace RodelAgent.Statics;

/// <summary>
/// 翻译静态类.
/// </summary>
public static class TranslateStatics
{
    /// <summary>
    /// 获取在线翻译服务.
    /// </summary>
    /// <returns>服务列表.</returns>
    public static Dictionary<ProviderType, string> GetOnlineTranslateServices()
    {
        var resourceToolkit = GlobalStatics.ServiceProvider.GetRequiredService<IStringResourceToolkit>();
        return new Dictionary<ProviderType, string>
        {
            // { ProviderType.Google, resourceToolkit.GetString("Google") },
            { ProviderType.Azure, resourceToolkit.GetString("AzureTranslate") },
            { ProviderType.Ali, resourceToolkit.GetString("AliTranslate") },
            { ProviderType.Baidu, resourceToolkit.GetString("BaiduTranslate") },
            { ProviderType.Tencent, resourceToolkit.GetString("TencentTranslate") },
            { ProviderType.Youdao, resourceToolkit.GetString("YoudaoTranslate") },
            { ProviderType.Volcano, resourceToolkit.GetString("VolcanoTranslate") },
        };
    }
}
