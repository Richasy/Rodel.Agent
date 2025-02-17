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
    [ObservableProperty]
    public partial string Version { get; set; }

    [ObservableProperty]
    public partial bool IsLoading { get; set; }

    [ObservableProperty]
    public partial string ErrorMessage { get; set; }

    [RelayCommand]
    private void Initialize()
    {
        var libPath = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.WorkingDirectory, string.Empty);
        if (!string.IsNullOrEmpty(libPath))
        {
            ErrorMessage = ResourceToolkit.GetLocalizedString(StringNames.InvalidDirectoryPath);
            SettingsToolkit.DeleteLocalSetting(Models.Constants.SettingNames.WorkingDirectory);
        }

        Version = this.Get<IAppToolkit>().GetPackageVersion();
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

            SettingsToolkit.WriteLocalSetting(Models.Constants.SettingNames.WorkingDirectory, folder.Path);
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

            SettingsToolkit.WriteLocalSetting(Models.Constants.SettingNames.WorkingDirectory, folder.Path);
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
