// Copyright (c) Rodel. All rights reserved.

using Microsoft.UI.Xaml.Media.Imaging;
using RodelAgent.UI.Toolkits;

namespace RodelAgent.UI.Controls.Draw;

/// <summary>
/// 本地绘图图片.
/// </summary>
public sealed partial class LocalDrawImage : UserControl
{
    /// <summary>
    /// <see cref="Id"/> 依赖属性.
    /// </summary>
    public static readonly DependencyProperty IdProperty =
        DependencyProperty.Register(nameof(Id), typeof(string), typeof(LocalDrawImage), new PropertyMetadata(default, new PropertyChangedCallback(OnIdChanged)));

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalDrawImage"/> class.
    /// </summary>
    public LocalDrawImage()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    /// <summary>
    /// 图片Id.
    /// </summary>
    public string Id
    {
        get => (string)GetValue(IdProperty);
        set => SetValue(IdProperty, value);
    }

    private static void OnIdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var instance = d as LocalDrawImage;
        if (instance == null)
        {
            return;
        }

        instance.ResetImage(e.NewValue as string);
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
        => ResetImage(Id);

    private void ResetImage(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return;
        }

        var imagePath = AppToolkit.GetDrawPicturePath(id);
        if (!File.Exists(imagePath))
        {
            return;
        }

        var bitmap = new BitmapImage(new Uri(imagePath));
        LocalImage.Source = bitmap;
    }
}
