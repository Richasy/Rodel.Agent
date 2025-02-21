// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.Toolkits;
using Windows.Storage;
using Windows.System;

namespace RodelAgent.UI.Controls.Startup;

/// <summary>
/// 启动页页脚.
/// </summary>
public sealed partial class StartupFooter : StartupPageControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StartupFooter"/> class.
    /// </summary>
    public StartupFooter() => InitializeComponent();

    private async void OnDocumentButtonClick(object sender, RoutedEventArgs e)
        => await Windows.System.Launcher.LaunchUriAsync(new Uri(AppToolkit.GetDocumentLink(string.Empty))).AsTask();

    private async void OnOpenLoggerClick(object sender, RoutedEventArgs e)
    {
        var folder = await StorageFolder.GetFolderFromPathAsync(Path.Combine(ApplicationData.Current.LocalFolder.Path, "Logger")).AsTask();
        _ = await Launcher.LaunchFolderAsync(folder).AsTask();
    }
}
