// Copyright (c) Richasy. All rights reserved.

using RodelDraw.Core.Providers;
using RodelDraw.Models.Client;
using RodelDraw.Models.Constants;

namespace RodelDraw.Core.Factories;

/// <summary>
/// 创建绘图服务商的工厂.
/// </summary>
public sealed partial class DrawProviderFactory
{
    private void InjectOpenAI(OpenAIClientConfig? config)
    {
        if (!string.IsNullOrEmpty(config?.Key))
        {
            AddCreateMethod(ProviderType.OpenAI, () => new OpenAIProvider(config));
        }
    }

    private void InjectAzureOpenAI(AzureOpenAIClientConfig? config)
    {
        if (!string.IsNullOrEmpty(config?.Key)
            && !string.IsNullOrEmpty(config?.Endpoint)
            && config.IsCustomModelNotEmpty())
        {
            AddCreateMethod(ProviderType.AzureOpenAI, () => new AzureOpenAIProvider(config));
        }
    }

    private void InjectQianFan(QianFanClientConfig? config)
    {
        if (!string.IsNullOrEmpty(config?.Key)
            && !string.IsNullOrEmpty(config?.Secret))
        {
            AddCreateMethod(ProviderType.QianFan, () => new QianFanProvider(config));
        }
    }

    private void InjectSparkDesk(SparkDeskClientConfig? config)
    {
        if (!string.IsNullOrEmpty(config?.Key)
            && !string.IsNullOrEmpty(config?.AppId)
            && !string.IsNullOrEmpty(config?.Secret))
        {
            AddCreateMethod(ProviderType.SparkDesk, () => new SparkDeskProvider(config));
        }
    }

    private void InjectHunYuan(HunYuanClientConfig? config)
    {
        if (!string.IsNullOrEmpty(config?.Key)
            && !string.IsNullOrEmpty(config?.SecretId))
        {
            AddCreateMethod(ProviderType.HunYuan, () => new HunYuanProvider(config));
        }
    }
}
