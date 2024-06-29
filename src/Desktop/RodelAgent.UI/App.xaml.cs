// Copyright (c) Rodel. All rights reserved.

using H.NotifyIcon;
using Microsoft.UI.Dispatching;
using Microsoft.Windows.AppLifecycle;
using NLog;
using RodelAgent.UI.Controls;
using RodelAgent.UI.Forms;
using RodelAgent.UI.Models.Constants;
using RodelAgent.UI.Toolkits;
using Windows.ApplicationModel.Activation;
using Windows.Globalization;
using Windows.Storage;

namespace RodelAgent.UI;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
{
    private const string Id = "Richasy.RodelAgent";
    private DispatcherQueue _dispatcherQueue;
    private WindowBase _window;

    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        InitializeComponent();
        RodelAgent.Context.MigrationUtils.SetRootPath(Windows.ApplicationModel.Package.Current.InstalledPath);
        var language = SettingsToolkit.ReadLocalSetting(SettingNames.AppLanguage, "default");
        ApplicationLanguages.PrimaryLanguageOverride = language != "default"
            ? language
            : string.Empty;

        FluentIcons.WinUI.Extensions.UseSegoeMetrics(this);
        var mainAppInstance = AppInstance.FindOrRegisterForKey(Id);
        mainAppInstance.Activated += OnAppInstanceActivated;
        UnhandledException += OnUnhandledException;
    }

    /// <summary>
    /// Gets the service provider instance.
    /// </summary>
    internal static IServiceProvider ServiceProvider => GlobalDependencies.ServiceProvider;

    private TaskbarIcon TrayIcon { get; set; }

    private bool HandleCloseEvents { get; set; }

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
            var fullPath = $"{rootFolder.Path}\\Logger";
            NLog.GlobalDiagnosticsContext.Set("LogPath", fullPath);

            GlobalDependencies.Initialize();
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
            if (_window == null)
            {
                await LaunchWindowAsync();
            }
            else if (_window.Visible && HandleCloseEvents && arguments?.Data == null)
            {
                _ = _window.Hide();
            }
            else
            {
                _window.Activate();
                _ = _window.SetForegroundWindow();
            }
        });
    }

    private void InitializeTrayIcon()
    {
        if (TrayIcon != null)
        {
            return;
        }

        var showHideWindowCommand = (XamlUICommand)Resources["ShowHideWindowCommand"];
        showHideWindowCommand.ExecuteRequested += OnShowHideWindowCommandExecuteRequested;

        var exitApplicationCommand = (XamlUICommand)Resources["QuitCommand"];
        exitApplicationCommand.ExecuteRequested += OnQuitCommandExecuteRequested;

        try
        {
            TrayIcon = (TaskbarIcon)Resources["TrayIcon"];
            TrayIcon.ForceCreate();
        }
        catch (Exception)
        {
            var logger = LogManager.GetCurrentClassLogger();
            logger.Error("Failed to initialize tray icon");
        }
    }

    private async Task LaunchWindowAsync(IActivatedEventArgs args = default)
    {
        if (args is IProtocolActivatedEventArgs protocolArgs
            && !string.IsNullOrEmpty(protocolArgs.Uri.Host))
        {
            // 处理协议启动.
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

            var shouldSkipStartup = SettingsToolkit.ReadLocalSetting(SettingNames.ShouldSkipStartup, false);
            if (!shouldSkipStartup)
            {
                var window = new StartupWindow();
                window.Activate();
            }
            else
            {
                _window = new MainWindow();
                _window.Closed += OnMainWindowClosedAsync;

                HandleCloseEvents = SettingsToolkit.ReadLocalSetting(SettingNames.HideWhenCloseWindow, true);
                if (HandleCloseEvents)
                {
                    InitializeTrayIcon();
                }

                _window.Activate();
            }
        }
    }

    private async void OnMainWindowClosedAsync(object sender, WindowEventArgs args)
    {
        HandleCloseEvents = SettingsToolkit.ReadLocalSetting(SettingNames.HideWhenCloseWindow, true);
        if (HandleCloseEvents)
        {
            args.Handled = true;

            var shouldAsk = SettingsToolkit.ReadLocalSetting(SettingNames.ShouldAskBeforeWindowClosed, true);
            if (shouldAsk)
            {
                _window.Activate();
                var dialog = new CloseWindowTipDialog
                {
                    XamlRoot = _window.Content.XamlRoot,
                };
                var result = await dialog.ShowAsync();
                if (result == ContentDialogResult.None)
                {
                    return;
                }

                var shouldHide = result == ContentDialogResult.Secondary;
                if (dialog.IsNeverAskChecked)
                {
                    SettingsToolkit.WriteLocalSetting(SettingNames.ShouldAskBeforeWindowClosed, false);
                    SettingsToolkit.WriteLocalSetting(SettingNames.HideWhenCloseWindow, shouldHide);
                }

                if (!shouldHide)
                {
                    ExitApp();
                    return;
                }
            }

            InitializeTrayIcon();
            _ = _window.Hide();
        }
    }

    private void ExitApp()
    {
        HandleCloseEvents = false;
        TrayIcon?.Dispose();
        _window?.Close();
        Environment.Exit(0);
    }

    private void OnUnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        var logger = ServiceProvider.GetRequiredService<ILogger<App>>();
        logger.LogError(e.Exception, "An exception occurred while the application was running");
        e.Handled = true;
    }

    private void OnQuitCommandExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        => ExitApp();

    private void OnShowHideWindowCommandExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        => ActivateWindow();

    private void OnAppInstanceActivated(object sender, AppActivationArguments e)
        => ActivateWindow(e);
}
