// Copyright (c) Richasy. All rights reserved.

using Humanizer;
using RodelAgent.Models.Common;
using RodelAgent.UI.Toolkits;

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
}
