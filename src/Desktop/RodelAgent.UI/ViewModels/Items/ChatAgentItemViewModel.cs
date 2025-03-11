// Copyright (c) Richasy. All rights reserved.

using RodelAgent.Interfaces;
using RodelAgent.Models.Feature;
using RodelAgent.UI.Controls.Chat;
using RodelAgent.UI.Models.Constants;
using RodelAgent.UI.Toolkits;
using RodelAgent.UI.ViewModels.Core;
using RodelAgent.UI.ViewModels.View;

namespace RodelAgent.UI.ViewModels.Items;

/// <summary>
/// 对话助理视图模型.
/// </summary>
public sealed partial class ChatAgentItemViewModel : ViewModelBase<ChatAgent>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChatAgentItemViewModel"/> class.
    /// </summary>
    public ChatAgentItemViewModel(ChatAgent data)
        : base(data)
    {
        Name = data.Name;
    }

    [ObservableProperty]
    public partial bool IsSelected { get; set; }

    [ObservableProperty]
    public partial string Name { get; set; }

    [RelayCommand]
    private async Task ModifyAsync()
    {
        this.Get<ChatAgentConfigViewModel>().SetData(this);
        var dialog = new ChatAgentConfigDialog();
        await dialog.ShowAsync();
    }

    [RelayCommand]
    private async Task CreateDuplicateAsync()
    {
        var agent = Data.Clone();
        agent.Name = $"{agent.Name} - {ResourceToolkit.GetLocalizedString(StringNames.Duplicate)}";
        var vm = new ChatAgentItemViewModel(agent);
        this.Get<ChatAgentConfigViewModel>().SetData(vm);
        var dialog = new ChatAgentConfigDialog();
        await dialog.ShowAsync();
    }

    [RelayCommand]
    private async Task DeleteAsync()
    {
        var pageVM = this.Get<ChatPageViewModel>();
        if (IsSelected)
        {
            pageVM.SelectServiceCommand.Execute(pageVM.Services!.First());
        }

        pageVM.Agents.Remove(this);
        await this.Get<IStorageService>().RemoveChatAgentAsync(Data.Id);
    }
}
