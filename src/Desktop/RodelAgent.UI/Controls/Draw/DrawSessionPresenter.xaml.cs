// Copyright (c) Rodel. All rights reserved.

using RodelAgent.UI.ViewModels.Components;
using RodelDraw.Models.Client;

namespace RodelAgent.UI.Controls.Draw;

/// <summary>
/// 绘图会话控件基类.
/// </summary>
public sealed partial class DrawSessionPresenter : DrawSessionControlBase
{
    private double _xi = 1;

    /// <summary>
    /// Initializes a new instance of the <see cref="DrawSessionPresenter"/> class.
    /// </summary>
    public DrawSessionPresenter()
    {
        InitializeComponent();
    }

    /// <inheritdoc/>
    protected override void OnViewModelChanged(DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue is DrawSessionViewModel oldViewModel)
        {
            oldViewModel.DataChanged -= OnDataChanged;
        }

        if (e.NewValue is DrawSessionViewModel newViewModel)
        {
            newViewModel.DataChanged += OnDataChanged;
        }
    }

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
        => ViewModel.DataChanged -= OnDataChanged;

    private void OnDataChanged(object sender, DrawSession e)
        => CheckImageSize();

    private void OnImageSizeChanged(object sender, SizeChangedEventArgs e)
        => CheckImageSize();

    private void InitSize()
    {
        _xi = 1;
        if (string.IsNullOrEmpty(ViewModel?.Size))
        {
            return;
        }

        var sp = ViewModel.Size.Split("x");
        if (sp.Length == 2)
        {
            var width = double.Parse(sp[0]);
            var height = double.Parse(sp[1]);
            if (width > 0 && height > 0)
            {
                _xi = width / height;
            }
        }
    }

    private void CheckImageSize()
    {
        if (DisplayImage == null || DisplayImage.ActualWidth < 1)
        {
            return;
        }

        InitSize();
        var width = DisplayImage.ActualWidth;
        var targetHeight = width / _xi;
        if (Math.Abs(DisplayImage.MinHeight - targetHeight) > 1)
        {
            DisplayImage.MinHeight = targetHeight;
        }
    }
}
