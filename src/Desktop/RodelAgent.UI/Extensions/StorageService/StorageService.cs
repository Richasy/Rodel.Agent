// Copyright (c) Richasy. All rights reserved.

using RodelAgent.Context;
using RodelAgent.Interfaces;
using RodelAgent.UI.Models.Constants;
using RodelChat.Models.Client;
using Windows.ApplicationModel;

namespace RodelAgent.UI.Extensions;

internal sealed partial class StorageService : IStorageService
{
    private readonly DbService _dbService;

    /// <summary>
    /// Initializes a new instance of the <see cref="StorageService"/> class.
    /// </summary>
    public StorageService(DbService dbService)
    {
        _dbService = dbService;
        _dbService.SetWorkingDirectory(GetWorkingDirectory());
        _dbService.SetPackageDirectory(Package.Current.InstalledPath);
    }

    public Task AddOrUpdateChatAgentAsync(ChatSessionPreset agent) => throw new NotImplementedException();
    public Task AddOrUpdateChatGroupPresetAsync(ChatGroupPreset preset) => throw new NotImplementedException();
    public Task AddOrUpdateChatSessionPresetAsync(ChatSessionPreset preset) => throw new NotImplementedException();
    public Task<List<ChatSessionPreset>> GetChatAgentsAsync() => throw new NotImplementedException();
    
    public Task<ChatGroupPreset> GetChatGroupPresetByIdAsync(string presetId) => throw new NotImplementedException();
    public Task<List<ChatGroupPreset>> GetChatGroupPresetsAsync() => throw new NotImplementedException();
    public Task<ChatSessionPreset> GetChatSessionPresetByIdAsync(string presetId) => throw new NotImplementedException();
    public Task<List<ChatSessionPreset>> GetChatSessionPresetsAsync() => throw new NotImplementedException();

    /// <inheritdoc/>
    public string GetWorkingDirectory()
        => Toolkits.SettingsToolkit.ReadLocalSetting(SettingNames.WorkingDirectory, string.Empty);
    public Task RemoveChatAgentAsync(string agentId) => throw new NotImplementedException();
    public Task RemoveChatGroupPresetAsync(string presetId) => throw new NotImplementedException();
    public Task RemoveChatSessionPresetAsync(string presetId) => throw new NotImplementedException();

    /// <inheritdoc/>
    public void SetWorkingDirectory(string workingDirectory)
    {
        Toolkits.SettingsToolkit.WriteLocalSetting(SettingNames.WorkingDirectory, workingDirectory);
        _dbService.SetWorkingDirectory(workingDirectory);
    }
}
