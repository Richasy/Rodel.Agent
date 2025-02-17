// Copyright (c) Richasy. All rights reserved.

using Richasy.AgentKernel;

// Copyright (c) Richasy. All rights reserved.

using RodelAgent.Interfaces;
using RodelAudio.Models.Client;
using RodelChat.Models.Client;
using RodelDraw.Models.Client;
using RodelTranslate.Models.Client;
using System.Text.Json.Serialization.Metadata;

namespace RodelAgent.UI.Extensions;

internal sealed partial class StorageService : IStorageService
{
    public Task AddOrUpdateAudioSessionAsync(AudioSession session, byte[]? audioData) => throw new NotImplementedException();
    public Task AddOrUpdateChatAgentAsync(ChatSessionPreset agent) => throw new NotImplementedException();
    public Task AddOrUpdateChatGroupPresetAsync(ChatGroupPreset preset) => throw new NotImplementedException();
    public Task AddOrUpdateChatGroupSessionAsync(ChatGroup session) => throw new NotImplementedException();
    public Task AddOrUpdateChatSessionAsync(ChatSession session) => throw new NotImplementedException();
    public Task AddOrUpdateChatSessionPresetAsync(ChatSessionPreset preset) => throw new NotImplementedException();
    public Task AddOrUpdateDrawSessionAsync(DrawSession session, byte[]? imageData) => throw new NotImplementedException();
    public Task AddOrUpdateTranslateSessionAsync(TranslateSession session) => throw new NotImplementedException();
    public Task<T> GetAudioConfigAsync<T>(AudioProviderType type, JsonTypeInfo<T> typeInfo) where T : class => throw new NotImplementedException();
    public Task<List<AudioSession>?> GetAudioSessionsAsync() => throw new NotImplementedException();
    public Task<List<ChatSessionPreset>> GetChatAgentsAsync() => throw new NotImplementedException();
    public Task<T> GetChatConfigAsync<T>(ChatProviderType type, JsonTypeInfo<T> typeInfo) where T : class => throw new NotImplementedException();
    public Task<ChatGroupPreset> GetChatGroupPresetByIdAsync(string presetId) => throw new NotImplementedException();
    public Task<List<ChatGroupPreset>> GetChatGroupPresetsAsync() => throw new NotImplementedException();
    public Task<List<ChatGroup>?> GetChatGroupSessionsAsync(string presetId) => throw new NotImplementedException();
    public Task<ChatSessionPreset> GetChatSessionPresetByIdAsync(string presetId) => throw new NotImplementedException();
    public Task<List<ChatSessionPreset>> GetChatSessionPresetsAsync() => throw new NotImplementedException();
    public Task<List<ChatSession>?> GetChatSessionsAsync(ChatProviderType type) => throw new NotImplementedException();
    public Task<List<ChatSession>?> GetChatSessionsAsync(string presetId) => throw new NotImplementedException();
    public Task<T> GetDrawConfigAsync<T>(DrawProviderType type, JsonTypeInfo<T> typeInfo) where T : class => throw new NotImplementedException();
    public Task<List<DrawSession>?> GetDrawSessionsAsync() => throw new NotImplementedException();
    public Task<T> GetTranslateConfigAsync<T>(TranslateProviderType type, JsonTypeInfo<T> typeInfo) where T : class => throw new NotImplementedException();
    public Task<List<TranslateSession>?> GetTranslateSessionsAsync(TranslateProviderType type) => throw new NotImplementedException();
    public string GetWorkingDirectory() => throw new NotImplementedException();
    public Task RemoveAudioSessionAsync(string sessionId) => throw new NotImplementedException();
    public Task RemoveChatAgentAsync(string agentId) => throw new NotImplementedException();
    public Task RemoveChatGroupPresetAsync(string presetId) => throw new NotImplementedException();
    public Task RemoveChatGroupSessionAsync(string sessionId) => throw new NotImplementedException();
    public Task RemoveChatSessionAsync(string sessionId) => throw new NotImplementedException();
    public Task RemoveChatSessionPresetAsync(string presetId) => throw new NotImplementedException();
    public Task RemoveDrawSessionAsync(string sessionId) => throw new NotImplementedException();
    public Task RemoveTranslateSessionAsync(string sessionId) => throw new NotImplementedException();
    public Task<string> RetrieveAzureSpeechVoicesAsync() => throw new NotImplementedException();
    public Task SaveAzureSpeechVoicesAsync(string json) => throw new NotImplementedException();
    public Task SetAudioConfigAsync<T>(AudioProviderType type, T config, JsonTypeInfo<T> typeInfo) where T : class => throw new NotImplementedException();
    public Task SetChatConfigAsync<T>(ChatProviderType type, T config, JsonTypeInfo<T> typeInfo) where T : class => throw new NotImplementedException();
    public Task SetDrawConfigAsync<T>(DrawProviderType type, T config, JsonTypeInfo<T> typeInfo) where T : class => throw new NotImplementedException();
    public Task SetTranslateConfigAsync<T>(TranslateProviderType type, T config, JsonTypeInfo<T> typeInfo) where T : class => throw new NotImplementedException();
    public void SetWorkingDirectory(string workingDirectory) => throw new NotImplementedException();
}
