// Copyright (c) Rodel. All rights reserved.

using Windows.Storage;
using Windows.System;

namespace RodelAgent.UI.Controls.Settings;

/// <summary>
/// 应用日志设置部分.
/// </summary>
public sealed partial class AppLogSettingSection : SettingSectionBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppLogSettingSection"/> class.
    /// </summary>
    public AppLogSettingSection() => InitializeComponent();

    private async void OnItemClickAsync(object sender, RoutedEventArgs e)
    {
        var folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Logger", CreationCollisionOption.OpenIfExists).AsTask();
        _ = await Launcher.LaunchFolderAsync(folder);
    }
}
