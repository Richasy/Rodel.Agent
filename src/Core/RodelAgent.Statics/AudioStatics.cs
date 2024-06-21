// Copyright (c) Rodel. All rights reserved.

using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using RodelAgent.Interfaces;
using RodelAudio.Models.Constants;

namespace RodelAgent.Statics;

/// <summary>
/// 音频静态类.
/// </summary>
public static class AudioStatics
{
    /// <summary>
    /// 获取在线音频服务.
    /// </summary>
    /// <returns>服务列表.</returns>
    public static Dictionary<ProviderType, string> GetOnlineAudioServices()
    {
        var resourceToolkit = GlobalStatics.ServiceProvider.GetRequiredService<IStringResourceToolkit>();
        return new Dictionary<ProviderType, string>
        {
            { ProviderType.OpenAI, "OpenAI" },
            { ProviderType.AzureOpenAI, "Azure OpenAI" },
            { ProviderType.AzureSpeech, resourceToolkit.GetString("AzureSpeech") },
        };
    }
}
