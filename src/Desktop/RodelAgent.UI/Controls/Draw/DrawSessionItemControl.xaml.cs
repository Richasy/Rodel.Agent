// Copyright (c) Rodel. All rights reserved.

using RodelAgent.UI.Toolkits;
using RodelAgent.UI.ViewModels.Components;
using RodelAgent.UI.ViewModels.Items;
using RodelAgent.UI.ViewModels.Pages;

namespace RodelAgent.UI.Controls.Draw;

/// <summary>
/// 会话项控件.
/// </summary>
public sealed partial class DrawSessionItemControl : DrawSessionItemControlBase
{
    private double _xi = 1;

    /// <summary>
    /// Initializes a new instance of the <see cref="DrawSessionItemControl"/> class.
    /// </summary>
    public DrawSessionItemControl() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnViewModelChanged(DrawSessionItemViewModel? oldValue, DrawSessionItemViewModel? newValue)
        => Initialize();

    /// <inheritdoc/>
    protected override void OnControlLoaded()
        => Initialize();

    private DrawSessionViewModel GetSessionViewModel()
        => this.Get<DrawServicePageViewModel>().Session;

    private void Initialize()
    {
        if (ViewModel == null || DateBlock == null)
        {
            return;
        }

        var time = !ViewModel.Data.Time.HasValue
            ? "-/-"
            : ViewModel.Data.Time.Value.ToString("yyyy-MM-dd HH:mm:ss");
        DateBlock.Text = time;

        _xi = 1;
        var sp = ViewModel.Data.Request.Size.Split("x");
        if (sp.Length == 2)
        {
            var width = double.Parse(sp[0]);
            var height = double.Parse(sp[1]);
            if (width > 0 && height > 0)
            {
                _xi = width / height;
            }
        }

        CheckImageSize();
    }

    private void OnImageSizeChanged(object sender, SizeChangedEventArgs e)
        => CheckImageSize();

    private void CheckImageSize()
    {
        if (DisplayImage == null || DisplayImage.ActualWidth < 1)
        {
            return;
        }

        var width = DisplayImage.ActualWidth;
        var targetHeight = width / _xi;
        if (Math.Abs(DisplayImage.MinHeight - targetHeight) > 1)
        {
            DisplayImage.MinHeight = targetHeight;
        }
    }

    private void OnSessionClick(object sender, RoutedEventArgs e)
        => GetSessionViewModel().LoadSessionCommand.Execute(ViewModel);

    private void OnCopyItemClick(object sender, RoutedEventArgs e)
        => GetSessionViewModel().CopyImageCommand.Execute(AppToolkit.GetDrawPicturePath(ViewModel.Data.Id));

    private void OnOpenItemClick(object sender, RoutedEventArgs e)
        => GetSessionViewModel().OpenImageCommand.Execute(AppToolkit.GetDrawPicturePath(ViewModel.Data.Id));

    private void OnDeleteItemClick(object sender, RoutedEventArgs e)
    {
        var pageVM = this.Get<DrawServicePageViewModel>();
        pageVM.DeleteHistoryItemCommand.Execute(ViewModel);
    }
}

/// <summary>
/// 会话项控件基类.
/// </summary>
public abstract class DrawSessionItemControlBase : LayoutUserControlBase<DrawSessionItemViewModel>
{
}
