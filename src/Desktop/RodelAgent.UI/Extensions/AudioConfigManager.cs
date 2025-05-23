﻿// Copyright (c) Richasy. All rights reserved.

using Richasy.AgentKernel;
using Richasy.AgentKernel.Connectors.Azure.Models;
using Richasy.AgentKernel.Connectors.OpenAI.Models;
using Richasy.AgentKernel.Connectors.Tencent.Models;
using Richasy.AgentKernel.Connectors.Volcano.Models;
using Richasy.AgentKernel.Models;
using RodelAgent.Interfaces;

namespace RodelAgent.UI.Extensions;

/// <summary>
/// 音频服务配置管理器.
/// </summary>
public sealed class AudioConfigManager : AudioConfigManagerBase
{
    /// <inheritdoc/>
    protected override AIServiceConfig? ConvertToConfig(AudioClientConfigBase? config)
    {
        return config switch
        {
            OpenAIAudioConfig openAIConfig => openAIConfig.ToAIServiceConfig(),
            AzureOpenAIAudioConfig azureOaiConfig => azureOaiConfig.ToAIServiceConfig(),
            AzureAudioConfig azureConfig => azureConfig.ToAIServiceConfig(),
            VolcanoAudioConfig volcanoConfig => volcanoConfig.ToAIServiceConfig(),
            TencentAudioConfig tencentConfig => tencentConfig.ToAIServiceConfig(),
            _ => null,
        };
    }

    /// <inheritdoc/>
    protected override async Task<AudioClientConfiguration> OnInitializeAsync()
    {
        var providers = Enum.GetValues<AudioProviderType>().ToList();
        var config = new AudioClientConfiguration();
        var storageService = this.Get<IStorageService>();
        foreach (var provider in providers)
        {
            switch (provider)
            {
                case AudioProviderType.OpenAI:
                    config.OpenAI = await storageService.GetAudioConfigAsync(provider, JsonGenContext.Default.OpenAIAudioConfig);
                    break;
                case AudioProviderType.AzureOpenAI:
                    config.AzureOpenAI = await storageService.GetAudioConfigAsync(provider, JsonGenContext.Default.AzureOpenAIAudioConfig);
                    break;
                case AudioProviderType.Azure:
                    config.AzureSpeech = await storageService.GetAudioConfigAsync(provider, JsonGenContext.Default.AzureAudioConfig);
                    break;
                case AudioProviderType.Volcano:
                    config.Volcano = await storageService.GetAudioConfigAsync(provider, JsonGenContext.Default.VolcanoAudioConfig);
                    break;
                case AudioProviderType.Tencent:
                    config.Tencent = await storageService.GetAudioConfigAsync(provider, JsonGenContext.Default.TencentAudioConfig);
                    break;
                default:
                    break;
            }
        }

        return config;
    }

    /// <inheritdoc/>
    protected override async Task OnSaveAsync(AudioClientConfiguration configuration)
    {
        var providers = Enum.GetValues<AudioProviderType>();
        var storageService = this.Get<IStorageService>();
        foreach (var provider in providers)
        {
            switch (provider)
            {
                case AudioProviderType.OpenAI:
                    await storageService.SetAudioConfigAsync(provider, configuration.OpenAI ?? new(), JsonGenContext.Default.OpenAIAudioConfig);
                    break;
                case AudioProviderType.AzureOpenAI:
                    await storageService.SetAudioConfigAsync(provider, configuration.AzureOpenAI ?? new(), JsonGenContext.Default.AzureOpenAIAudioConfig);
                    break;
                case AudioProviderType.Azure:
                    await storageService.SetAudioConfigAsync(provider, configuration.AzureSpeech ?? new(), JsonGenContext.Default.AzureAudioConfig);
                    break;
                case AudioProviderType.Volcano:
                    await storageService.SetAudioConfigAsync(provider, configuration.Volcano ?? new(), JsonGenContext.Default.VolcanoAudioConfig);
                    break;
                case AudioProviderType.Tencent:
                    await storageService.SetAudioConfigAsync(provider, configuration.Tencent ?? new(), JsonGenContext.Default.TencentAudioConfig);
                    break;
                default:
                    break;
            }
        }
    }
}

internal static partial class ConfigExtensions
{
    public static AIServiceConfig? ToAIServiceConfig(this OpenAIAudioConfig? config)
    {
        var endpoint = string.IsNullOrEmpty(config?.Endpoint) ? null : new Uri(config.Endpoint);
        return config is null || string.IsNullOrWhiteSpace(config.Key)
            ? default
            : new OpenAIServiceConfig(config.Key, string.Empty, endpoint, config.OrganizationId);
    }

    public static AIServiceConfig? ToAIServiceConfig(this AzureOpenAIAudioConfig? config)
    {
        return config is null || string.IsNullOrWhiteSpace(config.Key) || string.IsNullOrEmpty(config.Endpoint)
            ? default
            : new AzureOpenAIServiceConfig(config.Key, string.Empty, new(config.Endpoint));
    }

    public static AIServiceConfig? ToAIServiceConfig(this AzureAudioConfig? config)
    {
        return config is null || string.IsNullOrWhiteSpace(config.Key) || string.IsNullOrEmpty(config.Region)
            ? default
            : new AzureAudioServiceConfig(config.Key, config.Region);
    }

    public static AIServiceConfig? ToAIServiceConfig(this VolcanoAudioConfig? config)
    {
        return config is null || string.IsNullOrWhiteSpace(config.Key) || string.IsNullOrEmpty(config.AppId)
            ? default
            : new VolcanoAudioServiceConfig(config.Key, config.AppId, string.Empty);
    }

    public static AIServiceConfig? ToAIServiceConfig(this TencentAudioConfig? config)
    {
        return config is null || string.IsNullOrWhiteSpace(config.Key) || string.IsNullOrEmpty(config.SecretId)
            ? default
            : new TencentAudioServiceConfig(config.Key, config.SecretId, string.Empty);
    }
}