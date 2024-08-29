// Copyright (c) Rodel. All rights reserved.

using System.Collections.Specialized;
using RodelAgent.Interfaces;
using RodelAgent.UI.Toolkits;
using RodelAgent.UI.ViewModels.Components;
using Windows.Storage;
using Windows.System;

namespace RodelAgent.UI.ViewModels.Pages;

/// <summary>
/// 音频服务页面视图模型.
/// </summary>
public sealed partial class AudioServicePageViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AudioServicePageViewModel"/> class.
    /// </summary>
    public AudioServicePageViewModel(
        IStorageService storageService,
        ILogger<AudioServicePageViewModel> logger,
        AudioSessionViewModel sessionVM)
    {
        _storageService = storageService;
        _logger = logger;
        Session = sessionVM;

        IsAvailableServicesEmpty = AvailableServices.Count == 0;
        IsHistoryEmpty = History.Count == 0;
        HistoryColumnWidth = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.AudioHistoryColumnWidth, 250d);
        IsHistoryColumnManualHide = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.IsAudioHistoryColumnManualHide, false);

        History.CollectionChanged += OnHistoryCollectionChanged;
    }

    [RelayCommand]
    private static async Task OpenAudioFolderAsync()
    {
        var dir = AppToolkit.GetSpeechFolderPath();
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
            SettingsToolkit.WriteLocalSetting(Models.Constants.SettingNames.AudioHistoryColumnWidth, value);
        }
    }

    partial void OnIsHistoryColumnManualHideChanged(bool value)
    {
        SettingsToolkit.WriteLocalSetting(Models.Constants.SettingNames.IsAudioHistoryColumnManualHide, value);
    }
}
