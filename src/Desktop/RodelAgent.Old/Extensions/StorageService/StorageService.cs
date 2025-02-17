// Copyright (c) Richasy. All rights reserved.

using Richasy.AgentKernel;
using RodelAgent.Context;
using RodelAgent.Interfaces;
using RodelAgent.UI.Models.Constants;
using RodelAgent.UI.Toolkits;
using RodelAudio.Interfaces.Client;
using RodelAudio.Models.Client;
using RodelChat.Models.Client;
using RodelDraw.Interfaces.Client;
using RodelDraw.Models.Client;
using RodelTranslate.Interfaces.Client;
using RodelTranslate.Models.Client;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace RodelAgent.UI.Extensions;

/// <summary>
/// 存储服务.
/// </summary>
public sealed partial class StorageService : IStorageService
{
    private const string AzureSpeechVoicesKey = "AzureSpeechVoices.json";
    private readonly DbService _dbService;
    private readonly IChatParametersFactory _chatParametersFactory;
    private readonly IDrawParametersFactory _drawParametersFactory;
    private readonly IAudioParametersFactory _audioParametersFactory;
    private readonly ITranslateParametersFactory _translateParametersFactory;
    private List<ChatSession> _chatSessions;
    private List<ChatSessionPreset> _chatSessionPresets;
    private List<ChatSessionPreset> _chatAgents;
    private List<ChatGroupPreset> _chatGroupPresets;
    private List<ChatGroup> _chatGroups;
    private List<TranslateSession> _translateSessions;
    private List<DrawSession> _drawSessions;
    private List<AudioSession> _audioSessions;

    /// <summary>
    /// Initializes a new instance of the <see cref="StorageService"/> class.
    /// </summary>
    /// <param name="dbService">数据服务.</param>
    public StorageService(
        DbService dbService,
        IChatParametersFactory chatParametersFactory,
        ITranslateParametersFactory translateParameterFactory,
        IDrawParametersFactory drawParametersFactory,
        IAudioParametersFactory audioParametersFactory)
    {
        _dbService = dbService;
        _dbService.SetWorkingDirectory(GetWorkingDirectory());
        _chatParametersFactory = chatParametersFactory;
        _translateParametersFactory = translateParameterFactory;
        _drawParametersFactory = drawParametersFactory;
        _audioParametersFactory = audioParametersFactory;
    }

    /// <inheritdoc/>
    public async Task<T> GetChatConfigAsync<T>(ChatProviderType type, JsonTypeInfo<T> typeInfo)
        where T : class
    {
        var json = await _dbService.GetSecretAsync("Chat_" + type.ToString());
        return typeof(T).Equals(typeof(string))
            ? json as T
            : json is null
                ? default
                : JsonSerializer.Deserialize(json, typeInfo);
    }

    /// <inheritdoc/>
    public async Task SetChatConfigAsync<T>(ChatProviderType type, T config, JsonTypeInfo<T> typeInfo)
        where T : class
    {
        var json = typeof(T).Equals(typeof(string)) ? config as string : JsonSerializer.Serialize(config, typeInfo);
        await _dbService.SetSecretAsync("Chat_" + type.ToString(), json);
    }

    /// <inheritdoc/>
    public async Task<T> GetTranslateConfigAsync<T>(TranslateProviderType type, JsonTypeInfo<T> typeInfo)
        where T : class
    {
        var json = await _dbService.GetSecretAsync("Trans_" + type.ToString());
        return typeof(T).Equals(typeof(string))
            ? json as T
            : json is null
                ? default
                : JsonSerializer.Deserialize<T>(json, typeInfo);
    }

    /// <inheritdoc/>
    public async Task SetTranslateConfigAsync<T>(TranslateProviderType type, T config, JsonTypeInfo<T> typeInfo)
        where T : class
    {
        var json = typeof(T).Equals(typeof(string)) ? config as string : JsonSerializer.Serialize(config, typeInfo);
        await _dbService.SetSecretAsync("Trans_" + type.ToString(), json);
    }

    /// <inheritdoc/>
    public async Task<T> GetDrawConfigAsync<T>(DrawProviderType type, JsonTypeInfo<T> typeInfo)
        where T : class
    {
        var json = await _dbService.GetSecretAsync("Draw_" + type.ToString());
        return typeof(T).Equals(typeof(string))
            ? json as T
            : json is null
                ? default
                : JsonSerializer.Deserialize<T>(json, typeInfo);
    }

    /// <inheritdoc/>
    public async Task SetDrawConfigAsync<T>(DrawProviderType type, T config, JsonTypeInfo<T> typeInfo)
        where T : class
    {
        var json = typeof(T).Equals(typeof(string)) ? config as string : JsonSerializer.Serialize(config, typeInfo);
        await _dbService.SetSecretAsync("Draw_" + type.ToString(), json);
    }

    /// <inheritdoc/>
    public async Task<T> GetAudioConfigAsync<T>(AudioProviderType type, JsonTypeInfo<T> typeInfo)
        where T : class
    {
        var json = await _dbService.GetSecretAsync("Audio_" + type.ToString());
        return typeof(T).Equals(typeof(string))
            ? json as T
            : json is null
                ? default
                : JsonSerializer.Deserialize<T>(json, typeInfo);
    }

    /// <inheritdoc/>
    public async Task SetAudioConfigAsync<T>(AudioProviderType type, T config, JsonTypeInfo<T> typeInfo)
        where T : class
    {
        var json = typeof(T).Equals(typeof(string)) ? config as string : JsonSerializer.Serialize(config, typeInfo);
        await _dbService.SetSecretAsync("Audio_" + type.ToString(), json);
    }

    /// <inheritdoc/>
    public string GetWorkingDirectory()
        => SettingsToolkit.ReadLocalSetting(SettingNames.WorkingDirectory, string.Empty);

    /// <inheritdoc/>
    public void SetWorkingDirectory(string workingDirectory)
    {
        SettingsToolkit.WriteLocalSetting(SettingNames.WorkingDirectory, workingDirectory);
        _dbService.SetWorkingDirectory(workingDirectory);
    }

    /// <inheritdoc/>
    public Task<string> RetrieveAzureSpeechVoicesAsync()
    {
        var path = Path.Combine(GetWorkingDirectory(), AzureSpeechVoicesKey);
        return File.Exists(path)
            ? File.ReadAllTextAsync(path)
            : Task.FromResult(string.Empty);
    }

    /// <inheritdoc/>
    public Task SaveAzureSpeechVoicesAsync(string json)
    {
        var path = Path.Combine(GetWorkingDirectory(), AzureSpeechVoicesKey);
        return File.WriteAllTextAsync(path, json);
    }
}
