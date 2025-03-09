// Copyright (c) Richasy. All rights reserved.

using Richasy.AgentKernel;
using RodelAgent.Models.Common;
using RodelAgent.UI.Toolkits;
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

    public async Task<List<DrawRecord>?> GetDrawSessionsAsync()
    {
        var sessionJsonList = await _dbService.GetAllDrawSessionsAsync();
        if (sessionJsonList is null || sessionJsonList.Count == 0)
        {
            return [];
        }

        var sessionList = new List<DrawRecord>();
        foreach (var sessionJson in sessionJsonList)
        {
            if (string.IsNullOrEmpty(sessionJson))
            {
                continue;
            }

            var session = JsonSerializer.Deserialize(sessionJson, JsonGenContext.Default.DrawRecord);
            if (session != null)
            {
                sessionList.Add(session);
            }
        }

        return sessionList;
    }

    public async Task AddOrUpdateDrawSessionAsync(DrawRecord session, byte[]? imageData)
    {
        var json = JsonSerializer.Serialize(session, JsonGenContext.Default.DrawRecord);
        await _dbService.AddOrUpdateDrawDataAsync(session.Id, json);

        if (imageData == null || imageData.Length == 0)
        {
            return;
        }

        var imagePath = AppToolkit.GetDrawPicturePath(session.Id);
        if (!Directory.Exists(Path.GetDirectoryName(imagePath)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(imagePath)!);
        }

        await File.WriteAllBytesAsync(imagePath, imageData);
    }

    public async Task RemoveDrawSessionAsync(string sessionId)
    {
        var imagePath = AppToolkit.GetDrawPicturePath(sessionId);
        if (File.Exists(imagePath))
        {
            await Task.Run(() => File.Delete(imagePath));
        }

        await _dbService.RemoveDrawDataAsync(sessionId);
    }
}
