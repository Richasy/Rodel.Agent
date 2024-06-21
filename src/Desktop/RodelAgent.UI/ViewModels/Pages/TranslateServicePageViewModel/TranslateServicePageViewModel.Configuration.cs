// Copyright (c) Rodel. All rights reserved.

using RodelTranslate.Interfaces.Client;
using RodelTranslate.Models.Client;

namespace RodelAgent.UI.ViewModels.Pages;

/// <summary>
/// 翻译服务页面视图模型关于配置管理的部分.
/// </summary>
public sealed partial class TranslateServicePageViewModel
{
    private void ResetTranslateClientConfiguration()
    {
        var config = new TranslateClientConfiguration();
        foreach (var service in AvailableServices)
        {
            var propertyName = service.ProviderType.ToString();
            var property = config.GetType().GetProperty(propertyName);
            if (property != null && property.PropertyType.IsSubclassOf(typeof(ClientConfigBase)))
            {
                var convertedConfig = Convert.ChangeType(service.Config, property.PropertyType);
                property.SetValue(config, convertedConfig);
            }
            else
            {
                _logger.LogDebug($"无法设置 {propertyName} 的配置.");
                continue;
            }
        }

        var chatProviderFactory = GlobalDependencies.ServiceProvider.GetRequiredService<ITranslateProviderFactory>();
        chatProviderFactory.ResetConfiguration(config);
    }
}
