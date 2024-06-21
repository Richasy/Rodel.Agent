// Copyright (c) Rodel. All rights reserved.

using RodelAgent.UI.Controls.Startup;
using RodelChat.Models.Client;

namespace RodelAgent.UI.ViewModels.Items;

/// <summary>
/// 聊天模型项视图模型.
/// </summary>
public sealed partial class ChatModelItemViewModel : ViewModelBase<ChatModel>
{
    private readonly Action<ChatModelItemViewModel> _deleteAction;

    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private string _id;

    [ObservableProperty]
    private bool _isSupportFileUpload;

    [ObservableProperty]
    private bool _isSupportTool;

    [ObservableProperty]
    private bool _isSupportVision;

    [ObservableProperty]
    private long _contextLength;

    [ObservableProperty]
    private bool _isSelected;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatModelItemViewModel"/> class.
    /// </summary>
    public ChatModelItemViewModel(ChatModel model, Action<ChatModelItemViewModel> deleteAction = null)
        : base(model)
    {
        Name = model.DisplayName;
        Id = model.Id;
        IsSupportFileUpload = model.IsSupportFileUpload;
        IsSupportTool = model.IsSupportTool;
        IsSupportVision = model.IsSupportVision;
        ContextLength = model.ContextLength;
        _deleteAction = deleteAction;
    }

    [RelayCommand]
    private void Delete()
        => _deleteAction?.Invoke(this);

    [RelayCommand]
    private async Task ModifyAsync()
    {
        var dialog = new CustomChatModelDialog(Data);
        var dialogResult = await dialog.ShowAsync();
        if (dialogResult == ContentDialogResult.Primary)
        {
            Name = dialog.Model.DisplayName;
            Id = dialog.Model.Id;
            IsSupportFileUpload = dialog.Model.IsSupportFileUpload;
            IsSupportTool = dialog.Model.IsSupportTool;
            IsSupportVision = dialog.Model.IsSupportVision;
            ContextLength = dialog.Model.ContextLength;
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

    partial void OnIsSupportFileUploadChanged(bool value)
    {
        if (Data.IsSupportFileUpload != value)
        {
            Data.IsSupportFileUpload = value;
        }
    }

    partial void OnIsSupportToolChanged(bool value)
    {
        if (Data.IsSupportTool != value)
        {
            Data.IsSupportTool = value;
        }
    }

    partial void OnIsSupportVisionChanged(bool value)
    {
        if (Data.IsSupportVision != value)
        {
            Data.IsSupportVision = value;
        }
    }

    partial void OnContextLengthChanged(long value)
    {
        if (Data.ContextLength != value && value >= 0)
        {
            Data.ContextLength = value;
        }
    }
}
