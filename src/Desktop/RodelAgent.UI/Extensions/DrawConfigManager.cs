// Copyright (c) Richasy. All rights reserved.

using Richasy.AgentKernel;
using Richasy.AgentKernel.Connectors.Azure.Models;
using Richasy.AgentKernel.Connectors.Baidu.Models;
using Richasy.AgentKernel.Connectors.IFlyTek.Models;
using Richasy.AgentKernel.Connectors.OpenAI.Models;
using Richasy.AgentKernel.Connectors.Tencent.Models;
using Richasy.AgentKernel.Models;
using RodelAgent.Interfaces;

namespace RodelAgent.UI.Extensions;

/// <summary>
/// 绘制配置管理器.
/// </summary>
internal sealed class DrawConfigManager : DrawConfigManagerBase
{
    /// <inheritdoc/>
    protected override AIServiceConfig? ConvertToConfig(DrawClientConfigBase? config)
    {
        return config switch
        {
            OpenAIDrawConfig openAIConfig => openAIConfig.ToAIServiceConfig(),
            AzureOpenAIDrawConfig azureOaiConfig => azureOaiConfig.ToAIServiceConfig(),
            ErnieDrawConfig ernieConfig => ernieConfig.ToAIServiceConfig(),
            HunyuanDrawConfig hunyuanConfig => hunyuanConfig.ToAIServiceConfig(),
            SparkDrawConfig sparkConfig => sparkConfig.ToAIServiceConfig(),
            _ => null,
        };
    }

    /// <inheritdoc/>
    protected override async Task<DrawClientConfiguration> OnInitializeAsync()
    {
        var providers = Enum.GetValues<DrawProviderType>().ToList();
        var config = new DrawClientConfiguration();
        var storageService = this.Get<IStorageService>();
        foreach (var provider in providers)
        {
            switch (provider)
            {
                case DrawProviderType.OpenAI:
                    config.OpenAI = await storageService.GetDrawConfigAsync(provider, JsonGenContext.Default.OpenAIDrawConfig);
                    break;
                case DrawProviderType.AzureOpenAI:
                    config.AzureOpenAI = await storageService.GetDrawConfigAsync(provider, JsonGenContext.Default.AzureOpenAIDrawConfig);
                    break;
                case DrawProviderType.Ernie:
                    config.Ernie = await storageService.GetDrawConfigAsync(provider, JsonGenContext.Default.ErnieDrawConfig);
                    break;
                case DrawProviderType.Hunyuan:
                    config.Hunyuan = await storageService.GetDrawConfigAsync(provider, JsonGenContext.Default.HunyuanDrawConfig);
                    break;
                case DrawProviderType.Spark:
                    config.Spark = await storageService.GetDrawConfigAsync(provider, JsonGenContext.Default.SparkDrawConfig);
                    break;
                default:
                    break;
            }
        }

        return config;
    }

    /// <inheritdoc/>
    protected override async Task OnSaveAsync(DrawClientConfiguration configuration)
    {
        var providers = Enum.GetValues<DrawProviderType>();
        var storageService = this.Get<IStorageService>();
        foreach (var provider in providers)
        {
            switch (provider)
            {
                case DrawProviderType.OpenAI:
                    await storageService.SetDrawConfigAsync(provider, configuration.OpenAI ?? new(), JsonGenContext.Default.OpenAIDrawConfig);
                    break;
                case DrawProviderType.AzureOpenAI:
                    await storageService.SetDrawConfigAsync(provider, configuration.AzureOpenAI ?? new(), JsonGenContext.Default.AzureOpenAIDrawConfig);
                    break;
                case DrawProviderType.Ernie:
                    await storageService.SetDrawConfigAsync(provider, configuration.Ernie ?? new(), JsonGenContext.Default.ErnieDrawConfig);
                    break;
                case DrawProviderType.Hunyuan:
                    await storageService.SetDrawConfigAsync(provider, configuration.Hunyuan ?? new(), JsonGenContext.Default.HunyuanDrawConfig);
                    break;
                case DrawProviderType.Spark:
                    await storageService.SetDrawConfigAsync(provider, configuration.Spark ?? new(), JsonGenContext.Default.SparkDrawConfig);
                    break;
                default:
                    break;
            }
        }
    }
}

internal static partial class ConfigExtensions
{
    public static AIServiceConfig? ToAIServiceConfig(this AzureOpenAIDrawConfig? config)
    {
        return config is null || string.IsNullOrWhiteSpace(config.Key) || string.IsNullOrEmpty(config.Endpoint)
            ? default
            : new AzureOpenAIServiceConfig(config.Key, string.Empty, new(config.Endpoint));
    }

    public static AIServiceConfig? ToAIServiceConfig(this OpenAIDrawConfig? config)
    {
        var endpoint = string.IsNullOrEmpty(config?.Endpoint) ? null : new Uri(config.Endpoint);
        return config is null || string.IsNullOrWhiteSpace(config.Key)
            ? default
            : new OpenAIServiceConfig(config.Key, string.Empty, endpoint, config.OrganizationId);
    }

    public static AIServiceConfig? ToAIServiceConfig(this ErnieDrawConfig? config)
    {
        return config is null || string.IsNullOrWhiteSpace(config.Key) || string.IsNullOrEmpty(config.Secret)
            ? default
            : new ErnieServiceConfig(config.Key, config.Secret!, string.Empty);
    }

    public static AIServiceConfig? ToAIServiceConfig(this HunyuanDrawConfig? config)
    {
        return config is null || string.IsNullOrWhiteSpace(config.Key) || string.IsNullOrEmpty(config.Secret)
            ? default
            : new HunyuanDrawServiceConfig(config.Key, config.Secret, string.Empty);
    }

    public static AIServiceConfig? ToAIServiceConfig(this SparkDrawConfig? config)
    {
        return config is null || string.IsNullOrWhiteSpace(config.Key) || string.IsNullOrEmpty(config.AppId)
            ? default
            : new SparkDrawServiceConfig(config.Key, config.Secret!, config.AppId, string.Empty);
    }
}
