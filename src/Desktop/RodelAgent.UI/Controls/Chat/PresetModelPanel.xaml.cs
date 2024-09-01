// Copyright (c) Rodel. All rights reserved.

using RodelAgent.Statics;
using RodelAgent.UI.Toolkits;
using RodelAgent.UI.ViewModels;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;

namespace RodelAgent.UI.Controls.Chat;

/// <summary>
/// 预设模型面板.
/// </summary>
public sealed partial class PresetModelPanel : ChatPresetControlBase
{
    private bool _avatarChanged = false;
    private bool _isEmojiAvatar = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="PresetModelPanel"/> class.
    /// </summary>
    public PresetModelPanel() => InitializeComponent();

    /// <summary>
    /// 保存头像到本地.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    public async Task SaveAvatarAsync()
    {
        if (Cropper.Visibility == Visibility.Collapsed || !_avatarChanged || _isEmojiAvatar)
        {
            return;
        }

        var avatarPath = AppToolkit.GetPresetAvatarPath(ViewModel.Data.Data.Id);
        if (!File.Exists(avatarPath))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(avatarPath));
            File.Create(avatarPath).Dispose();
        }

        var file = await StorageFile.GetFileFromPathAsync(avatarPath);
        using var stream = await file.OpenAsync(FileAccessMode.ReadWrite);
        await Cropper.SaveAsync(stream, CommunityToolkit.WinUI.Controls.BitmapFileFormat.Png, true);
    }

    /// <summary>
    /// 验证是否有效.
    /// </summary>
    /// <returns>是否有效.</returns>
    public bool IsValid()
        => !string.IsNullOrEmpty(ViewModel.Name);

    /// <inheritdoc/>
    protected override async void OnControlLoaded()
    {
        if (!string.IsNullOrEmpty(ViewModel.Data.Data.Emoji))
        {
            _isEmojiAvatar = true;
            var emoji = EmojiStatics.GetEmojis().FirstOrDefault(x => x.Unicode == ViewModel.Data.Data.Emoji);
            ShowEmoji(emoji);
            return;
        }

        var avatarPath = AppToolkit.GetPresetAvatarPath(ViewModel.Data.Data.Id);
        if (File.Exists(avatarPath))
        {
            var file = await StorageFile.GetFileFromPathAsync(avatarPath);
            await InitializeCropperAsync(file);
        }
    }

    private async void OnImageAreaTappedAsync(object sender, TappedRoutedEventArgs e)
        => await PickImageFileAsync();

    private async void OnImageAreaDropAsync(object sender, DragEventArgs e)
    {
        if (e.DataView.Contains(StandardDataFormats.StorageItems))
        {
            var items = await e.DataView.GetStorageItemsAsync();
            if (items.Count > 0 && items[0] is StorageFile file)
            {
                if (file.ContentType.Contains("image"))
                {
                    _avatarChanged = true;
                    await InitializeCropperAsync(file);
                }
            }
        }
    }

    private async void OnReplaceImageButtonClickAsync(object sender, RoutedEventArgs e)
        => await PickImageFileAsync();

    private void OnImageAreaDragOver(object sender, DragEventArgs e)
    {
        e.DragUIOverride.Caption = ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.AvatarDropTip);
        e.DragUIOverride.IsCaptionVisible = true;
        e.AcceptedOperation = DataPackageOperation.Move;
    }

    private async Task PickImageFileAsync()
    {
        var image = await FileToolkit.PickFileAsync(".png,.jpg,.bmp", GlobalDependencies.ServiceProvider.GetService<AppViewModel>().ActivatedWindow);
        if (image is null)
        {
            return;
        }

        _avatarChanged = true;
        await InitializeCropperAsync(image);
    }

    private async Task InitializeCropperAsync(StorageFile imageFile)
    {
        _isEmojiAvatar = false;
        ViewModel.Data.Data.Emoji = default;
        EmojiContainer.Visibility = Visibility.Collapsed;
        Cropper.Visibility = Visibility.Visible;
        ImagePlaceholderContainer.Visibility = Visibility.Collapsed;
        ReplaceImageButton.Visibility = Visibility.Visible;
        await Cropper.LoadImageFromFile(imageFile);
    }

    private void ShowEmoji(EmojiItem emoji)
    {
        _isEmojiAvatar = true;
        ViewModel.Data.Data.Emoji = emoji.Unicode;
        EmojiContainer.Visibility = Visibility.Visible;
        EmojiBlock.Text = emoji.ToEmoji();
        Cropper.Visibility = Visibility.Collapsed;
        ReplaceImageButton.Visibility = Visibility.Visible;
        ImagePlaceholderContainer.Visibility = Visibility.Collapsed;
    }

    private void OnEmojiClick(object sender, Statics.EmojiItem e)
    {
        EmojiFlyout.Hide();
        ShowEmoji(e);
    }

    private void OnEmojiAvatarButtonClick(object sender, RoutedEventArgs e)
        => FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
}
