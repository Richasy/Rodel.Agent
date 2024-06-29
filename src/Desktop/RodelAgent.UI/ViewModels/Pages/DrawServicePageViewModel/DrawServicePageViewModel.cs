// Copyright (c) Rodel. All rights reserved.

using System.Collections.Specialized;
using RodelAgent.Interfaces;
using RodelAgent.UI.Toolkits;
using RodelAgent.UI.ViewModels.Components;
using Windows.Storage;
using Windows.System;

namespace RodelAgent.UI.ViewModels.Pages;

/// <summary>
/// 绘图服务页面视图模型.
/// </summary>
public sealed partial class DrawServicePageViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DrawServicePageViewModel"/> class.
    /// </summary>
    public DrawServicePageViewModel(
        IStorageService storageService,
        ILogger<DrawServicePageViewModel> logger,
        DrawSessionViewModel sessionVM)
    {
        _storageService = storageService;
        _logger = logger;
        Session = sessionVM;

        IsAvailableServicesEmpty = AvailableServices.Count == 0;
        IsHistoryEmpty = History.Count == 0;
        HistoryColumnWidth = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.DrawHistoryColumnWidth, 250d);
        IsHistoryColumnManualHide = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.IsDrawHistoryColumnManualHide, false);

        History.CollectionChanged += OnHistoryCollectionChanged;
    }

    [RelayCommand]
    private static async Task OpenDrawFolderAsync()
    {
        var dir = AppToolkit.GetDrawFolderPath();
        var folder = await StorageFolder.GetFolderFromPathAsync(dir);
        await Launcher.LaunchFolderAsync(folder);
    }

    private void OnHistoryCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        HistoryCount = History.Count;
        IsHistoryEmpty = History.Count == 0;
    }

    partial void OnHistoryColumnWidthChanged(double value)
    {
        if (value > 0)
        {
            SettingsToolkit.WriteLocalSetting(Models.Constants.SettingNames.DrawHistoryColumnWidth, value);
        }
    }

    partial void OnIsHistoryColumnManualHideChanged(bool value)
    {
        SettingsToolkit.WriteLocalSetting(Models.Constants.SettingNames.IsDrawHistoryColumnManualHide, value);
    }
}
