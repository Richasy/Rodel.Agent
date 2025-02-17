// Copyright (c) Richasy. All rights reserved.

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
        XamlRoot = this.Get<AppViewModel>().ActivatedWindow.Content.XamlRoot;
    }

    private void OnClosing(ContentDialog sender, ContentDialogClosingEventArgs args)
        => this.Get<AppViewModel>().CurrentDialog = null;

    private void OnOpened(ContentDialog sender, ContentDialogOpenedEventArgs args)
        => this.Get<AppViewModel>().CurrentDialog = this;
}
