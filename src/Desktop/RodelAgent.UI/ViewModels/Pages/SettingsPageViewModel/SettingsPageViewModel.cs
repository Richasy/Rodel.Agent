// Copyright (c) Rodel. All rights reserved.

using Microsoft.Windows.AppLifecycle;
using RodelAgent.Interfaces;
using RodelAgent.UI.Models.Constants;
using RodelAgent.UI.Toolkits;
using Windows.Storage;
using Windows.System;

namespace RodelAgent.UI.ViewModels.Pages;

/// <summary>
/// 设置页面视图模型.
/// </summary>
public sealed partial class SettingsPageViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsPageViewModel"/> class.
    /// </summary>
    public SettingsPageViewModel(
        IStorageService storageService,
        ILogger<SettingsPageViewModel> logger)
    {
        _storageService = storageService;
        _logger = logger;
    }

    [RelayCommand]
    private void InitializeGenericSettings()
    {
        AppTheme = SettingsToolkit.ReadLocalSetting(SettingNames.AppTheme, ElementTheme.Default);
        UseStreamOutput = SettingsToolkit.ReadLocalSetting(SettingNames.UseStreamOutput, true);
        AppVersion = AppToolkit.GetPackageVersion();
        var copyrightTemplate = ResourceToolkit.GetLocalizedString(StringNames.CopyrightTemplate);
        Copyright = string.Format(copyrightTemplate, BuildYear);

        WorkingDirectory = SettingsToolkit.ReadLocalSetting(SettingNames.WorkingDirectory, string.Empty);

        ShouldRecordTranslate = SettingsToolkit.ReadLocalSetting(SettingNames.ShouldRecordTranslate, false);
    }

    [RelayCommand]
    private Task InitializeOnlineChatServicesAsync()
    {
        _shouldSaveChatServices = true;
        return PageViewModelShare.InitializeOnlineChatServicesAsync(OnlineChatServices, _storageService);
    }

    [RelayCommand]
    private Task InitializeOnlineTranslateServicesAsync()
    {
        _shouldSaveTranslateServices = true;
        return PageViewModelShare.InitializeOnlineTranslateServicesAsync(OnlineTranslateServices, _storageService);
    }

    [RelayCommand]
    private Task InitializeOnlineDrawServicesAsync()
    {
        _shouldSaveDrawServices = true;
        return PageViewModelShare.InitializeOnlineDrawServicesAsync(OnlineDrawServices, _storageService);
    }

    [RelayCommand]
    private Task InitializeOnlineAudioServicesAsync()
    {
        _shouldSaveAudioServices = true;
        return PageViewModelShare.InitializeOnlineAudioServicesAsync(OnlineAudioServices, _storageService);
    }

    [RelayCommand]
    private async Task SaveOnlineChatServicesAsync()
    {
        if (!_shouldSaveChatServices)
        {
            return;
        }

        _shouldSaveChatServices = false;
        await PageViewModelShare.SaveOnlineChatServicesAsync(OnlineChatServices, _storageService);
        GlobalDependencies.ServiceProvider.GetRequiredService<ChatServicePageViewModel>()
            .ResetAvailableChatServicesCommand.Execute(default);
    }

    [RelayCommand]
    private async Task SaveOnlineTranslateServicesAsync()
    {
        if (!_shouldSaveTranslateServices)
        {
            return;
        }

        _shouldSaveTranslateServices = false;
        await PageViewModelShare.SaveOnlineTranslateServicesAsync(OnlineTranslateServices, _storageService);
        GlobalDependencies.ServiceProvider.GetRequiredService<TranslateServicePageViewModel>()
           .ResetAvailableTranslateServicesCommand.Execute(default);
    }

    [RelayCommand]
    private async Task SaveOnlineDrawServicesAsync()
    {
        if (!_shouldSaveDrawServices)
        {
            return;
        }

        _shouldSaveDrawServices = false;
        await PageViewModelShare.SaveOnlineDrawServicesAsync(OnlineDrawServices, _storageService);
        GlobalDependencies.ServiceProvider.GetRequiredService<DrawServicePageViewModel>()
           .ResetAvailableDrawServicesCommand.Execute(default);
    }

    [RelayCommand]
    private async Task SaveOnlineAudioServicesAsync()
    {
        if (!_shouldSaveAudioServices)
        {
            return;
        }

        _shouldSaveAudioServices = false;
        await PageViewModelShare.SaveOnlineAudioServicesAsync(OnlineAudioServices, _storageService);
        GlobalDependencies.ServiceProvider.GetRequiredService<AudioServicePageViewModel>()
           .ResetAvailableAudioServicesCommand.Execute(default);
    }

    [RelayCommand]
    private async Task OpenDirectoryAsync()
    {
        var folder = await StorageFolder.GetFolderFromPathAsync(WorkingDirectory);
        await Launcher.LaunchFolderAsync(folder);
    }

    [RelayCommand]
    private void CloseDirectory()
    {
        WorkingDirectory = string.Empty;
        SettingsToolkit.DeleteLocalSetting(SettingNames.WorkingDirectory);
        SettingsToolkit.DeleteLocalSetting(SettingNames.ShouldSkipStartup);

        AppInstance.Restart(string.Empty);
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
    }

    partial void OnUseStreamOutputChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.UseStreamOutput, value);

    partial void OnShouldRecordTranslateChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.ShouldRecordTranslate, value);
}
