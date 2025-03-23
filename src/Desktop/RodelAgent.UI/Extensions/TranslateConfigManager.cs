// Copyright (c) Richasy. All rights reserved.

using Richasy.AgentKernel;
using Richasy.AgentKernel.Connectors.Ali.Models;
using Richasy.AgentKernel.Connectors.Azure.Models;
using Richasy.AgentKernel.Connectors.Baidu.Models;
using Richasy.AgentKernel.Connectors.Tencent.Models;
using Richasy.AgentKernel.Connectors.Volcano.Models;
using Richasy.AgentKernel.Connectors.Youdao.Models;
using Richasy.AgentKernel.Models;
using RodelAgent.Interfaces;

namespace RodelAgent.UI.Extensions;

/// <summary>
/// 翻译配置管理器.
/// </summary>
public sealed class TranslateConfigManager : TranslateConfigManagerBase
{
    /// <inheritdoc/>
    protected override TranslateServiceConfig? ConvertToConfig(TranslateClientConfigBase? config)
    {
        return config switch
        {
            AzureTranslateConfig azureConfig => azureConfig.ToTranslateServiceConfig(),
            BaiduTranslateConfig baiduConfig => baiduConfig.ToTranslateServiceConfig(),
            AliTranslateConfig aliConfig => aliConfig.ToTranslateServiceConfig(),
            YoudaoTranslateConfig youdaoConfig => youdaoConfig.ToTranslateServiceConfig(),
            VolcanoTranslateConfig volcanoConfig => volcanoConfig.ToTranslateServiceConfig(),
            TencentTranslateConfig tencentConfig => tencentConfig.ToTranslateServiceConfig(),
            _ => null,
        };
    }

    /// <inheritdoc/>
    protected override async Task<TranslateClientConfiguration> OnInitializeAsync()
    {
        var providers = Enum.GetValues<TranslateProviderType>().ToList();
        var config = new TranslateClientConfiguration();
        var storageService = this.Get<IStorageService>();
        foreach (var provider in providers)
        {
            switch (provider)
            {
                case TranslateProviderType.Ali:
                    config.Ali = await storageService.GetTranslateConfigAsync(provider, JsonGenContext.Default.AliTranslateConfig);
                    break;
                case TranslateProviderType.Baidu:
                    config.Baidu = await storageService.GetTranslateConfigAsync(provider, JsonGenContext.Default.BaiduTranslateConfig);
                    break;
                case TranslateProviderType.Youdao:
                    config.Youdao = await storageService.GetTranslateConfigAsync(provider, JsonGenContext.Default.YoudaoTranslateConfig);
                    break;
                case TranslateProviderType.Azure:
                    config.Azure = await storageService.GetTranslateConfigAsync(provider, JsonGenContext.Default.AzureTranslateConfig);
                    break;
                case TranslateProviderType.Volcano:
                    config.Volcano = await storageService.GetTranslateConfigAsync(provider, JsonGenContext.Default.VolcanoTranslateConfig);
                    break;
                case TranslateProviderType.Tencent:
                    config.Tencent = await storageService.GetTranslateConfigAsync(provider, JsonGenContext.Default.TencentTranslateConfig);
                    break;
                default:
                    break;
            }
        }

        return config;
    }

    /// <inheritdoc/>
    protected override async Task OnSaveAsync(TranslateClientConfiguration configuration)
    {
        var providers = Enum.GetValues<TranslateProviderType>();
        var storageService = this.Get<IStorageService>();
        foreach (var provider in providers)
        {
            switch (provider)
            {
                case TranslateProviderType.Ali:
                    await storageService.SetTranslateConfigAsync(provider, configuration.Ali ?? new(), JsonGenContext.Default.AliTranslateConfig);
                    break;
                case TranslateProviderType.Baidu:
                    await storageService.SetTranslateConfigAsync(provider, configuration.Baidu ?? new(), JsonGenContext.Default.BaiduTranslateConfig);
                    break;
                case TranslateProviderType.Youdao:
                    await storageService.SetTranslateConfigAsync(provider, configuration.Youdao ?? new(), JsonGenContext.Default.YoudaoTranslateConfig);
                    break;
                case TranslateProviderType.Azure:
                    await storageService.SetTranslateConfigAsync(provider, configuration.Azure ?? new(), JsonGenContext.Default.AzureTranslateConfig);
                    break;
                case TranslateProviderType.Volcano:
                    await storageService.SetTranslateConfigAsync(provider, configuration.Volcano ?? new(), JsonGenContext.Default.VolcanoTranslateConfig);
                    break;
                case TranslateProviderType.Tencent:
                    await storageService.SetTranslateConfigAsync(provider, configuration.Tencent ?? new(), JsonGenContext.Default.TencentTranslateConfig);
                    break;
                default:
                    break;
            }
        }
    }
}

internal static partial class ConfigExtensions
{
    public static TranslateServiceConfig? ToTranslateServiceConfig(this AzureTranslateConfig? config)
    {
        return config is null || string.IsNullOrWhiteSpace(config.Key) || string.IsNullOrWhiteSpace(config.Region)
            ? throw new ArgumentException("The configuration is not valid.", nameof(config))
            : new AzureTranslateServiceConfig(config.Key, config.Region);
    }

    public static TranslateServiceConfig? ToTranslateServiceConfig(this BaiduTranslateConfig? config)
    {
        return config is null || string.IsNullOrWhiteSpace(config.Key) || string.IsNullOrWhiteSpace(config.AppId)
            ? default
            : new BaiduTranslateServiceConfig(config.AppId, config.Key);
    }

    public static TranslateServiceConfig? ToTranslateServiceConfig(this AliTranslateConfig? config)
    {
        return config is null || string.IsNullOrWhiteSpace(config.Key) || string.IsNullOrWhiteSpace(config.Secret)
            ? default
            : new AliTranslateServiceConfig(config.Key, config.Secret);
    }

    public static TranslateServiceConfig? ToTranslateServiceConfig(this TencentTranslateConfig? config)
    {
        return config is null || string.IsNullOrWhiteSpace(config.Key) || string.IsNullOrEmpty(config.SecretId)
            ? default
            : new TencentTranslateServiceConfig(config.SecretId, config.Key);
    }

    public static TranslateServiceConfig? ToTranslateServiceConfig(this YoudaoTranslateConfig? config)
    {
        return config is null || string.IsNullOrWhiteSpace(config.Key) || string.IsNullOrWhiteSpace(config.AppId)
            ? default
            : new YoudaoTranslateServiceConfig(config.AppId, config.Key);
    }

    public static TranslateServiceConfig? ToTranslateServiceConfig(this VolcanoTranslateConfig? config)
    {
        return config is null || string.IsNullOrWhiteSpace(config.Key) || string.IsNullOrEmpty(config.KeyId)
            ? default
            : new VolcanoTranslateServiceConfig(config.KeyId, config.Key);
    }
}
