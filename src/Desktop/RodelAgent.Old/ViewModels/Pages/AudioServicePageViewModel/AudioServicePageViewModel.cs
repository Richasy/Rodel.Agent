// Copyright (c) Richasy. All rights reserved.

using RodelAgent.Interfaces;
using RodelAgent.UI.Pages;
using RodelAgent.UI.Toolkits;
using RodelAgent.UI.ViewModels.Components;
using System.Collections.Specialized;
using Windows.Storage;
using Windows.System;

namespace RodelAgent.UI.ViewModels.Pages;

/// <summary>
/// 音频服务页面视图模型.
/// </summary>
public sealed partial class AudioServicePageViewModel : LayoutPageViewModelBase
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

        History.CollectionChanged += OnHistoryCollectionChanged;
    }

    /// <inheritdoc/>
    protected override string GetPageKey()
        => nameof(AudioServicePage);

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
}
