// Copyright (c) Richasy. All rights reserved.

using Richasy.AgentKernel;
using RodelAgent.Models.Feature;
using RodelAgent.UI.Toolkits;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace RodelAgent.UI.Extensions;

internal sealed partial class StorageService
{
    /// <inheritdoc/>
    public async Task<T?> GetChatConfigAsync<T>(ChatProviderType type, JsonTypeInfo<T> typeInfo) where T : class
    {
        var json = await _dbService.GetSecretAsync("Chat_" + type.ToString());
        return typeof(T).Equals(typeof(string))
            ? json as T
            : json is null
                ? default
                : JsonSerializer.Deserialize(json, typeInfo);
    }

    /// <inheritdoc/>
    public async Task SetChatConfigAsync<T>(ChatProviderType type, T config, JsonTypeInfo<T> typeInfo) where T : class
    {
        var json = typeof(T).Equals(typeof(string)) ? config as string : JsonSerializer.Serialize(config, typeInfo);
        await _dbService.SetSecretAsync("Chat_" + type.ToString(), json!);
    }

    public async Task<List<ChatConversation>?> GetChatConversationsAsync(ChatProviderType type)
    {
        var sessions = await GetAllChatConversationsAsync();
        return [.. sessions.Where(p => p.Provider == type)];
    }

    public async Task<List<ChatConversation>?> GetChatConversationsByAgentAsync(string agentId)
    {
        var sessions = await GetAllChatConversationsAsync();
        return [.. sessions.Where(p => p.AgentId == agentId)];
    }

    public async Task AddOrUpdateChatConversationAsync(ChatConversation session)
    {
        var json = JsonSerializer.Serialize(session, JsonGenContext.Default.ChatConversation);
        await _dbService.AddOrUpdateChatDataAsync(session.Id, json);
    }

    public Task RemoveChatConversationAsync(string conversationId)
        => _dbService.RemoveChatDataAsync(conversationId);

    public async Task<List<ChatAgent>> GetChatAgentsAsync()
    {
        await InitializeChatAgentsInternalAsync();
        return _chatAgents;
    }

    public async Task AddOrUpdateChatAgentAsync(ChatAgent agent)
    {
        await InitializeChatAgentsInternalAsync();
        if (_chatAgents.Any(p => p.Id == agent.Id))
        {
            _chatAgents.Remove(_chatAgents.First(p => p.Id == agent.Id));
        }

        _chatAgents.Add(agent);
        var json = JsonSerializer.Serialize(agent, JsonGenContext.Default.ChatAgent);
        await File.WriteAllTextAsync(Path.Combine(GetWorkingDirectory(), "Agents", agent.Id + ".json"), json);
    }

    public async Task RemoveChatAgentAsync(string agentId)
    {
        await InitializeChatAgentsInternalAsync();
        _chatAgents.RemoveAll(p => p.Id == agentId);
        var file = Path.Combine(GetWorkingDirectory(), "Agents", agentId + ".json");
        if (File.Exists(file))
        {
            await Task.Run(() => File.Delete(file));
        }
    }

    public async Task<List<ChatGroup>> GetChatGroupsAsync()
    {
        await InitializeChatGroupsInternalAsync();
        return _chatGroups;
    }

    public async Task AddOrUpdateChatGroupAsync(ChatGroup group)
    {
        await InitializeChatGroupsInternalAsync();
        if (_chatGroups.Any(p => p.Id == group.Id))
        {
            _chatGroups.Remove(_chatGroups.First(p => p.Id == group.Id));
        }

        _chatGroups.Add(group);
        var json = JsonSerializer.Serialize(group, JsonGenContext.Default.ChatGroup);
        await File.WriteAllTextAsync(Path.Combine(GetWorkingDirectory(), "Groups", group.Id + ".json"), json);
    }

    public async Task<ChatGroup?> GetChatGroupByIdAsync(string groupId)
    {
        await InitializeChatGroupsInternalAsync();
        return _chatGroups.Find(p => p.Id == groupId);
    }

    public async Task RemoveChatGroupAsync(string presetId)
    {
        await InitializeChatGroupsInternalAsync();
        _chatGroups.RemoveAll(p => p.Id == presetId);
        var file = Path.Combine(GetWorkingDirectory(), "Groups", presetId + ".json");
        if (File.Exists(file))
        {
            await Task.Run(() => File.Delete(file));
        }
    }

    private async Task<List<ChatConversation>> GetAllChatConversationsAsync()
    {
        var sessionJsonList = await _dbService.GetAllChatConversationsAsync();
        var sessionList = new List<ChatConversation>();
        foreach (var sessionJson in sessionJsonList)
        {
            if (string.IsNullOrEmpty(sessionJson))
            {
                continue;
            }

            var session = JsonSerializer.Deserialize(sessionJson, JsonGenContext.Default.ChatConversation);
            if (session != null)
            {
                sessionList.Add(session);
            }
        }

        return sessionList;
    }

    private async Task InitializeChatAgentsInternalAsync(bool force = false)
    {
        if (_chatAgents != null && !force)
        {
            return;
        }

        _chatAgents = [];
        var folder = Path.Combine(GetWorkingDirectory(), "Agents");
        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }

        foreach (var file in Directory.GetFiles(folder, "*.json"))
        {
            try
            {
                var json = await File.ReadAllTextAsync(file);
                if (json.Contains("\"parameters\":", StringComparison.OrdinalIgnoreCase))
                {
                    var newAgent = MigrationToolkit.ToChatObject(json, isAgent: true) as ChatAgent;
                    json = JsonSerializer.Serialize(newAgent, JsonGenContext.Default.ChatAgent);
                    await File.WriteAllTextAsync(file, json);
                }

                var agent = JsonSerializer.Deserialize(json, JsonGenContext.Default.ChatAgent);
                _chatAgents.Add(agent!);
            }
            catch (Exception)
            {
                continue;
            }
        }
    }

    private async Task InitializeChatGroupsInternalAsync(bool force = false)
    {
        if (_chatGroups != null && !force)
        {
            return;
        }

        _chatGroups = [];
        var folder = Path.Combine(GetWorkingDirectory(), "Groups");
        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }

        var presetFiles = Directory.GetFiles(folder, "*.json");
        foreach (var file in presetFiles)
        {
            try
            {
                var json = await File.ReadAllTextAsync(file);
                if (json.Contains("\"terminate_text\":", StringComparison.OrdinalIgnoreCase))
                {
                    json = json.Replace("terminate_text", "terminate_sequence", StringComparison.OrdinalIgnoreCase);
                    await File.WriteAllTextAsync(file, json);
                }

                var group = JsonSerializer.Deserialize(json, JsonGenContext.Default.ChatGroup);
                _chatGroups.Add(group!);
            }
            catch (Exception)
            {
                continue;
            }
        }
    }
}
