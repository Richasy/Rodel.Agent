// Copyright (c) Richasy. All rights reserved.

using Humanizer;
using RodelAgent.Interfaces;
using RodelAgent.Models.Common;
using RodelAgent.UI.Models.Constants;
using RodelAgent.UI.Toolkits;
using RodelAgent.UI.ViewModels.Core;
using RodelAgent.UI.ViewModels.View;
using Windows.Storage;
using Windows.System;

namespace RodelAgent.UI.ViewModels.Items;

public sealed partial class AudioRecordItemViewModel : ViewModelBase<AudioRecord>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AudioRecordItemViewModel"/> class.
    /// </summary>
    public AudioRecordItemViewModel(AudioRecord data)
        : base(data)
    {
        AudioPath = AppToolkit.GetAudioPath(data.Id);
        Time = data.Time.Humanize();
    }

    /// <summary>
    /// 图片.
    /// </summary>
    [ObservableProperty]
    public partial string AudioPath { get; set; }

    /// <summary>
    /// 日期.
    /// </summary>
    [ObservableProperty]
    public partial string Time { get; set; }

    [RelayCommand]
    private void Display()
        => this.Get<AudioPageViewModel>().ShowRecordCommand.Execute(Data);

    [RelayCommand]
    private async Task OpenAsync()
    {
        var filePath = AppToolkit.GetAudioPath(Data.Id);
        if (File.Exists(filePath))
        {
            var file = await StorageFile.GetFileFromPathAsync(filePath);
            await Launcher.LaunchFileAsync(file);
        }
        else
        {
            this.Get<AppViewModel>().ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(StringNames.FileNotFound), InfoType.Error));
        }
    }

    [RelayCommand]
    private async Task DeleteAsync()
    {
        await this.Get<IStorageService>().RemoveAudioSessionAsync(Data.Id);
        var pageVM = this.Get<AudioPageViewModel>();
        pageVM.ReloadHistoryCommand.Execute(default);
        if (pageVM.AudioPath == AudioPath)
        {
            pageVM.AudioPath = string.Empty;
        }
    }
}
