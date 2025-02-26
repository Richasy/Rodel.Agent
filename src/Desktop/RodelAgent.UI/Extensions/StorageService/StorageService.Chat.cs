// Copyright (c) Richasy. All rights reserved.

using Richasy.AgentKernel;
using RodelAgent.Models.Feature;
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

    public async Task<List<ChatConversation>?> GetChatConversationsAsync(string presetId)
    {
        var sessions = await GetAllChatConversationsAsync();
        return [.. sessions.Where(p => p.PresetId == presetId)];
    }

    public async Task AddOrUpdateChatConversationAsync(ChatConversation session)
    {
        var json = JsonSerializer.Serialize(session, JsonGenContext.Default.ChatConversation);
        await _dbService.AddOrUpdateChatDataAsync(session.Id, json);
    }

    public Task RemoveChatConversationAsync(string conversationId)
        => _dbService.RemoveChatDataAsync(conversationId);

    private async Task<List<ChatConversation>> GetAllChatConversationsAsync()
    {
        var sessionJsonList = await _dbService.GetAllChatConversationsAsync();
        var sessionList = new List<ChatConversation>();
        foreach (var sessionJson in sessionJsonList)
        {
            var sjson = sessionJson;
            // TODO: 处理旧数据.
            if (string.IsNullOrEmpty(sjson))
            {
                continue;
            }

            var session = JsonSerializer.Deserialize(sjson, JsonGenContext.Default.ChatConversation);
            if (session != null)
            {
                sessionList.Add(session);
            }
        }

        return sessionList;
    }
}
