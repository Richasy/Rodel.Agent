// Copyright (c) Richasy. All rights reserved.

using Windows.Storage;
using Windows.System;

namespace RodelAgent.UI.Controls.Settings;

/// <summary>
/// 日志设置控件.
/// </summary>
public sealed partial class LoggerSettingControl : SettingsPageControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LoggerSettingControl"/> class.
    /// </summary>
    public LoggerSettingControl() => InitializeComponent();

    private static string LoggerFolder => Path.Combine(ApplicationData.Current.LocalFolder.Path, "Logger");

    private async void OnOpenLoggerFolderButtonClick(object sender, RoutedEventArgs e)
    {
        var folder = await StorageFolder.GetFolderFromPathAsync(LoggerFolder).AsTask();
        _ = await Launcher.LaunchFolderAsync(folder).AsTask();
    }
}
