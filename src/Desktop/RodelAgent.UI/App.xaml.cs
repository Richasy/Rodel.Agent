// Copyright (c) Richasy. All rights reserved.

using Microsoft.UI.Dispatching;
using Microsoft.Windows.AppLifecycle;
using RodelAgent.UI.Models.Constants;
using RodelAgent.UI.Toolkits;
using RodelAgent.UI.ViewModels.Core;
using SimpleTrayIcon;
using System.Runtime.InteropServices;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;

namespace RodelAgent.UI;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
{
    private const string Id = "Richasy.RodelAgent";
    private static TrayMenu _trayMenu;
    private readonly DispatcherQueue _dispatcherQueue;

    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        InitializeComponent();
        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        UnhandledException += OnUnhandledException;
    }

    /// <summary>
    /// 关闭托盘菜单.
    /// </summary>
    public static void CloseTrayMenu()
        => _trayMenu?.Dispose();

    /// <summary>
    /// Invoked when the application is launched.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override async void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        var instance = Microsoft.Windows.AppLifecycle.AppInstance.FindOrRegisterForKey(Id);
        if (instance.IsCurrent)
        {
            instance.Activated += OnInstanceActivated;
            GlobalDependencies.Initialize();

            if (RuntimeInformation.ProcessArchitecture != Architecture.Arm64)
            {
                try
                {
                    var packagePath = Package.Current.InstalledPath;
                    var icon = new System.Drawing.Icon(Path.Combine(packagePath, "Assets", "logo.ico"));
                    _trayMenu = new TrayMenu(icon, ResourceToolkit.GetLocalizedString(StringNames.AppName));
                    var showItem = new TrayMenuItem { Content = ResourceToolkit.GetLocalizedString(StringNames.Show) };
                    var exitItem = new TrayMenuItem { Content = ResourceToolkit.GetLocalizedString(StringNames.Exit) };
                    showItem.Click += OnTrayShowItemClick;
                    exitItem.Click += OnTrayExitItemClick;
                    _trayMenu.Items.Add(showItem);
                    _trayMenu.Items.Add(exitItem);
                    _trayMenu.DoubleClick += OnTrayMenuDoubleClick;
                    GlobalDependencies.Kernel.GetRequiredService<AppViewModel>().IsTraySupport = true;
                }
                catch (Exception)
                {
                    GlobalDependencies.Kernel.GetRequiredService<AppViewModel>().IsTraySupport = false;
                }
            }
        }

        var eventArgs = instance.GetActivatedEventArgs();
        var data = eventArgs.Data is IActivatedEventArgs
            ? eventArgs.Data as IActivatedEventArgs
            : args.UWPLaunchActivatedEventArgs;
        await LaunchWindowAsync(data);
    }

    private static async Task LaunchWindowAsync(IActivatedEventArgs? args = default)
    {
        if (args is IProtocolActivatedEventArgs protocolArgs
            && !string.IsNullOrEmpty(protocolArgs.Uri.Host))
        {
            // TODO: Handle protocol activation.
        }
        else
        {
            var instance = Microsoft.Windows.AppLifecycle.AppInstance.FindOrRegisterForKey(Id);

            // If the current instance is not the previously registered instance
            if (!instance.IsCurrent)
            {
                var activatedArgs = Microsoft.Windows.AppLifecycle.AppInstance.GetCurrent().GetActivatedEventArgs();

                // Redirect to the existing instance
                await instance.RedirectActivationToAsync(activatedArgs);

                // Kill the current instance
                Current.Exit();
                return;
            }

            GlobalDependencies.Kernel.GetRequiredService<AppViewModel>().LaunchCommand.Execute(default);
        }
    }

    private static void ShowWindowsInternal()
    {
        foreach (var wnd in GlobalDependencies.Kernel.GetRequiredService<AppViewModel>().Windows)
        {
            wnd.SetForegroundWindow();
            wnd.Activate();
        }
    }

    private void OnInstanceActivated(object? sender, AppActivationArguments e)
        => _dispatcherQueue.TryEnqueue(() => GlobalDependencies.Kernel.GetRequiredService<AppViewModel>().CheckActivateArgumentsCommand.Execute(default));

    private void OnUnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        GlobalDependencies.Kernel.GetRequiredService<ILogger<App>>().LogError(e.Exception, "Unhandled exception occurred.");
        e.Handled = true;
    }

    private void OnTrayExitItemClick(object? sender, TrayMenuItemClickedEventArgs e)
    {
        this.Get<AppViewModel>().ExitFromTray = true;
        this.Get<AppViewModel>().Windows.Find(p => p is Forms.MainWindow)?.Close();
    }

    private void OnTrayShowItemClick(object? sender, TrayMenuItemClickedEventArgs e)
        => ShowWindowsInternal();

    private void OnTrayMenuDoubleClick(object? sender, EventArgs e)
        => ShowWindowsInternal();
}
