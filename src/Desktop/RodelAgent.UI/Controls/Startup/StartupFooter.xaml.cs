// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.Toolkits;

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

#pragma warning disable VSTHRD100 // Avoid async void methods
    private async void OnDocumentButtonClick(object sender, RoutedEventArgs e)
#pragma warning restore VSTHRD100 // Avoid async void methods
        => await Windows.System.Launcher.LaunchUriAsync(new Uri(AppToolkit.GetDocumentLink(string.Empty))).AsTask();
}
