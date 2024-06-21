// Copyright (c) Rodel. All rights reserved.

using RodelChat.Models.Client;

namespace RodelAgent.UI.ViewModels.Items;

/// <summary>
/// 聊天预设项视图模型.
/// </summary>
public sealed partial class ChatPresetItemViewModel : ViewModelBase<ChatSessionPreset>
{
    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private bool _isSelected;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatPresetItemViewModel"/> class.
    /// </summary>
    public ChatPresetItemViewModel(ChatSessionPreset data)
        : base(data)
    {
        Name = data.Name;
    }
}
