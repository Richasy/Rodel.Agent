// Copyright (c) Rodel. All rights reserved.

using System.Text.Json;
using RodelChat.Models.Client;
using chatConstants = RodelChat.Models.Constants;

namespace RodelAgent.UI.Extensions;

/// <summary>
/// 存储服务.
/// </summary>
public sealed partial class StorageService
{
    /// <inheritdoc/>
    public async Task<List<ChatSession>> GetChatSessionsAsync(chatConstants.ProviderType type)
    {
        await InitializeChatSessionsAsync();
        var sessions = _chatSessions.Where(s => s.Provider == type).ToList();
        return sessions;
    }

    /// <inheritdoc/>
    public async Task<List<ChatSession>> GetChatSessionsAsync(string presetId)
    {
        await InitializeChatSessionsAsync();
        var sessions = _chatSessions.Where(s => s.PresetId == presetId).ToList();
        return sessions;
    }

    /// <inheritdoc/>
    public async Task AddOrUpdateChatSessionAsync(ChatSession session)
    {
        await InitializeChatSessionsAsync();
        if (_chatSessions.Any(s => s.Id == session.Id))
        {
            _chatSessions.Remove(_chatSessions.First(s => s.Id == session.Id));
        }

        _chatSessions.Add(session);
        var id = session.Id;
        var v = JsonSerializer.Serialize(session);
        await _dbService.AddOrUpdateChatDataAsync(id, v);
    }

    /// <inheritdoc/>
    public async Task RemoveChatSessionAsync(string sessionId)
    {
        _chatSessions.RemoveAll(p => p.Id == sessionId);
        await _dbService.RemoveChatDataAsync(sessionId);
    }

    /// <inheritdoc/>
    public async Task<List<ChatSessionPreset>> GetChatSessionPresetsAsync()
    {
        await InitializePresetInternalAsync(chatConstants.ChatSessionPresetType.Session);
        return _chatSessionPresets;
    }

    /// <inheritdoc/>
    public async Task<ChatSessionPreset> GetChatSessionPresetByIdAsync(string presetId)
    {
        await InitializePresetInternalAsync(chatConstants.ChatSessionPresetType.Session);
        await InitializePresetInternalAsync(chatConstants.ChatSessionPresetType.Agent);
        return _chatSessionPresets.Concat(_chatAgents).FirstOrDefault(p => p.Id == presetId);
    }

    /// <inheritdoc/>
    public Task AddOrUpdateChatSessionPresetAsync(ChatSessionPreset preset)
        => AddOrUpdatePresetInternalAsync(chatConstants.ChatSessionPresetType.Session, preset);

    /// <inheritdoc/>
    public Task RemoveChatSessionPresetAsync(string presetId)
        => RemovePresetInternalAsync(chatConstants.ChatSessionPresetType.Session, presetId);

    /// <inheritdoc/>
    public async Task<List<ChatSessionPreset>> GetChatAgentsAsync()
    {
        await InitializePresetInternalAsync(chatConstants.ChatSessionPresetType.Agent);
        return _chatAgents;
    }

    /// <inheritdoc/>
    public Task AddOrUpdateChatAgentAsync(ChatSessionPreset agent)
        => AddOrUpdatePresetInternalAsync(chatConstants.ChatSessionPresetType.Agent, agent);

    /// <inheritdoc/>
    public Task RemoveChatAgentAsync(string agentId)
        => RemovePresetInternalAsync(chatConstants.ChatSessionPresetType.Agent, agentId);

    private async Task InitializeChatSessionsAsync()
    {
        if (_chatSessions == null)
        {
            var chatSessions = new List<ChatSession>();
            var allSessions = await _dbService.GetAllChatSessionAsync();
            foreach (var session in allSessions)
            {
                try
                {
                    var sessionObj = JsonSerializer.Deserialize<ChatSession>(session);
                    var parameters = _chatParametersFactory.CreateChatParameters(sessionObj.Provider);
                    if (sessionObj.Parameters == null)
                    {
                        sessionObj.Parameters = parameters;
                    }
                    else
                    {
                        parameters.SetDictionary(sessionObj.Parameters.ToDictionary());
                        sessionObj.Parameters = parameters;
                    }

                    chatSessions.Add(sessionObj);
                }
                catch (Exception)
                {
                    continue;
                }
            }

            _chatSessions = chatSessions.OrderByDescending(p => p.Messages?.LastOrDefault()?.Time ?? DateTimeOffset.MinValue).ToList();
        }
    }

    private async Task InitializePresetInternalAsync(chatConstants.ChatSessionPresetType type, bool force = false)
    {
        var set = GetPresetListInternal(type);

        if (set == null || force)
        {
            if (type == chatConstants.ChatSessionPresetType.Session)
            {
                _chatSessionPresets = new List<ChatSessionPreset>();
            }
            else if (type == chatConstants.ChatSessionPresetType.Agent)
            {
                _chatAgents = new List<ChatSessionPreset>();
            }

            set = GetPresetListInternal(type);
            var folder = GetPresetFolderInternal(type);
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            var presetFiles = Directory.GetFiles(folder, "*.json");
            foreach (var file in presetFiles)
            {
                try
                {
                    var preset = await File.ReadAllTextAsync(file);
                    var presetObj = JsonSerializer.Deserialize<ChatSessionPreset>(preset);
                    var parameters = _chatParametersFactory.CreateChatParameters(presetObj.Provider);
                    parameters.SetDictionary(presetObj.Parameters.ToDictionary());
                    presetObj.Parameters = parameters;
                    set.Add(presetObj);
                }
                catch (Exception)
                {
                    continue;
                }
            }
        }
    }

    private async Task AddOrUpdatePresetInternalAsync(chatConstants.ChatSessionPresetType type, ChatSessionPreset preset)
    {
        await InitializePresetInternalAsync(type);
        var set = GetPresetListInternal(type);
        if (set.Any(p => p.Id == preset.Id))
        {
            set.Remove(set.First(p => p.Id == preset.Id));
        }

        set.Add(preset);
        var v = JsonSerializer.Serialize(preset);
        var folder = GetPresetFolderInternal(type);
        var filePath = Path.Combine(folder, $"{preset.Id}.json");
        await File.WriteAllTextAsync(filePath, v);
    }

    private async Task RemovePresetInternalAsync(chatConstants.ChatSessionPresetType type, string presetId)
    {
        await InitializePresetInternalAsync(type);
        var set = GetPresetListInternal(type);
        set.RemoveAll(p => p.Id == presetId);
        var folder = GetPresetFolderInternal(type);
        var filePath = Path.Combine(folder, $"{presetId}.json");
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }

    private string GetPresetFolderInternal(chatConstants.ChatSessionPresetType type)
        => type switch
        {
            chatConstants.ChatSessionPresetType.Session => Path.Combine(GetWorkingDirectory(), "Presets"),
            chatConstants.ChatSessionPresetType.Agent => Path.Combine(GetWorkingDirectory(), "Agents"),
            _ => throw new ArgumentOutOfRangeException(nameof(type)),
        };

    private List<ChatSessionPreset> GetPresetListInternal(chatConstants.ChatSessionPresetType type)
        => type switch
        {
            chatConstants.ChatSessionPresetType.Session => _chatSessionPresets,
            chatConstants.ChatSessionPresetType.Agent => _chatAgents,
            _ => throw new ArgumentOutOfRangeException(nameof(type)),
        };
}
