// Copyright (c) Richasy. All rights reserved.

using Microsoft.UI.Xaml.Media.Imaging;

namespace RodelAgent.UI.Controls.Draw;

/// <summary>
/// …˙Õº.
/// </summary>
public sealed partial class DrawImage : LayoutUserControlBase
{
    /// <summary>
    /// <see cref="Source"/> “¿¿µ Ù–‘.
    /// </summary>
    public static readonly DependencyProperty SourceProperty =
        DependencyProperty.Register(nameof(Source), typeof(Uri), typeof(DrawImage), new PropertyMetadata(default, new PropertyChangedCallback(OnUriChanged)));

    /// <summary>
    /// <see cref="Proportion"/> “¿¿µ Ù–‘.
    /// </summary>
    public static readonly DependencyProperty ProportionProperty =
        DependencyProperty.Register(nameof(Proportion), typeof(double), typeof(DrawImage), new PropertyMetadata(1d));

    /// <summary>
    /// Initializes a new instance of the <see cref="DrawImage"/> class.
    /// </summary>
    public DrawImage() => InitializeComponent();

    /// <summary>
    /// Õº∆¨Id.
    /// </summary>
    public Uri Source
    {
        get => (Uri)GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    /// <summary>
    /// ±»¿˝.
    /// </summary>
    public double Proportion
    {
        get => (double)GetValue(ProportionProperty);
        set => SetValue(ProportionProperty, value);
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
        var perferHeight = width / Proportion;
        if (Math.Abs(perferHeight - height) > 2)
        {
            LocalImage.Height = perferHeight;
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
