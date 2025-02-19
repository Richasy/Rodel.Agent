// Copyright (c) Richasy. All rights reserved.

using Richasy.AgentKernel;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace RodelAgent.UI.Extensions;

internal sealed partial class StorageService
{
    public async Task<T?> GetDrawConfigAsync<T>(DrawProviderType type, JsonTypeInfo<T> typeInfo) where T : class
    {
        var json = await _dbService.GetSecretAsync("Draw_" + type.ToString());
        return typeof(T).Equals(typeof(string))
            ? json as T
            : json is null
                ? default
                : JsonSerializer.Deserialize(json, typeInfo);
    }

    public async Task SetDrawConfigAsync<T>(DrawProviderType type, T config, JsonTypeInfo<T> typeInfo) where T : class
    {
        var json = typeof(T).Equals(typeof(string)) ? config as string : JsonSerializer.Serialize(config, typeInfo);
        await _dbService.SetSecretAsync("Draw_" + type.ToString(), json!);
    }
}
