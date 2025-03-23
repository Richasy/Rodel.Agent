// Copyright (c) Richasy. All rights reserved.

using RodelAgent.Context;
using RodelAgent.Interfaces;
using RodelAgent.Models.Feature;
using RodelAgent.UI.Models.Constants;
using Windows.ApplicationModel;

namespace RodelAgent.UI.Extensions;

internal sealed partial class StorageService : IStorageService
{
    private readonly DbService _dbService;
    private List<ChatAgent> _chatAgents;
    private List<ChatGroup> _chatGroups;

    /// <summary>
    /// Initializes a new instance of the <see cref="StorageService"/> class.
    /// </summary>
    public StorageService(DbService dbService)
    {
        _dbService = dbService;
        _dbService.SetWorkingDirectory(GetWorkingDirectory());
        _dbService.SetPackageDirectory(Package.Current.InstalledPath);
    }

    /// <inheritdoc/>
    public string GetWorkingDirectory()
        => Toolkits.SettingsToolkit.ReadLocalSetting(SettingNames.WorkingDirectory, string.Empty);

    /// <inheritdoc/>
    public void SetWorkingDirectory(string workingDirectory)
    {
        Toolkits.SettingsToolkit.WriteLocalSetting(SettingNames.WorkingDirectory, workingDirectory);
        _dbService.SetWorkingDirectory(workingDirectory);
    }
}
