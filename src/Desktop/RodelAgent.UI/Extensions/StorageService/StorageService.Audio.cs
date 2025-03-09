﻿// Copyright (c) Richasy. All rights reserved.

using Richasy.AgentKernel;
using RodelAgent.Models.Common;
using RodelAgent.UI.Toolkits;
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

    public async Task<List<AudioRecord>?> GetAudioSessionsAsync()
    {
        var sessionJsonList = await _dbService.GetAllAudioSessionsAsync();
        if (sessionJsonList is null || sessionJsonList.Count == 0)
        {
            return [];
        }

        var sessionList = new List<AudioRecord>();
        foreach (var sessionJson in sessionJsonList)
        {
            if (string.IsNullOrEmpty(sessionJson))
            {
                continue;
            }

            var session = JsonSerializer.Deserialize(sessionJson, JsonGenContext.Default.AudioRecord);
            if (session != null)
            {
                sessionList.Add(session);
            }
        }

        return sessionList;
    }

    public async Task AddOrUpdateAudioSessionAsync(AudioRecord session, byte[]? audioData)
    {
        var json = JsonSerializer.Serialize(session, JsonGenContext.Default.AudioRecord);
        await _dbService.AddOrUpdateAudioDataAsync(session.Id, json);

        if (audioData == null || audioData.Length == 0)
        {
            return;
        }

        var audioPath = AppToolkit.GetAudioPath(session.Id, true);
        if (!Directory.Exists(Path.GetDirectoryName(audioPath)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(audioPath)!);
        }

        if (session.Provider == AudioProviderType.Edge)
        {
            audioPath = audioPath.Replace(".wav", ".mp3", StringComparison.OrdinalIgnoreCase);
        }

        await File.WriteAllBytesAsync(audioPath, audioData);
    }

    public async Task RemoveAudioSessionAsync(string sessionId)
    {
        var audioPath = AppToolkit.GetAudioPath(sessionId);
        if (File.Exists(audioPath))
        {
            await Task.Run(() => File.Delete(audioPath));
        }

        await _dbService.RemoveAudioDataAsync(sessionId);
    }
}
