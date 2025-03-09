// Copyright (c) Richasy. All rights reserved.

using RodelAgent.Models.Feature;

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
}
