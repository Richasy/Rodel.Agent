﻿// Copyright (c) Rodel. All rights reserved.

namespace RodelChat.UI.Forms;

/// <summary>
/// Window base class.
/// </summary>
public class WindowBase : WindowEx
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WindowBase"/> class.
    /// </summary>
    public WindowBase()
    {
        AppWindow.TitleBar.ExtendsContentIntoTitleBar = true;
        AppWindow.TitleBar.ButtonBackgroundColor = Colors.Transparent;
        AppWindow.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
        AppWindow.TitleBar.IconShowOptions = Microsoft.UI.Windowing.IconShowOptions.HideIconAndSystemMenu;
        SystemBackdrop = new Microsoft.UI.Xaml.Media.MicaBackdrop();
        Title = ResourceToolkit.GetLocalizedString(StringNames.AppName);
        this.SetIcon("Assets/logo.ico");

        Activated += OnActivated;
    }

    private void OnActivated(object sender, WindowActivatedEventArgs args)
    {
        if (args.WindowActivationState != WindowActivationState.Deactivated)
        {
            AppViewModel.Instance.ActivatedWindow = this;
        }
    }
}
