// Copyright (c) Richasy. All rights reserved.

using Richasy.AgentKernel;
using RodelAgent.Context;
using RodelAgent.Interfaces;
using RodelAgent.UI.Models.Constants;
using RodelAudio.Models.Client;
using RodelChat.Models.Client;
using RodelTranslate.Models.Client;
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

    /// <inheritdoc/>
    public string GetWorkingDirectory()
        => Toolkits.SettingsToolkit.ReadLocalSetting(SettingNames.WorkingDirectory, string.Empty);

    /// <inheritdoc/>
    public void SetWorkingDirectory(string workingDirectory)
    {
        Toolkits.SettingsToolkit.WriteLocalSetting(SettingNames.WorkingDirectory, workingDirectory);
        _dbService.SetWorkingDirectory(workingDirectory);
    }

    public Task AddOrUpdateAudioSessionAsync(AudioSession session, byte[]? audioData) => throw new NotImplementedException();
    public Task AddOrUpdateChatAgentAsync(ChatSessionPreset agent) => throw new NotImplementedException();
    public Task AddOrUpdateChatGroupPresetAsync(ChatGroupPreset preset) => throw new NotImplementedException();
    public Task AddOrUpdateChatGroupSessionAsync(ChatGroup session) => throw new NotImplementedException();
    public Task AddOrUpdateChatSessionAsync(ChatSession session) => throw new NotImplementedException();
    public Task AddOrUpdateChatSessionPresetAsync(ChatSessionPreset preset) => throw new NotImplementedException();
    
    public Task AddOrUpdateTranslateSessionAsync(TranslateSession session) => throw new NotImplementedException();
    
    public Task<List<AudioSession>?> GetAudioSessionsAsync() => throw new NotImplementedException();
    public Task<List<ChatSessionPreset>> GetChatAgentsAsync() => throw new NotImplementedException();
    
    public Task<ChatGroupPreset> GetChatGroupPresetByIdAsync(string presetId) => throw new NotImplementedException();
    public Task<List<ChatGroupPreset>> GetChatGroupPresetsAsync() => throw new NotImplementedException();
    public Task<List<ChatGroup>?> GetChatGroupSessionsAsync(string presetId) => throw new NotImplementedException();
    public Task<ChatSessionPreset> GetChatSessionPresetByIdAsync(string presetId) => throw new NotImplementedException();
    public Task<List<ChatSessionPreset>> GetChatSessionPresetsAsync() => throw new NotImplementedException();
    public Task<List<ChatSession>?> GetChatSessionsAsync(ChatProviderType type) => throw new NotImplementedException();
    public Task<List<ChatSession>?> GetChatSessionsAsync(string presetId) => throw new NotImplementedException();
    
    
    
    public Task<List<TranslateSession>?> GetTranslateSessionsAsync(TranslateProviderType type) => throw new NotImplementedException();
    public Task RemoveAudioSessionAsync(string sessionId) => throw new NotImplementedException();
    public Task RemoveChatAgentAsync(string agentId) => throw new NotImplementedException();
    public Task RemoveChatGroupPresetAsync(string presetId) => throw new NotImplementedException();
    public Task RemoveChatGroupSessionAsync(string sessionId) => throw new NotImplementedException();
    public Task RemoveChatSessionAsync(string sessionId) => throw new NotImplementedException();
    public Task RemoveChatSessionPresetAsync(string presetId) => throw new NotImplementedException();
    
    public Task RemoveTranslateSessionAsync(string sessionId) => throw new NotImplementedException();
    public Task<string> RetrieveAzureSpeechVoicesAsync() => throw new NotImplementedException();
    public Task SaveAzureSpeechVoicesAsync(string json) => throw new NotImplementedException();
}
