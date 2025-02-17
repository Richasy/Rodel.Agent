// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.Models.Constants;
using RodelAgent.UI.Toolkits;
using RodelAgent.UI.ViewModels.Items;
using RodelAudio.Models.Client;
using RodelAudio.Models.Constants;

namespace RodelAgent.UI.ViewModels.Pages;

/// <summary>
/// 音频服务页面视图模型.
/// </summary>
public sealed partial class AudioServicePageViewModel
{
    [RelayCommand]
    private async Task ResetAvailableAudioServicesAsync()
    {
        var drawServices = await PageViewModelShare.GetAudioServicesAsync(_storageService);
        SyncAudioServices(drawServices.Where(p => p.IsCompleted).ToList());

        IsAvailableServicesEmpty = AvailableServices.Count == 0;
        ResetAudioClientConfiguration();
        if (SettingsToolkit.IsSettingKeyExist(SettingNames.LastSelectedAudioService))
        {
            var lastSelectedService = SettingsToolkit.ReadLocalSetting(SettingNames.LastSelectedAudioService, ProviderType.AzureOpenAI);
            var lastSelectedServiceVM = AvailableServices.FirstOrDefault(p => p.ProviderType == lastSelectedService);
            SetSelectedAudioServiceCommand.Execute(lastSelectedServiceVM ?? AvailableServices.FirstOrDefault());
        }
        else
        {
            SetSelectedAudioServiceCommand.Execute(AvailableServices.FirstOrDefault());
        }
    }

    [RelayCommand]
    private void SetSelectedAudioService(AudioServiceItemViewModel drawVM)
    {
        foreach (var item in AvailableServices)
        {
            item.IsSelected = drawVM != null && item.Equals(drawVM);
        }

        if (drawVM != null)
        {
            SettingsToolkit.WriteLocalSetting(SettingNames.LastSelectedAudioService, drawVM.ProviderType);
            Session.InitializeCommand.Execute(drawVM.ProviderType);
        }
        else
        {
            SettingsToolkit.DeleteLocalSetting(SettingNames.LastSelectedAudioService);
        }

        UpdateHistoryCommand.Execute(default);
    }

    [RelayCommand]
    private async Task UpdateHistoryAsync()
    {
        if (Session.AudioService == null)
        {
            return;
        }

        var history = await _storageService.GetAudioSessionsAsync();
        SyncAudioHistory(history);
    }

    [RelayCommand]
    private async Task DeleteHistoryItemAsync(AudioSessionItemViewModel session)
    {
        if (Session.AudioPath?.Contains(session.Data.Id) ?? false)
        {
            Session.ClearCommand.Execute(default);
            Session.InitializeCommand.Execute(Session.AudioService.ProviderType);
        }

        History.Remove(session);
        await _storageService.RemoveAudioSessionAsync(session.Data.Id);
    }

    private void SyncAudioHistory(List<AudioSession> list)
    {
        // 边界情况处理
        ArgumentNullException.ThrowIfNull(list);
        ArgumentNullException.ThrowIfNull(History);

        var listDict = list.ToDictionary(item => item.Id);

        try
        {
            for (var i = History.Count - 1; i >= 0; i--)
            {
                var item = History[i];
                if (!listDict.ContainsKey(item.Data.Id))
                {
                    History.RemoveAt(i);
                }
            }

            for (var i = 0; i < list.Count; i++)
            {
                var listItem = list[i];
                if (i < History.Count)
                {
                    var collectionItem = History[i];
                    if (!Equals(listItem.Id, collectionItem.Data.Id))
                    {
                        History.Insert(i, new(listItem));
                    }
                }
                else
                {
                    History.Add(new(listItem));
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to sync audio history.");
        }
    }

    private void SyncAudioServices(List<AudioServiceItemViewModel> list)
    {
        // 边界情况处理
        ArgumentNullException.ThrowIfNull(list);
        ArgumentNullException.ThrowIfNull(AvailableServices);

        var listDict = list.ToDictionary(item => item.ProviderType);

        for (var i = AvailableServices.Count - 1; i >= 0; i--)
        {
            var item = AvailableServices[i];
            if (!listDict.ContainsKey(item.ProviderType))
            {
                AvailableServices.RemoveAt(i);
            }
        }

        for (var i = 0; i < list.Count; i++)
        {
            var listItem = list[i];
            if (i < AvailableServices.Count)
            {
                var collectionItem = AvailableServices[i];
                if (!Equals(listItem.ProviderType, collectionItem.ProviderType))
                {
                    AvailableServices.Insert(i, listItem);
                }
                else
                {
                    collectionItem.SetConfig(listItem.Config);
                }
            }
            else
            {
                AvailableServices.Add(listItem);
            }
        }
    }
}
