// Copyright (c) Rodel. All rights reserved.

using Microsoft.UI.Xaml.Media.Imaging;
using RodelAgent.UI.Toolkits;
using Windows.Storage;

namespace RodelAgent.UI.Controls;

/// <summary>
/// 插件头像.
/// </summary>
public sealed partial class PluginAvatar : UserControl
{
    /// <summary>
    /// <see cref="Id"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty IdProperty =
        DependencyProperty.Register(nameof(Id), typeof(string), typeof(PluginAvatar), new PropertyMetadata(default, new PropertyChangedCallback(OnIdChanged)));

    /// <summary>
    /// Initializes a new instance of the <see cref="PluginAvatar"/> class.
    /// </summary>
    public PluginAvatar()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    /// <summary>
    /// 插件 ID.
    /// </summary>
    public string Id
    {
        get => (string)GetValue(IdProperty);
        set => SetValue(IdProperty, value);
    }

    private static void OnIdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var instance = d as PluginAvatar;
        instance?.CheckAvatarAsync();
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
        => CheckAvatarAsync();

    private async void CheckAvatarAsync()
    {
        if (!IsLoaded || Avatar == null)
        {
            return;
        }

        var avatarPath = AppToolkit.GetPluginAvatarPath(Id);
        if (File.Exists(avatarPath))
        {
            var bitmap = new BitmapImage();
            var file = await StorageFile.GetFileFromPathAsync(avatarPath);
            using var stream = await file.OpenReadAsync();
            await bitmap.SetSourceAsync(stream);
            bitmap.DecodePixelWidth = Convert.ToInt32(Math.Max(ActualWidth * 2, 96));
            Avatar.Source = bitmap;
            Avatar.Visibility = Visibility.Visible;
            DefaultIcon.Visibility = Visibility.Collapsed;
        }
        else
        {
            Avatar.Visibility = Visibility.Collapsed;
            DefaultIcon.Visibility = Visibility.Visible;
        }
    }
}
