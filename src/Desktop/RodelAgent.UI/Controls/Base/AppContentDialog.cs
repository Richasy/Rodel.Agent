// Copyright (c) Rodel. All rights reserved.

using RodelAgent.UI.Toolkits;
using RodelAgent.UI.ViewModels;

namespace RodelAgent.UI.Controls;

/// <summary>
/// 应用内容对话框.
/// </summary>
public abstract class AppContentDialog : ContentDialog
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppContentDialog"/> class.
    /// </summary>
    public AppContentDialog()
    {
        Opened += OnOpened;
        Closing += OnClosing;
        AppToolkit.ResetControlTheme(this);
        XamlRoot = GlobalDependencies.ServiceProvider.GetRequiredService<AppViewModel>().ActivatedWindow.Content.XamlRoot;
    }

    private void OnClosing(ContentDialog sender, ContentDialogClosingEventArgs args)
        => GlobalDependencies.ServiceProvider.GetRequiredService<AppViewModel>().CurrentDialog = null;

    private void OnOpened(ContentDialog sender, ContentDialogOpenedEventArgs args)
        => GlobalDependencies.ServiceProvider.GetRequiredService<AppViewModel>().CurrentDialog = this;
}
