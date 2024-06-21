// Copyright (c) Rodel. All rights reserved.

using System.Collections.Specialized;
using System.IO.Compression;
using System.Text.Json;
using RodelAgent.UI.Models.Constants;
using RodelAgent.UI.Toolkits;
using RodelAgent.UI.ViewModels.Items;

namespace RodelAgent.UI.ViewModels.Pages;

/// <summary>
/// 聊天服务页面视图模型.
/// </summary>
public sealed partial class ChatServicePageViewModel
{
    [RelayCommand]
    private async Task ImportPluginAsync()
    {
        var appVM = GlobalDependencies.ServiceProvider.GetRequiredService<AppViewModel>();
        var file = await FileToolkit.PickFileAsync(".zip", appVM.ActivatedWindow);
        if (file is null)
        {
            return;
        }

        try
        {
            var pluginFolder = AppToolkit.GetChatPluginFolder();
            var fileName = Path.GetFileNameWithoutExtension(file.Path);
            var destFolder = Path.Combine(pluginFolder, fileName);
            var dest = Path.Combine(destFolder, $"{fileName}.dll");
            if (Directory.Exists(destFolder))
            {
                await Task.Run(() =>
                {
                    Directory.Delete(destFolder, true);
                });
            }

            await Task.Run(() =>
            {
                ZipFile.ExtractToDirectory(file.Path, destFolder);
            });

            if (!File.Exists(dest))
            {
                await Task.Run(() =>
                {
                    Directory.Delete(destFolder, true);
                });

                await appVM.ShowMessageDialogAsync(StringNames.PluginDllNotFound);
                return;
            }

            await InsertPluginFileAsync(Path.GetDirectoryName(dest));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to import plugin.");
            appVM.ShowTip(ex.Message);
        }
    }

    [RelayCommand]
    private async Task ResetPluginsAsync(bool force = false)
    {
        if (!force && _isPluginInitialized)
        {
            return;
        }

        var folderPath = AppToolkit.GetChatPluginFolder();
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        var folders = Directory.GetDirectories(folderPath);
        var deletingFolderIds = SettingsToolkit.ReadLocalSetting(SettingNames.DeletingPluginIds, "[]");
        var ids = JsonSerializer.Deserialize<List<string>>(deletingFolderIds);
        foreach (var folder in folders)
        {
            if (ids.Contains(Path.GetFileName(folder)))
            {
                try
                {
                    await Task.Run(() =>
                    {
                        Directory.Delete(folder, true);
                    });
                }
                catch (Exception)
                {
                    continue;
                }
            }

            await InsertPluginFileAsync(folder);
        }

        SettingsToolkit.WriteLocalSetting(SettingNames.DeletingPluginIds, "[]");
        _isPluginInitialized = true;
    }

    private async Task InsertPluginFileAsync(string path)
    {
        var directory = Path.GetFileName(path);
        var dllFile = Path.Combine(path, $"{directory}.dll");
        if (!File.Exists(dllFile))
        {
            return;
        }

        var plugins = await _chatClient.RetrievePluginsFromDllAsync(dllFile);
        foreach (var plugin in plugins)
        {
            var id = $"{directory}<|>{plugin.ExternalId}";
            if (Plugins.Any(p => p.Id == id))
            {
                Plugins.Remove(Plugins.First(p => p.Id == id));
            }

            var pluginVM = new ChatPluginItemViewModel(plugin, id, CheckDeletePlugin);
            Plugins.Insert(0, pluginVM);
        }

        CurrentSession?.ResetPluginsCommand.Execute(default);
    }

    private void CheckDeletePlugin(ChatPluginItemViewModel plugin)
    {
        IsDeletingPluginsNotEmpty = Plugins.Any(p => p.IsDeleting);
        var ids = Plugins.Where(p => p.IsDeleting).Select(p => p.Id).Distinct().ToList();
        var json = JsonSerializer.Serialize(ids);
        SettingsToolkit.WriteLocalSetting(SettingNames.DeletingPluginIds, json);
    }

    private void OnPluginsCountChanged(object sender, NotifyCollectionChangedEventArgs e)
        => CheckPluginsCount();

    private void CheckPluginsCount()
        => IsPluginEmpty = Plugins.Count == 0;
}
