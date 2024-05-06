// Copyright (c) Rodel. All rights reserved.

using Microsoft.UI.Dispatching;
using Microsoft.Windows.AppLifecycle;
using NLog;
using Windows.ApplicationModel.Activation;
using Windows.Storage;

namespace RodelChat.UI;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// 应用标识符.
    /// </summary>
    public const string Id = "084C510A-A128-4709-9FFE-81F6A1B3F58G";
    private DispatcherQueue _dispatcherQueue;
    private WindowBase _window;

    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        InitializeComponent();
        var mainAppInstance = AppInstance.FindOrRegisterForKey(Id);
        mainAppInstance.Activated += OnAppInstanceActivated;
        UnhandledException += OnUnhandledException;
    }

    /// <summary>
    /// Invoked when the application is launched.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override async void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        var instance = AppInstance.FindOrRegisterForKey(Id);
        if (instance.IsCurrent)
        {
            _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
            var rootFolder = ApplicationData.Current.LocalFolder;
            var fullPath = $"{rootFolder.Path}\\Logger\\";
            GlobalDiagnosticsContext.Set("LogPath", fullPath);
        }

        var eventArgs = instance.GetActivatedEventArgs();
        var data = eventArgs.Data is IActivatedEventArgs
            ? eventArgs.Data as IActivatedEventArgs
            : args.UWPLaunchActivatedEventArgs;
        await LaunchWindowAsync(data);
    }

    /// <summary>
    /// Try activating the window and bringing it to the foreground.
    /// </summary>
    private void ActivateWindow(AppActivationArguments arguments = default)
    {
        _ = _dispatcherQueue.TryEnqueue(async () =>
        {
            var needWelcome = !SettingsToolkit.ReadLocalSetting(SettingNames.SkipWelcome, false);
            if (needWelcome)
            {
                return;
            }

            if (_window == null)
            {
                await LaunchWindowAsync();
            }
            else if (_window.Visible && arguments?.Data == null)
            {
                _ = _window.Hide();
            }
            else
            {
                _window.Activate();
                _ = _window.SetForegroundWindow();
            }

            try
            {
                if (arguments?.Data is IActivatedEventArgs args)
                {
                    ((MainWindow)_window).ActivateArguments(args);
                }
            }
            catch (Exception)
            {
            }
        });
    }

    private async Task LaunchWindowAsync(IActivatedEventArgs args = default)
    {
        if (args is IProtocolActivatedEventArgs protocolArgs
            && !string.IsNullOrEmpty(protocolArgs.Uri.Host))
        {
            // TODO: Handle protocol activation
        }
        else
        {
            var instance = AppInstance.FindOrRegisterForKey(Id);

            // If the current instance is not the previously registered instance
            if (!instance.IsCurrent)
            {
                var activatedArgs = AppInstance.GetCurrent().GetActivatedEventArgs();

                // Redirect to the existing instance
                await instance.RedirectActivationToAsync(activatedArgs);

                // Kill the current instance
                Current.Exit();
                return;
            }

            var needWelcome = !SettingsToolkit.ReadLocalSetting(SettingNames.SkipWelcome, false);

            if (needWelcome)
            {
                var window = new WelcomeWindow();
                window.Activate();
            }
            else
            {
                _window = new MainWindow(args);
                _window.Activate();
            }
        }
    }

    private void OnUnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        var logger = LogManager.GetCurrentClassLogger();
        logger.Error(e.Exception, "An exception occurred while the application was running");
        e.Handled = true;
    }

    private void OnAppInstanceActivated(object sender, AppActivationArguments e)
        => ActivateWindow(e);
}
