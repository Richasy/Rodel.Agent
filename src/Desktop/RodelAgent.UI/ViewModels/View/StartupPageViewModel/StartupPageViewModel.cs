// Copyright (c) Richasy. All rights reserved.

using Richasy.WinUIKernel.Share.Toolkits;
using RodelAgent.UI.Models.Constants;
using RodelAgent.UI.Toolkits;
using RodelAgent.UI.ViewModels.Core;
using Windows.Storage;

namespace RodelAgent.UI.ViewModels.View;

/// <summary>
/// Startup page view model.
/// </summary>
public sealed partial class StartupPageViewModel(ILogger<StartupPageViewModel> logger) : ViewModelBase
{
    [RelayCommand]
    private void Initialize()
    {
        Version = this.Get<IAppToolkit>().GetPackageVersion();
        if (SettingsToolkit.ReadLocalSetting(SettingNames.MigrationFailed, false))
        {
            ErrorMessage = ResourceToolkit.GetLocalizedString(StringNames.MigrationFailed);
        }
        else
        {
            var libPath = SettingsToolkit.ReadLocalSetting(SettingNames.WorkingDirectory, string.Empty);
            if (!string.IsNullOrEmpty(libPath))
            {
                ErrorMessage = ResourceToolkit.GetLocalizedString(StringNames.InvalidDirectoryPath);
                SettingsToolkit.DeleteLocalSetting(SettingNames.WorkingDirectory);
            }
        }
    }

    [RelayCommand]
    private async Task OpenWorkingDirectoryAsync()
    {
        ErrorMessage = string.Empty;
        IsLoading = true;
        try
        {
            var folderObj = await this.Get<IFileToolkit>().PickFolderAsync(this.Get<AppViewModel>().ActivatedWindow);
            if (folderObj is not StorageFolder folder)
            {
                return;
            }

            if (!Directory.Exists(folder.Path))
            {
                ErrorMessage = ResourceToolkit.GetLocalizedString(StringNames.OpenWorkingDirectoryFailed);
                return;
            }

            SettingsToolkit.WriteLocalSetting(SettingNames.WorkingDirectory, folder.Path);

            IsMigrating = true;
            await MigrationToolkit.TryMigrateAsync();
            IsMigrating = false;

            this.Get<AppViewModel>().RestartCommand.Execute(default);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to open library.");
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task CreateWorkingDirectoryAsync()
    {
        ErrorMessage = string.Empty;
        IsLoading = true;
        try
        {
            var folderObj = await this.Get<IFileToolkit>().PickFolderAsync(this.Get<AppViewModel>().ActivatedWindow);
            if (folderObj is not StorageFolder folder)
            {
                return;
            }

            var items = await folder.GetItemsAsync().AsTask();
            if (items.Count > 0)
            {
                ErrorMessage = ResourceToolkit.GetLocalizedString(StringNames.WorkingDirectoryShouldBeEmpty);
                return;
            }

            SettingsToolkit.WriteLocalSetting(SettingNames.WorkingDirectory, folder.Path);
            this.Get<AppViewModel>().RestartCommand.Execute(default);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create library.");
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }
}
