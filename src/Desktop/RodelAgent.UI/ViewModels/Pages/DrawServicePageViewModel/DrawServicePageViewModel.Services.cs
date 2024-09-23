// Copyright (c) Rodel. All rights reserved.

using RodelAgent.UI.Models.Constants;
using RodelAgent.UI.Toolkits;
using RodelAgent.UI.ViewModels.Items;
using RodelDraw.Models.Client;
using RodelDraw.Models.Constants;

namespace RodelAgent.UI.ViewModels.Pages;

/// <summary>
/// 绘图服务页面视图模型.
/// </summary>
public sealed partial class DrawServicePageViewModel
{
    [RelayCommand]
    private async Task ResetAvailableDrawServicesAsync()
    {
        var drawServices = await PageViewModelShare.GetDrawServicesAsync(_storageService);
        SyncDrawServices(drawServices.Where(p => p.IsCompleted).ToList());

        IsAvailableServicesEmpty = AvailableServices.Count == 0;
        ResetDrawClientConfiguration();
        if (SettingsToolkit.IsSettingKeyExist(SettingNames.LastSelectedDrawService))
        {
            var lastSelectedService = SettingsToolkit.ReadLocalSetting(SettingNames.LastSelectedDrawService, ProviderType.AzureOpenAI);
            var lastSelectedServiceVM = AvailableServices.FirstOrDefault(p => p.ProviderType == lastSelectedService);
            SetSelectedDrawServiceCommand.Execute(lastSelectedServiceVM ?? AvailableServices.FirstOrDefault());
        }
        else
        {
            SetSelectedDrawServiceCommand.Execute(AvailableServices.FirstOrDefault());
        }
    }

    [RelayCommand]
    private void SetSelectedDrawService(DrawServiceItemViewModel drawVM)
    {
        foreach (var item in AvailableServices)
        {
            item.IsSelected = drawVM != null && item.Equals(drawVM);
        }

        if (drawVM != null)
        {
            SettingsToolkit.WriteLocalSetting(SettingNames.LastSelectedDrawService, drawVM.ProviderType);
            Session.InitializeCommand.Execute(drawVM.ProviderType);
        }
        else
        {
            SettingsToolkit.DeleteLocalSetting(SettingNames.LastSelectedDrawService);
        }

        UpdateHistoryCommand.Execute(default);
    }

    [RelayCommand]
    private async Task UpdateHistoryAsync()
    {
        if (Session.DrawService == null)
        {
            return;
        }

        var history = await _storageService.GetDrawSessionsAsync();
        SyncDrawHistory(history);
    }

    [RelayCommand]
    private async Task DeleteHistoryItemAsync(DrawSessionItemViewModel session)
    {
        if (Session.ImagePath?.Contains(session.Data.Id) ?? false)
        {
            Session.ClearCommand.Execute(default);
            Session.InitializeCommand.Execute(Session.DrawService.ProviderType);
        }

        History.Remove(session);
        await _storageService.RemoveDrawSessionAsync(session.Data.Id);
    }

    private void SyncDrawHistory(List<DrawSession> list)
    {
        // 边界情况处理
        ArgumentNullException.ThrowIfNull(list);
        ArgumentNullException.ThrowIfNull(History);

        var listDict = list.ToDictionary(item => item.Id);

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

    private void SyncDrawServices(List<DrawServiceItemViewModel> list)
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
