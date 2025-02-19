// Copyright (c) Richasy. All rights reserved.

using Richasy.WinUIKernel.AI.ViewModels;
using Richasy.WinUIKernel.Share.Toolkits;
using RodelAgent.UI.Models.Constants;
using RodelAgent.UI.Toolkits;
using Windows.Storage;

namespace RodelAgent.UI.ViewModels.View;

/// <summary>
/// 设置页面视图模型.
/// </summary>
public sealed partial class SettingsPageViewModel : AISettingsViewModelBase
{
    [RelayCommand]
    private void Initialize()
    {
        if (_isInitialized)
        {
            return;
        }

        AppTheme = SettingsToolkit.ReadLocalSetting(SettingNames.AppTheme, ElementTheme.Default);
        WorkingDirectory = SettingsToolkit.ReadLocalSetting(SettingNames.WorkingDirectory, string.Empty);
        CheckTheme();

        var copyrightTemplate = ResourceToolkit.GetLocalizedString(StringNames.Copyright);
        Copyright = string.Format(copyrightTemplate, 2025);
        PackageVersion = this.Get<IAppToolkit>().GetPackageVersion();

        HideWhenWindowClosing = SettingsToolkit.ReadLocalSetting(SettingNames.HideWhenCloseWindow, false);

        InitializeLinks();
        InitializeLibraries();

        _isInitialized = true;
    }

    [RelayCommand]
    private async Task OpenWorkingDirectoryAsync()
    {
        if (string.IsNullOrEmpty(WorkingDirectory) || !Directory.Exists(WorkingDirectory))
        {
            return;
        }

        var folder = await StorageFolder.GetFolderFromPathAsync(WorkingDirectory);
        await Windows.System.Launcher.LaunchFolderAsync(folder);
    }

    [RelayCommand]
    private void CloseLibrary()
    {
        SettingsToolkit.WriteLocalSetting(SettingNames.WorkingDirectory, string.Empty);
        this.Get<Core.AppViewModel>().RestartCommand.Execute(default);
    }

    private void CheckTheme()
    {
        AppThemeText = AppTheme switch
        {
            ElementTheme.Light => ResourceToolkit.GetLocalizedString(StringNames.LightTheme),
            ElementTheme.Dark => ResourceToolkit.GetLocalizedString(StringNames.DarkTheme),
            _ => ResourceToolkit.GetLocalizedString(StringNames.SystemDefault),
        };
    }

    partial void OnAppThemeChanged(ElementTheme value)
    {
        SettingsToolkit.WriteLocalSetting(SettingNames.AppTheme, value);
        CheckTheme();
        this.Get<Core.AppViewModel>().ChangeThemeCommand.Execute(value);
    }

    partial void OnHideWhenWindowClosingChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.HideWhenCloseWindow, value);
}
