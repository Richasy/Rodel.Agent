// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.Models.Constants;
using RodelAgent.UI.Toolkits;
using RodelAgent.UI.ViewModels.Components;
using RodelAgent.UI.ViewModels.Items;

namespace RodelAgent.UI.ViewModels.Pages;

/// <summary>
/// 聊天服务页面视图模型关于服务的部分.
/// </summary>
public sealed partial class ChatServicePageViewModel
{
    [RelayCommand]
    private async Task ResetAvailableChatServicesAsync()
    {
        var chatServices = await PageViewModelShare.GetChatServicesAsync(_storageService);
        SyncChatServices(chatServices.Where(p => p.IsCompleted).ToList());

        IsAvailableServicesEmpty = AvailableServices.Count == 0;
        ResetChatClientConfiguration();
        if (SettingsToolkit.IsSettingKeyExist(SettingNames.LastSelectedChatService))
        {
            var lastSelectedService = SettingsToolkit.ReadLocalSetting(SettingNames.LastSelectedChatService, ProviderType.OpenAI);
            var lastSelectedServiceVM = AvailableServices.FirstOrDefault(p => p.ProviderType == lastSelectedService);
            SetSelectedChatServiceCommand.Execute(lastSelectedServiceVM ?? AvailableServices.FirstOrDefault());
        }
    }

    [RelayCommand]
    private async Task SetSelectedChatServiceAsync(ChatServiceItemViewModel chatVM)
    {
        foreach (var item in AvailableServices)
        {
            item.IsSelected = chatVM != null && item.Equals(chatVM);
        }

        if (chatVM != null)
        {
            SetSelectedAgentCommand.Execute(default);
            SetSelectedSessionPresetCommand.Execute(default);
            SetSelectedGroupPresetCommand.Execute(default);
            HistoryChatSessions.Clear();
            var sessions = await _storageService.GetChatSessionsAsync(chatVM.ProviderType);
            foreach (var session in sessions)
            {
                HistoryChatSessions.Add(new ChatSessionViewModel(session, _chatClient));
            }

            CheckHistorySessionStatus();
            _chatClient.LoadChatSessions(sessions);
            SettingsToolkit.WriteLocalSetting(SettingNames.LastSelectedChatService, chatVM.ProviderType);
            CreateNewSession();
        }
        else
        {
            SettingsToolkit.DeleteLocalSetting(SettingNames.LastSelectedChatService);
        }
    }

    private void SyncChatServices(List<ChatServiceItemViewModel> list)
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
