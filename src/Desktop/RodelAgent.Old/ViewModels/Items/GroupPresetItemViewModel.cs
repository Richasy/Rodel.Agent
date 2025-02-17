// Copyright (c) Richasy. All rights reserved.

using RodelChat.Models.Client;

namespace RodelAgent.UI.ViewModels.Items;

/// <summary>
/// 群组预设项视图模型.
/// </summary>
public sealed partial class GroupPresetItemViewModel : ViewModelBase<ChatGroupPreset>
{
    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private bool _isSelected;

    /// <summary>
    /// Initializes a new instance of the <see cref="GroupPresetItemViewModel"/> class.
    /// </summary>
    public GroupPresetItemViewModel(ChatGroupPreset data)
        : base(data)
    {
        Name = data.Name;
    }
}
