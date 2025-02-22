// Copyright (c) Richasy. All rights reserved.

using Humanizer;
using RodelAgent.Interfaces;
using RodelAgent.Models.Common;
using RodelAgent.UI.Models.Constants;
using RodelAgent.UI.Toolkits;
using RodelAgent.UI.ViewModels.Core;
using RodelAgent.UI.ViewModels.View;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System;

namespace RodelAgent.UI.ViewModels.Items;

/// <summary>
/// 绘图记录项视图模型.
/// </summary>
public sealed partial class DrawRecordItemViewModel : ViewModelBase<DrawRecord>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DrawRecordItemViewModel"/> class.
    /// </summary>
    public DrawRecordItemViewModel(DrawRecord data)
        : base(data)
    {
        var imagePath = AppToolkit.GetDrawPicturePath(data.Id);
        Image = new Uri($"file://{imagePath}");
        Time = data.Time.Humanize();
        Proportion = (double)(data.Size?.Width ?? 1d) / data.Size?.Height ?? 1d;
        Width = data.Size?.Width ?? 1;
        Height = data.Size?.Height ?? 1;
    }

    /// <summary>
    /// 图片.
    /// </summary>
    [ObservableProperty]
    public partial Uri? Image { get; set; }

    /// <summary>
    /// 日期.
    /// </summary>
    [ObservableProperty]
    public partial string Time { get; set; }

    /// <summary>
    /// 比例.
    /// </summary>
    [ObservableProperty]
    public partial double Proportion { get; set; }

    /// <summary>
    /// 宽度.
    /// </summary>
    public int Width { get; }

    /// <summary>
    /// 高度.
    /// </summary>
    public int Height { get; }

    [RelayCommand]
    private void Display()
        => this.Get<DrawPageViewModel>().ShowRecordCommand.Execute(Data);

    [RelayCommand]
    private async Task OpenAsync()
    {
        var filePath = AppToolkit.GetDrawPicturePath(Data.Id);
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
    private async Task CopyAsync()
    {
        var filePath = AppToolkit.GetDrawPicturePath(Data.Id);
        if (File.Exists(filePath))
        {
            var file = await StorageFile.GetFileFromPathAsync(filePath);
            var dp = new DataPackage();
            dp.SetBitmap(RandomAccessStreamReference.CreateFromFile(file));
            Clipboard.SetContent(dp);
            this.Get<AppViewModel>().ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(StringNames.Copied), InfoType.Success));
        }
        else
        {
            this.Get<AppViewModel>().ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(StringNames.FileNotFound), InfoType.Error));
        }
    }

    [RelayCommand]
    private async Task DeleteAsync()
    {
        await this.Get<IStorageService>().RemoveDrawSessionAsync(Data.Id);
        this.Get<DrawPageViewModel>().ReloadHistoryCommand.Execute(default);
    }
}
