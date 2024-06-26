// Copyright (c) Rodel. All rights reserved.

using RodelAgent.Interfaces;
using RodelAgent.UI.ViewModels.Items;
using RodelAgent.UI.ViewModels.Pages;

namespace RodelAgent.UI.ViewModels.Components;

/// <summary>
/// 群组预设模块视图模型.
/// </summary>
public sealed partial class GroupPresetModuleViewModel : ViewModelBase<GroupPresetItemViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GroupPresetModuleViewModel"/> class.
    /// </summary>
    public GroupPresetModuleViewModel(
        IStorageService storageService)
        : base(default)
    {
        _storageService = storageService;
        SelectedAgents.CollectionChanged += (sender, e) => CheckAgentCount();
    }

    /// <summary>
    /// 设置数据.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    public async Task SetDataAsync(GroupPresetItemViewModel data)
    {
        Data = data;
        Name = data.Name;
        MaxRounds = data.Data.MaxRounds;
        TotalAgents.Clear();
        SelectedAgents.Clear();
        TerminateText.Clear();

        var agents = await _storageService.GetChatAgentsAsync();
        foreach (var agent in agents)
        {
            TotalAgents.Add(new ChatPresetItemViewModel(agent));
        }

        foreach (var item in data.Data.Agents)
        {
            var a = TotalAgents.FirstOrDefault(p => p.Data.Id == item);
            if (a == null)
            {
                continue;
            }

            SelectedAgents.Add(a);
        }

        if (data.Data.TerminateText != null)
        {
            foreach (var item in data.Data.TerminateText)
            {
                TerminateText.Add(item);
            }
        }
    }

    [RelayCommand]
    private async Task SaveGroupPresetAsync()
    {
        UpdatePresetData();
        var pageVM = GlobalDependencies.ServiceProvider.GetRequiredService<ChatServicePageViewModel>();
        await _storageService.AddOrUpdateChatGroupPresetAsync(Data.Data);
        pageVM.ResetGroupsCommand.Execute(default);
        GlobalDependencies.ServiceProvider.GetRequiredService<AppViewModel>().ForceUpdatePresetAvatar(Data.Data.Id);
        CloseRequested?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private void AddAgent(ChatPresetItemViewModel agent)
    {
        if (SelectedAgents.Contains(agent))
        {
            return;
        }

        SelectedAgents.Add(agent);
    }

    [RelayCommand]
    private void RemoveAgent(ChatPresetItemViewModel agent)
        => SelectedAgents.Remove(agent);

    private void UpdatePresetData()
    {
        Data.Data.Name = Name;
        Data.Data.MaxRounds = MaxRounds;
        Data.Data.Agents = SelectedAgents.Select(p => p.Data.Id).ToList();
        Data.Data.TerminateText = TerminateText.ToList();
    }

    private void CheckAgentCount()
    {
        IsAgentsEmpty = TotalAgents.Count == 0;
        IsNoAgentSelected = SelectedAgents.Count == 0;
    }
}
