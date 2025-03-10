// Copyright (c) Richasy. All rights reserved.

using RodelAgent.Models.Feature;
using RodelAgent.UI.Controls.Chat;
using RodelAgent.UI.ViewModels.Core;

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
}
