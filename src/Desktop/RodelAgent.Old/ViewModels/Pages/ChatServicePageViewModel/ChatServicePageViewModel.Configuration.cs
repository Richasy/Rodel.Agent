// Copyright (c) Richasy. All rights reserved.

namespace RodelAgent.UI.ViewModels.Pages;

/// <summary>
/// 聊天服务页面视图模型关于配置管理的部分.
/// </summary>
public sealed partial class ChatServicePageViewModel
{
    private void ResetChatClientConfiguration()
    {
        var config = new ChatClientConfiguration();
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

        var chatProviderFactory = this.Get<IChatProviderFactory>();
        chatProviderFactory.ResetConfiguration(config);
    }
}
