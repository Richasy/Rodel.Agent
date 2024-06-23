// Copyright (c) Rodel. All rights reserved.

using Microsoft.UI.Xaml.Media.Imaging;
using RodelAgent.Interfaces;
using RodelAgent.Statics;
using RodelAgent.UI.Toolkits;
using RodelAgent.UI.ViewModels;
using Windows.Storage;

namespace RodelAgent.UI.Controls;

/// <summary>
/// 预设头像.
/// </summary>
public sealed partial class PresetAvatar : UserControl
{
    /// <summary>
    /// <see cref="PresetId"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty PresetIdProperty =
        DependencyProperty.Register(nameof(PresetId), typeof(string), typeof(PresetAvatar), new PropertyMetadata(default, new PropertyChangedCallback(OnPresetIdChanged)));

    /// <summary>
    /// <see cref="DefaultSymbol"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty DefaultSymbolProperty =
        DependencyProperty.Register(nameof(DefaultSymbol), typeof(FluentIcons.Common.Symbol), typeof(PresetAvatar), new PropertyMetadata(FluentIcons.Common.Symbol.Bot));

    /// <summary>
    /// Initializes a new instance of the <see cref="PresetAvatar"/> class.
    /// </summary>
    public PresetAvatar()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    /// <summary>
    /// 预设 ID.
    /// </summary>
    public string PresetId
    {
        get => (string)GetValue(PresetIdProperty);
        set => SetValue(PresetIdProperty, value);
    }

    /// <summary>
    /// 默认图标.
    /// </summary>
    public FluentIcons.Common.Symbol DefaultSymbol
    {
        get => (FluentIcons.Common.Symbol)GetValue(DefaultSymbolProperty);
        set => SetValue(DefaultSymbolProperty, value);
    }

    private static void OnPresetIdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var instance = d as PresetAvatar;
        instance?.CheckAvatarAsync();
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        CheckAvatarAsync();
        GlobalDependencies.ServiceProvider.GetRequiredService<AppViewModel>().PresetAvatarUpdateRequested += OnPresetAvatarUpdateRequested;
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
        => GlobalDependencies.ServiceProvider.GetRequiredService<AppViewModel>().PresetAvatarUpdateRequested -= OnPresetAvatarUpdateRequested;

    private void OnPresetAvatarUpdateRequested(object sender, string e)
    {
        if (PresetId == e)
        {
            CheckAvatarAsync();
        }
    }

    private async void CheckAvatarAsync()
    {
        if (!IsLoaded || AgentAvatar == null || string.IsNullOrEmpty(PresetId))
        {
            return;
        }

        var preset = await GlobalDependencies.ServiceProvider.GetRequiredService<IStorageService>().GetChatSessionPresetByIdAsync(PresetId);
        if (!string.IsNullOrEmpty(preset.Emoji))
        {
            AgentAvatar.Visibility = Visibility.Collapsed;
            DefaultIcon.Visibility = Visibility.Collapsed;
            EmojiAvatar.Visibility = Visibility.Visible;
            var emoji = EmojiStatics.GetEmojis().FirstOrDefault(x => x.Unicode == preset.Emoji);
            EmojiAvatar.Text = emoji?.ToEmoji();
        }
        else
        {
            var avatarPath = AppToolkit.GetPresetAvatarPath(PresetId);
            if (File.Exists(avatarPath))
            {
                var bitmap = new BitmapImage();
                var file = await StorageFile.GetFileFromPathAsync(avatarPath);
                using var stream = await file.OpenReadAsync();
                await bitmap.SetSourceAsync(stream);
                bitmap.DecodePixelWidth = Convert.ToInt32(Math.Max(ActualWidth * 2, 96));
                AgentAvatar.Source = bitmap;
                AgentAvatar.Visibility = Visibility.Visible;
                DefaultIcon.Visibility = Visibility.Collapsed;
                EmojiAvatar.Visibility = Visibility.Collapsed;
            }
            else
            {
                AgentAvatar.Visibility = Visibility.Collapsed;
                EmojiAvatar.Visibility = Visibility.Collapsed;
                DefaultIcon.Visibility = Visibility.Visible;
            }
        }
    }
}
