// Copyright (c) Rodel. All rights reserved.

using RodelTranslate.Core.Providers;
using RodelTranslate.Models.Client;
using RodelTranslate.Models.Constants;

namespace RodelTranslate.Core.Factories;

/// <summary>
/// 翻译提供程序工厂.
/// </summary>
public sealed partial class TranslateProviderFactory
{
    private void InjectAzure(AzureClientConfig? config)
    {
        if (!string.IsNullOrEmpty(config?.Key) && !string.IsNullOrEmpty(config?.Region))
        {
            AddCreateMethod(ProviderType.Azure, () => new AzureProvider(config));
        }
    }

    private void InjectBaidu(BaiduClientConfig? config)
    {
        if (!string.IsNullOrEmpty(config?.Key) && !string.IsNullOrEmpty(config?.AppId))
        {
            AddCreateMethod(ProviderType.Baidu, () => new BaiduProvider(config));
        }
    }

    private void InjectYoudao(YoudaoClientConfig? config)
    {
        if (!string.IsNullOrEmpty(config?.Key) && !string.IsNullOrEmpty(config?.AppId))
        {
            AddCreateMethod(ProviderType.Youdao, () => new YoudaoProvider(config));
        }
    }

    private void InjectTencent(TencentClientConfig? config)
    {
        if (!string.IsNullOrEmpty(config?.Key) && !string.IsNullOrEmpty(config?.SecretId))
        {
            AddCreateMethod(ProviderType.Tencent, () => new TencentProvider(config));
        }
    }

    private void InjectAli(AliClientConfig? config)
    {
        if (!string.IsNullOrEmpty(config?.Key))
        {
            AddCreateMethod(ProviderType.Ali, () => new AliProvider(config));
        }
    }

    private void InjectVolcano(VolcanoClientConfig? config)
    {
        if (!string.IsNullOrEmpty(config?.Key) && !string.IsNullOrEmpty(config?.KeyId))
        {
            AddCreateMethod(ProviderType.Volcano, () => new VolcanoProvider(config));
        }
    }
}
