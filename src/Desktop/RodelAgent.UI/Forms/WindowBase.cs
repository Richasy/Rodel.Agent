// Copyright (c) Rodel. All rights reserved.

using RodelAgent.UI.Models.Constants;
using RodelAgent.UI.Toolkits;
using RodelAgent.UI.ViewModels;

namespace RodelAgent.UI.Forms;

/// <summary>
/// Window base class.
/// </summary>
public abstract class WindowBase : WindowEx
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
        Closed += OnClosed;
        CoreViewModel.DisplayWindows.Add(this);
    }

    /// <summary>
    /// 服务提供程序.
    /// </summary>
    public static IServiceProvider ServiceProvider => App.ServiceProvider;

    /// <summary>
    /// 核心视图模型.
    /// </summary>
    public static AppViewModel CoreViewModel => ServiceProvider.GetRequiredService<AppViewModel>();

    private void OnActivated(object sender, WindowActivatedEventArgs args)
    {
        if (args.WindowActivationState != WindowActivationState.Deactivated)
        {
            CoreViewModel.ActivatedWindow = this;
        }
    }

    private void OnClosed(object sender, WindowEventArgs args)
        => CoreViewModel.DisplayWindows.Remove(this);
}
