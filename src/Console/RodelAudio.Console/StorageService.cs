// Copyright (c) Rodel. All rights reserved.

using RodelAgent.Interfaces;
using RodelAudio.Models.Client;
using RodelAudio.Models.Constants;
using RodelChat.Models.Client;
using RodelDraw.Models.Client;
using RodelTranslate.Models.Client;

namespace RodelAudio.Console;

/// <summary>
/// 存储服务.
/// </summary>
public sealed class StorageService : IStorageService
{
    private const string AzureSpeechVoiceFileName = "AzureSpeechVoices.json";

    /// <inheritdoc/>
    public Task AddOrUpdateAudioSessionAsync(AudioSession session, byte[]? audioData) => throw new NotImplementedException();

    /// <inheritdoc/>
    public Task AddOrUpdateChatAgentAsync(ChatSessionPreset agent) => throw new NotImplementedException();

    /// <inheritdoc/>
    public Task AddOrUpdateChatSessionAsync(ChatSession session) => throw new NotImplementedException();

    /// <inheritdoc/>
    public Task AddOrUpdateChatSessionPresetAsync(ChatSessionPreset preset) => throw new NotImplementedException();

    /// <inheritdoc/>
    public Task AddOrUpdateDrawSessionAsync(DrawSession session, byte[]? imageData) => throw new NotImplementedException();

    /// <inheritdoc/>
    public Task AddOrUpdateTranslateSessionAsync(TranslateSession session) => throw new NotImplementedException();

    /// <inheritdoc/>
    public Task<T> GetAudioConfigAsync<T>(ProviderType type)
        where T : class => throw new NotImplementedException();

    /// <inheritdoc/>
    public Task<List<AudioSession>?> GetAudioSessionsAsync() => throw new NotImplementedException();

    /// <inheritdoc/>
    public Task<List<ChatSessionPreset>> GetChatAgentsAsync() => throw new NotImplementedException();

    /// <inheritdoc/>
    public Task<T> GetChatConfigAsync<T>(RodelChat.Models.Constants.ProviderType type)
        where T : class => throw new NotImplementedException();

    /// <inheritdoc/>
    public Task<ChatSessionPreset> GetChatSessionPresetByIdAsync(string presetId) => throw new NotImplementedException();

    /// <inheritdoc/>
    public Task<List<ChatSessionPreset>> GetChatSessionPresetsAsync() => throw new NotImplementedException();

    /// <inheritdoc/>
    public Task<List<ChatSession>?> GetChatSessionsAsync(RodelChat.Models.Constants.ProviderType type) => throw new NotImplementedException();

    /// <inheritdoc/>
    public Task<List<ChatSession>?> GetChatSessionsAsync(string presetId) => throw new NotImplementedException();

    /// <inheritdoc/>
    public Task<T> GetDrawConfigAsync<T>(RodelDraw.Models.Constants.ProviderType type)
        where T : class => throw new NotImplementedException();

    /// <inheritdoc/>
    public Task<List<DrawSession>?> GetDrawSessionsAsync() => throw new NotImplementedException();

    /// <inheritdoc/>
    public Task<T> GetTranslateConfigAsync<T>(RodelTranslate.Models.Constants.ProviderType type)
        where T : class => throw new NotImplementedException();

    /// <inheritdoc/>
    public Task<List<TranslateSession>?> GetTranslateSessionsAsync(RodelTranslate.Models.Constants.ProviderType type) => throw new NotImplementedException();

    /// <inheritdoc/>
    public string GetWorkingDirectory() => throw new NotImplementedException();

    /// <inheritdoc/>
    public Task RemoveAudioSessionAsync(string sessionId) => throw new NotImplementedException();

    /// <inheritdoc/>
    public Task RemoveChatAgentAsync(string agentId) => throw new NotImplementedException();

    /// <inheritdoc/>
    public Task RemoveChatSessionAsync(string sessionId) => throw new NotImplementedException();

    /// <inheritdoc/>
    public Task RemoveChatSessionPresetAsync(string presetId) => throw new NotImplementedException();

    /// <inheritdoc/>
    public Task RemoveDrawSessionAsync(string sessionId) => throw new NotImplementedException();

    /// <inheritdoc/>
    public Task RemoveTranslateSessionAsync(string sessionId) => throw new NotImplementedException();

    /// <inheritdoc/>
    public async Task<string> RetrieveAzureSpeechVoicesAsync()
    {
        if (File.Exists(AzureSpeechVoiceFileName))
        {
            var json = await File.ReadAllTextAsync(AzureSpeechVoiceFileName);
            return json;
        }

        return string.Empty;
    }

    /// <inheritdoc/>
    public Task SaveAzureSpeechVoicesAsync(string json)
        => File.WriteAllTextAsync(AzureSpeechVoiceFileName, json);

    /// <inheritdoc/>
    public Task SetAudioConfigAsync<T>(ProviderType type, T config)
        where T : class
        => throw new NotImplementedException();

    /// <inheritdoc/>
    public Task SetChatConfigAsync<T>(RodelChat.Models.Constants.ProviderType type, T config)
        where T : class => throw new NotImplementedException();

    /// <inheritdoc/>
    public Task SetDrawConfigAsync<T>(RodelDraw.Models.Constants.ProviderType type, T config)
        where T : class => throw new NotImplementedException();

    /// <inheritdoc/>
    public Task SetTranslateConfigAsync<T>(RodelTranslate.Models.Constants.ProviderType type, T config)
        where T : class => throw new NotImplementedException();

    /// <inheritdoc/>
    public void SetWorkingDirectory(string workingDirectory) => throw new NotImplementedException();
}
