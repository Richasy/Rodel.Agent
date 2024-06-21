// Copyright (c) Rodel. All rights reserved.

using RodelAgent.UI.Models.Constants;
using RodelAgent.UI.Toolkits;
using RodelAgent.UI.ViewModels.Items;
using RodelTranslate.Models.Client;
using RodelTranslate.Models.Constants;

namespace RodelAgent.UI.ViewModels.Pages;

/// <summary>
/// 聊天服务页面视图模型关于服务的部分.
/// </summary>
public sealed partial class TranslateServicePageViewModel
{
    [RelayCommand]
    private async Task ResetAvailableTranslateServicesAsync()
    {
        var translateServices = await PageViewModelShare.GetTranslateServicesAsync(_storageService);
        SyncTranslateServices(translateServices.Where(p => p.IsCompleted).ToList());

        IsAvailableServicesEmpty = AvailableServices.Count == 0;
        ResetTranslateClientConfiguration();
        if (SettingsToolkit.IsSettingKeyExist(SettingNames.LastSelectedTranslateService))
        {
            var lastSelectedService = SettingsToolkit.ReadLocalSetting(SettingNames.LastSelectedTranslateService, ProviderType.Azure);
            var lastSelectedServiceVM = AvailableServices.FirstOrDefault(p => p.ProviderType == lastSelectedService);
            SetSelectedTranslateServiceCommand.Execute(lastSelectedServiceVM ?? AvailableServices.FirstOrDefault());
        }
        else
        {
            SetSelectedTranslateServiceCommand.Execute(AvailableServices.FirstOrDefault());
        }
    }

    [RelayCommand]
    private void SetSelectedTranslateService(TranslateServiceItemViewModel translateVM)
    {
        foreach (var item in AvailableServices)
        {
            item.IsSelected = translateVM != null && item.Equals(translateVM);
        }

        if (translateVM != null)
        {
            SettingsToolkit.WriteLocalSetting(SettingNames.LastSelectedTranslateService, translateVM.ProviderType);
            Session.InitializeCommand.Execute(translateVM.ProviderType);
        }
        else
        {
            SettingsToolkit.DeleteLocalSetting(SettingNames.LastSelectedTranslateService);
        }

        ReloadHistoryCommand.Execute(default);
    }

    [RelayCommand]
    private async Task ReloadHistoryAsync()
    {
        History.Clear();
        if (Session.TranslateService == null)
        {
            return;
        }

        var provider = Session.TranslateService.ProviderType;
        var history = await _storageService.GetTranslateSessionsAsync(provider);
        foreach (var item in history)
        {
            History.Add(item);
        }
    }

    [RelayCommand]
    private async Task DeleteHistoryItemAsync(TranslateSession session)
    {
        History.Remove(session);
        await _storageService.RemoveTranslateSessionAsync(session.Id);
    }

    private void SyncTranslateServices(List<TranslateServiceItemViewModel> list)
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
