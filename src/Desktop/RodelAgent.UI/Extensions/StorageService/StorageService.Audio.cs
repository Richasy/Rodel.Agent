﻿// Copyright (c) Richasy. All rights reserved.

using Richasy.AgentKernel;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace RodelAgent.UI.Extensions;

internal sealed partial class StorageService
{
    public async Task<T?> GetAudioConfigAsync<T>(AudioProviderType type, JsonTypeInfo<T> typeInfo) where T : class
    {
        var json = await _dbService.GetSecretAsync("Audio_" + type.ToString());
        return typeof(T).Equals(typeof(string))
            ? json as T
            : json is null
                ? default
                : JsonSerializer.Deserialize(json, typeInfo);
    }

    public async Task SetAudioConfigAsync<T>(AudioProviderType type, T config, JsonTypeInfo<T> typeInfo) where T : class
    {
        var json = typeof(T).Equals(typeof(string)) ? config as string : JsonSerializer.Serialize(config, typeInfo);
        await _dbService.SetSecretAsync("Audio_" + type.ToString(), json!);
    }
}
