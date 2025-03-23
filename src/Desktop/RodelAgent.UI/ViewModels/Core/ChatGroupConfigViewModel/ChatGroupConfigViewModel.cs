// Copyright (c) Richasy. All rights reserved.

using RodelAgent.Interfaces;
using RodelAgent.Models.Feature;
using RodelAgent.UI.ViewModels.Items;
using RodelAgent.UI.ViewModels.View;

namespace RodelAgent.UI.ViewModels.Core;

/// <summary>
/// 聊天群组配置视图模型.
/// </summary>
public sealed partial class ChatGroupConfigViewModel : ViewModelBase
{
    public ChatGroupConfigViewModel(
        IStorageService storageService)
    {
        _storageService = storageService;
        SelectedAgents.CollectionChanged += (_, _) => CheckAgentCount();
        CheckAgentCount();
    }

    public void InjectFunc(
        Func<string>? getEmoji)
    {
        _getEmoji = getEmoji;
    }

    public void SetData(ChatGroupItemViewModel data)
    {
        Group = data.Data;
        Name = data.Name;
        MaxRounds = data.Data.MaxRounds;
        TotalAgents.Clear();
        SelectedAgents.Clear();
        TerminateSequence.Clear();
        foreach (var agent in this.Get<ChatPageViewModel>().Agents)
        {
            TotalAgents.Add(new ChatAgentItemViewModel(agent.Data));
        }

        foreach (var item in Group.Agents ?? [])
        {
            var a = TotalAgents.FirstOrDefault(p => p.Data.Id == item);
            if (a == null)
            {
                continue;
            }

            SelectedAgents.Add(a);
        }

        if (Group.TerminateSequence != null)
        {
            foreach (var item in Group.TerminateSequence)
            {
                TerminateSequence.Add(item);
            }
        }
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        UpdatePresetData();
        var pageVM = this.Get<ChatPageViewModel>();
        await _storageService.AddOrUpdateChatGroupAsync(Group!);
        pageVM.ReloadAvailableGroupsCommand.Execute(default);
        var sessionVM = this.Get<ChatSessionViewModel>();
        sessionVM.ForceReloadLogoCommand.Execute(default);
        sessionVM.TryReloadGroupCommand.Execute(Group!);
        CloseRequested?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private void AddAgent(ChatAgentItemViewModel agent)
    {
        if (SelectedAgents.Contains(agent))
        {
            return;
        }

        SelectedAgents.Add(agent);
    }

    [RelayCommand]
    private void RemoveAgent(ChatAgentItemViewModel agent)
        => SelectedAgents.Remove(agent);

    private void UpdatePresetData()
    {
        Group ??= new ChatGroup();
        if (string.IsNullOrEmpty(Group.Id))
        {
            Group.Id = Guid.NewGuid().ToString("N");
        }

        Group.Name = Name ?? string.Empty;
        Group.MaxRounds = MaxRounds;
        Group.Emoji = _getEmoji?.Invoke() ?? string.Empty;
        Group.Agents = [.. SelectedAgents.Select(p => p.Data.Id)];
        Group.TerminateSequence = [.. TerminateSequence];
    }

    private void CheckAgentCount()
    {
        IsAgentsEmpty = TotalAgents.Count == 0;
        IsNoAgentSelected = SelectedAgents.Count == 0;
    }
}
