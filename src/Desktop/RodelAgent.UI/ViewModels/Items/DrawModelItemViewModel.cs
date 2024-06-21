// Copyright (c) Rodel. All rights reserved.

using RodelAgent.UI.Controls.Startup;
using RodelDraw.Models.Client;

namespace RodelAgent.UI.ViewModels.Items;

/// <summary>
/// 绘图模型项视图模型.
/// </summary>
public sealed partial class DrawModelItemViewModel : ViewModelBase<DrawModel>
{
    private readonly Action<DrawModelItemViewModel> _deleteAction;

    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private string _id;

    [ObservableProperty]
    private bool _isSelected;

    /// <summary>
    /// Initializes a new instance of the <see cref="DrawModelItemViewModel"/> class.
    /// </summary>
    public DrawModelItemViewModel(DrawModel model, Action<DrawModelItemViewModel> deleteAction = null)
        : base(model)
    {
        Name = model.DisplayName;
        Id = model.Id;
        _deleteAction = deleteAction;
    }

    [RelayCommand]
    private void Delete()
        => _deleteAction?.Invoke(this);

    [RelayCommand]
    private async Task ModifyAsync()
    {
        var dialog = new CustomDrawModelDialog(Data);
        var dialogResult = await dialog.ShowAsync();
        if (dialogResult == ContentDialogResult.Primary)
        {
            Name = dialog.Model.DisplayName;
            Id = dialog.Model.Id;
        }
    }

    partial void OnNameChanged(string value)
    {
        if (Data.DisplayName != value)
        {
            Data.DisplayName = value;
        }
    }

    partial void OnIdChanged(string value)
    {
        if (Data.Id != value)
        {
            Data.Id = value;
        }
    }
}
