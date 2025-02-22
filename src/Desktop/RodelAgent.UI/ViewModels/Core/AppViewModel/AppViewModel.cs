// Copyright (c) Richasy. All rights reserved.

using Richasy.WinUIKernel.Share.Toolkits;
using RodelAgent.UI.Forms;
using RodelAgent.UI.Models.Constants;
using RodelAgent.UI.Toolkits;
using Windows.System;

namespace RodelAgent.UI.ViewModels.Core;

/// <summary>
/// Initializes a new instance of the <see cref="AppViewModel"/> class.
/// </summary>
public sealed partial class AppViewModel : ViewModelBase
{
    [RelayCommand]
    private static async Task LaunchAsync()
    {
        var libPath = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.WorkingDirectory, string.Empty);
        var isLibPathValid = !string.IsNullOrEmpty(libPath) && Directory.Exists(libPath);
        if (!isLibPathValid)
        {
            new StartupWindow().Activate();
        }
        else
        {
            try
            {
                await MigrationToolkit.TryMigrateAsync();
            }
            catch (Exception)
            {
                new StartupWindow().Activate();
                return;
            }

            new MainWindow().Activate();
        }
    }

    /// <summary>
    /// 显示提示.
    /// </summary>
    [RelayCommand]
    private async Task ShowTipAsync((string, InfoType) data)
    {
        if (ActivatedWindow is ITipWindow tipWindow)
        {
            await tipWindow.ShowTipAsync(data.Item1, data.Item2);
        }
        else
        {
            var firstWindow = Windows.OfType<ITipWindow>().FirstOrDefault();
            if (firstWindow is not null)
            {
                await firstWindow.ShowTipAsync(data.Item1, data.Item2);
            }
        }
    }

    [RelayCommand]
    private static void Restart()
    {
        Microsoft.Windows.AppLifecycle.AppInstance.GetCurrent().UnregisterKey();
        Microsoft.Windows.AppLifecycle.AppInstance.Restart(default);
    }

    [RelayCommand]
    private async Task CheckActivateArgumentsAsync()
    {
        // TODO: Check if the app is activated with arguments.
        _ = this;
        await Task.CompletedTask;
    }

    [RelayCommand]
    private void ChangeTheme(ElementTheme theme)
    {
        foreach (var window in Windows)
        {
            (window.Content as FrameworkElement)!.RequestedTheme = theme;
        }
    }

    [RelayCommand]
    private void CheckUpdate()
    {
        var localVersion = SettingsToolkit.ReadLocalSetting(SettingNames.AppVersion, string.Empty);
        var currentVersion = this.Get<IAppToolkit>().GetPackageVersion();
        if (localVersion != currentVersion)
        {
            SettingsToolkit.WriteLocalSetting(SettingNames.AppVersion, currentVersion);
            IsUpdateShown = true;
        }
    }

    [RelayCommand]
    private void HideUpdate()
        => IsUpdateShown = false;

    [RelayCommand]
    private async Task ShowUpdateAsync()
    {
        var packVersion = this.Get<IAppToolkit>().GetPackageVersion();
        var url = $"https://github.com/Richasy/Rodel.Agent/releases/tag/v{packVersion}";
        await Launcher.LaunchUriAsync(new Uri(url));
        HideUpdate();
    }

    [RelayCommand]
    private void HideAllWindows()
    {
        foreach (var wnd in Windows)
        {
            wnd.Hide();
        }
    }

    [RelayCommand]
    private void TryReloadChatServices()
    {
        if (IsClosing)
        {
            return;
        }

        RequestReloadChatServices?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private void TryReloadDrawServices()
    {
        if (IsClosing)
        {
            return;
        }

        RequestReloadDrawServices?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private void TryReloadAudioServices()
    {
        if (IsClosing)
        {
            return;
        }

        RequestReloadAudioServices?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private void TryReloadTranslateServices()
    {
        if (IsClosing)
        {
            return;
        }

        RequestReloadTranslateServices?.Invoke(this, EventArgs.Empty);
    }
}
