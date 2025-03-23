// Copyright (c) Richasy. All rights reserved.

using RodelAgent.Interfaces;
using RodelAgent.Models.Feature;
using RodelAgent.UI.Controls.Chat;
using RodelAgent.UI.ViewModels.Core;
using RodelAgent.UI.ViewModels.View;

namespace RodelAgent.UI.ViewModels.Items;

/// <summary>
/// 对话群组视图模型.
/// </summary>
public sealed partial class ChatGroupItemViewModel : ViewModelBase<ChatGroup>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChatGroupItemViewModel"/> class.
    /// </summary>
    public ChatGroupItemViewModel(ChatGroup data)
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
        this.Get<ChatGroupConfigViewModel>().SetData(this);
        var dialog = new ChatGroupConfigDialog();
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

        pageVM.Groups.Remove(this);
        await this.Get<IStorageService>().RemoveChatGroupAsync(Data.Id);
    }
}
