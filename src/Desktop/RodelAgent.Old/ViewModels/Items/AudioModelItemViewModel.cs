// Copyright (c) Richasy. All rights reserved.

using RodelAudio.Models.Client;

namespace RodelAgent.UI.ViewModels.Items;

/// <summary>
/// 绘图模型项视图模型.
/// </summary>
public sealed partial class AudioModelItemViewModel : ViewModelBase<AudioModel>
{
    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private string _id;

    [ObservableProperty]
    private bool _isSelected;

    /// <summary>
    /// Initializes a new instance of the <see cref="AudioModelItemViewModel"/> class.
    /// </summary>
    public AudioModelItemViewModel(AudioModel model)
        : base(model)
    {
        Name = model.DisplayName;
        Id = model.Id;
    }
}
