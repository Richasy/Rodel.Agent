// Copyright (c) Richasy. All rights reserved.

using Microsoft.UI.Xaml.Media.Imaging;
using RodelAgent.Interfaces;
using RodelAgent.Statics;
using RodelAgent.UI.Toolkits;
using Windows.Storage;

namespace RodelAgent.UI.Controls.Chat;

public sealed partial class ChatLogo : LayoutUserControlBase
{
    /// <summary>
    /// <see cref="Id"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty IdProperty =
        DependencyProperty.Register(nameof(Id), typeof(string), typeof(ChatLogo), new PropertyMetadata(default, new PropertyChangedCallback(OnIdChanged)));

    /// <summary>
    /// <see cref="DefaultSymbol"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty DefaultSymbolProperty =
        DependencyProperty.Register(nameof(DefaultSymbol), typeof(FluentIcons.Common.Symbol), typeof(ChatLogo), new PropertyMetadata(FluentIcons.Common.Symbol.Bot));

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatLogo"/> class.
    /// </summary>
    public ChatLogo() => InitializeComponent();

    /// <summary>
    /// 是否为群组聊天.
    /// </summary>
    public bool IsGroup { get; set; }

    /// <summary>
    /// 预设 ID.
    /// </summary>
    public string Id
    {
        get => (string)GetValue(IdProperty);
        set => SetValue(IdProperty, value);
    }

    /// <summary>
    /// 默认图标.
    /// </summary>
    public FluentIcons.Common.Symbol DefaultSymbol
    {
        get => (FluentIcons.Common.Symbol)GetValue(DefaultSymbolProperty);
        set => SetValue(DefaultSymbolProperty, value);
    }

    private static void OnIdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var instance = d as ChatLogo;
        instance?.CheckAvatar();
    }

    protected override void OnControlLoaded()
        => CheckAvatar();

    private async void CheckAvatar()
    {
        if (!IsLoaded || AgentAvatar == null || string.IsNullOrEmpty(Id))
        {
            return;
        }

        var emojiText = string.Empty;
        var storageService = this.Get<IStorageService>();
        if (IsGroup)
        {
            var group = await storageService.GetChatGroupByIdAsync(Id);
            emojiText = group?.Emoji;
        }
        else
        {
            var agent = (await storageService.GetChatAgentsAsync()).FirstOrDefault(p => p.Id == Id);
            emojiText = agent?.Emoji;
        }

        if (!string.IsNullOrEmpty(emojiText))
        {
            AgentAvatar.Visibility = Visibility.Collapsed;
            DefaultIcon.Visibility = Visibility.Collapsed;
            EmojiAvatar.Visibility = Visibility.Visible;
            var emoji = EmojiStatics.GetEmojis().Find(x => x.Unicode == emojiText);
            EmojiAvatar.Text = emoji?.ToEmoji();
        }
        else
        {
            var avatarPath = AppToolkit.GetPresetAvatarPath(Id);
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
