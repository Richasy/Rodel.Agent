// Copyright (c) Richasy. All rights reserved.

using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;

namespace RodelAgent.UI.Controls.Draw;

/// <summary>
/// 生图.
/// </summary>
public sealed partial class DrawImage : LayoutUserControlBase
{
    /// <summary>
    /// <see cref="Source"/> 依赖属性.
    /// </summary>
    public static readonly DependencyProperty SourceProperty =
        DependencyProperty.Register(nameof(Source), typeof(Uri), typeof(DrawImage), new PropertyMetadata(default, new PropertyChangedCallback(OnUriChanged)));

    /// <summary>
    /// <see cref="Proportion"/> 依赖属性.
    /// </summary>
    public static readonly DependencyProperty ProportionProperty =
        DependencyProperty.Register(nameof(Proportion), typeof(double), typeof(DrawImage), new PropertyMetadata(1d));

    /// <summary>
    /// <see cref="Stretch"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty StretchProperty =
        DependencyProperty.Register(nameof(Stretch), typeof(Stretch), typeof(DrawImage), new PropertyMetadata(Stretch.UniformToFill));

    /// <summary>
    /// Initializes a new instance of the <see cref="DrawImage"/> class.
    /// </summary>
    public DrawImage() => InitializeComponent();

    /// <summary>
    /// 图片Id.
    /// </summary>
    public Uri Source
    {
        get => (Uri)GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    /// <summary>
    /// 比例.
    /// </summary>
    public double Proportion
    {
        get => (double)GetValue(ProportionProperty);
        set => SetValue(ProportionProperty, value);
    }

    /// <summary>
    /// 扩展方式.
    /// </summary>
    public Stretch Stretch
    {
        get => (Stretch)GetValue(StretchProperty);
        set => SetValue(StretchProperty, value);
    }

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        ResetImage(Source);
        ResetSize();
        SizeChanged += OnSizeChanged;
    }

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
        => SizeChanged -= OnSizeChanged;

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (e.NewSize.Width > 0 && e.NewSize.Height > 0)
        {
            ResetSize();
        }
    }

    private static void OnUriChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not DrawImage instance)
        {
            return;
        }

        instance.ResetImage(e.NewValue as Uri);
    }

    private void ResetSize()
    {
        var width = ActualWidth;
        var height = ActualHeight;
        var preferHeight = width / Proportion;
        if (!double.IsInfinity(preferHeight) && Math.Abs(preferHeight - height) > 2)
        {
            LocalImage.Height = preferHeight;
        }
    }

    private void ResetImage(Uri? uri)
    {
        if (uri == null)
        {
            return;
        }

        var bitmap = new BitmapImage(uri);
        LocalImage.Source = bitmap;
    }
}
