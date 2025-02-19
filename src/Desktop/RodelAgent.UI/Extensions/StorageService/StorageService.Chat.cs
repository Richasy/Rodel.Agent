// Copyright (c) Richasy. All rights reserved.

using Richasy.AgentKernel;
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
}
