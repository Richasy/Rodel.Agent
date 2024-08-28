// Copyright (c) Rodel. All rights reserved.

using Microsoft.SemanticKernel;
using RodelAgent.UI.Models.Constants;
using RodelAgent.UI.Toolkits;
using Windows.Storage;
using Windows.System;

namespace RodelAgent.UI.ViewModels.Items;

/// <summary>
/// 聊天插件项视图模型.
/// </summary>
public sealed partial class ChatPluginItemViewModel : ViewModelBase<KernelPlugin>
{
    private readonly Action<ChatPluginItemViewModel> _deleteCheckAction;
    private readonly ILogger<ChatPluginItemViewModel> _logger;

    [ObservableProperty]
    private int _functionCount;

    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private string _id;

    [ObservableProperty]
    private string _description;

    [ObservableProperty]
    private bool _isDeleting;

    [ObservableProperty]
    private bool _isSelected;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatPluginItemViewModel"/> class.
    /// </summary>
    public ChatPluginItemViewModel(
        KernelPlugin data,
        string id,
        Action<ChatPluginItemViewModel> deleteCheckAction)
        : base(data)
    {
        FunctionCount = data.FunctionCount;
        Name = data.Name;
        Description = data.Description;
        Id = id;
        _deleteCheckAction = deleteCheckAction;
        _logger = this.Get<ILogger<ChatPluginItemViewModel>>();

        foreach (var func in data)
        {
            Functions.Add(new ChatPluginFunctionItemViewModel(func));
        }
    }

    /// <summary>
    /// 方法列表.
    /// </summary>
    public ObservableCollection<ChatPluginFunctionItemViewModel> Functions { get; } = new();

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is ChatPluginItemViewModel model && base.Equals(obj) && Id == model.Id;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), Id);

    [RelayCommand]
    private async Task OpenFolderAsync()
    {
        var destFolder = GetCurrentPluginFolder();
        if (!Directory.Exists(destFolder))
        {
            await this.Get<AppViewModel>().ShowMessageDialogAsync(StringNames.PluginFolderNotFound);
            return;
        }

        var folder = await StorageFolder.GetFolderFromPathAsync(destFolder);
        await Launcher.LaunchFolderAsync(folder);
    }

    [RelayCommand]
    private void DeletePlugin()
    {
        IsDeleting = true;
        _deleteCheckAction(this);
    }

    [RelayCommand]
    private void CancelDeletePlugin()
    {
        IsDeleting = false;
        _deleteCheckAction(this);
    }

    private string GetCurrentPluginFolder()
    {
        var actualId = Id.Split("<|>").First();
        var pluginFolder = Path.Combine(AppToolkit.GetChatPluginFolder(), actualId);
        return pluginFolder;
    }
}
