// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.Forms;
using RodelAgent.UI.Toolkits;

namespace RodelAgent.UI.ViewModels.Core;

/// <summary>
/// Initializes a new instance of the <see cref="AppViewModel"/> class.
/// </summary>
public sealed partial class AppViewModel : ViewModelBase
{
    [RelayCommand]
    private static void Launch()
    {
        var libPath = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.WorkingDirectory, string.Empty);
        var isLibPathValid = !string.IsNullOrEmpty(libPath) && Directory.Exists(libPath);
        if (!isLibPathValid)
        {
            new StartupWindow().Activate();
        }
        else
        {
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
}
