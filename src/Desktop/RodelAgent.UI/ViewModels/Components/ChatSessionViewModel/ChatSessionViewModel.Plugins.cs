// Copyright (c) Rodel. All rights reserved.

using System.Text.Json;
using RodelAgent.UI.Models.Constants;
using RodelAgent.UI.Toolkits;
using RodelAgent.UI.ViewModels.Pages;
using RodelChat.Models.Client;

namespace RodelAgent.UI.ViewModels.Components;

/// <summary>
/// 聊天会话视图模型.
/// </summary>
public sealed partial class ChatSessionViewModel
{
    [RelayCommand]
    private async Task ResetPluginsAsync()
    {
        var pageVM = GlobalDependencies.ServiceProvider.GetRequiredService<ChatServicePageViewModel>();
        await pageVM.ResetPluginsCommand.ExecuteAsync(default);
        Plugins.Clear();
        foreach (var item in pageVM.Plugins)
        {
            var plugin = new Items.ChatPluginItemViewModel(item.Data, item.Id, default);
            plugin.IsSelected = Data.Plugins != null && Data.Plugins.Contains(item.Id);
            Plugins.Add(plugin);
        }
    }

    [RelayCommand]
    private void ToolInvokingHandle(ToolInvokingEventArgs e)
    {
        var funcName = e.Function.Name;
        _dispatcherQueue.TryEnqueue(() =>
        {
            GeneratingTipText = string.Format(ResourceToolkit.GetLocalizedString(StringNames.CallingToolTemplate), funcName);
        });

        _logger.LogInformation($"Calling tool: {funcName}\nParameters: {JsonSerializer.Serialize(e.Parameters)}");
    }

    [RelayCommand]
    private void ToolInvokedHandle(ToolInvokedEventArgs e)
    {
        _dispatcherQueue.TryEnqueue(() =>
        {
            GeneratingTipText = ResourceToolkit.GetLocalizedString(StringNames.Generating);
        });
    }

    private void CheckPluginSelectedStatus()
    {
        var selectedPlugins = Plugins.Where(p => p.IsSelected).Select(p => p.Id).ToList();
        if (Data.Plugins == null || !Data.Plugins.SequenceEqual(selectedPlugins))
        {
            Data.Plugins = selectedPlugins;
        }
    }
}
