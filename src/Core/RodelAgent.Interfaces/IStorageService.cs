// Copyright (c) Rodel. All rights reserved.

using System.Collections.Generic;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;
using Richasy.AgentKernel;
using RodelAudio.Models.Client;
using RodelChat.Models.Client;
using RodelDraw.Models.Client;
using RodelTranslate.Models.Client;

namespace RodelAgent.Interfaces;

/// <summary>
/// 存储服务.
/// </summary>
public interface IStorageService
{
    /// <summary>
    /// 获取工作目录.
    /// </summary>
    /// <returns>目录.</returns>
    string GetWorkingDirectory();

    /// <summary>
    /// 设置工作目录.
    /// </summary>
    /// <param name="workingDirectory">工作目录.</param>
    void SetWorkingDirectory(string workingDirectory);

    /// <summary>
    /// 获取聊天供应商配置.
    /// </summary>
    /// <typeparam name="T">供应商配置类型.</typeparam>
    /// <param name="type">供应商类型.</param>
    /// <param name="typeInfo">类型信息.</param>
    /// <returns>配置.</returns>
    Task<T> GetChatConfigAsync<T>(ChatProviderType type, JsonTypeInfo<T> typeInfo)
        where T : class;

    /// <summary>
    /// 设置聊天供应商配置.
    /// </summary>
    /// <typeparam name="T">供应商配置类型.</typeparam>
    /// <param name="type">供应商类型.</param>
    /// <param name="config">配置.</param>
    /// <param name="typeInfo">类型信息.</param>
    /// <returns><see cref="Task"/>.</returns>
    Task SetChatConfigAsync<T>(ChatProviderType type, T config, JsonTypeInfo<T> typeInfo)
        where T : class;

    /// <summary>
    /// 获取翻译供应商配置.
    /// </summary>
    /// <typeparam name="T">供应商配置类型.</typeparam>
    /// <param name="type">供应商类型.</param>
    /// <param name="typeInfo">类型信息.</param>
    /// <returns>配置.</returns>
    Task<T> GetTranslateConfigAsync<T>(TranslateProviderType type, JsonTypeInfo<T> typeInfo)
        where T : class;

    /// <summary>
    /// 设置翻译供应商配置.
    /// </summary>
    /// <typeparam name="T">供应商配置类型.</typeparam>
    /// <param name="type">供应商类型.</param>
    /// <param name="config">配置.</param>
    /// <param name="typeInfo">类型信息.</param>
    /// <returns><see cref="Task"/>.</returns>
    Task SetTranslateConfigAsync<T>(TranslateProviderType type, T config, JsonTypeInfo<T> typeInfo)
        where T : class;

    /// <summary>
    /// 获取绘图供应商配置.
    /// </summary>
    /// <typeparam name="T">供应商配置类型.</typeparam>
    /// <param name="type">供应商类型.</param>
    /// <param name="typeInfo">类型信息.</param>
    /// <returns>配置.</returns>
    Task<T> GetDrawConfigAsync<T>(DrawProviderType type, JsonTypeInfo<T> typeInfo)
        where T : class;

    /// <summary>
    /// 设置绘图供应商配置.
    /// </summary>
    /// <typeparam name="T">供应商配置类型.</typeparam>
    /// <param name="type">供应商类型.</param>
    /// <param name="config">配置.</param>
    /// <param name="typeInfo">类型信息.</param>
    /// <returns><see cref="Task"/>.</returns>
    Task SetDrawConfigAsync<T>(DrawProviderType type, T config, JsonTypeInfo<T> typeInfo)
        where T : class;

    /// <summary>
    /// 获取音频供应商配置.
    /// </summary>
    /// <typeparam name="T">供应商配置类型.</typeparam>
    /// <param name="type">供应商类型.</param>
    /// <param name="typeInfo">类型信息.</param>
    /// <returns>配置.</returns>
    Task<T> GetAudioConfigAsync<T>(AudioProviderType type, JsonTypeInfo<T> typeInfo)
        where T : class;

    /// <summary>
    /// 设置音频供应商配置.
    /// </summary>
    /// <typeparam name="T">供应商配置类型.</typeparam>
    /// <param name="type">供应商类型.</param>
    /// <param name="config">配置.</param>
    /// <param name="typeInfo">类型信息.</param>
    /// <returns><see cref="Task"/>.</returns>
    Task SetAudioConfigAsync<T>(AudioProviderType type, T config, JsonTypeInfo<T> typeInfo)
        where T : class;

    /// <summary>
    /// 获取指定供应商的聊天会话.
    /// </summary>
    /// <param name="type">供应商类型.</param>
    /// <returns>会话列表.</returns>
    Task<List<ChatSession>?> GetChatSessionsAsync(ChatProviderType type);

    /// <summary>
    /// 获取指定预设的聊天会话.
    /// </summary>
    /// <param name="presetId">预设标识符.</param>
    /// <returns>会话列表.</returns>
    Task<List<ChatSession>?> GetChatSessionsAsync(string presetId);

    /// <summary>
    /// 添加或更新聊天会话.
    /// </summary>
    /// <param name="session">会话.</param>
    /// <returns><see cref="Task"/>.</returns>
    Task AddOrUpdateChatSessionAsync(ChatSession session);

    /// <summary>
    /// 移除聊天会话.
    /// </summary>
    /// <param name="sessionId">会话标识符.</param>
    /// <returns><see cref="Task"/>.</returns>
    Task RemoveChatSessionAsync(string sessionId);

    /// <summary>
    /// 获取指定预设的群组会话.
    /// </summary>
    /// <param name="presetId">预设标识符.</param>
    /// <returns>会话列表.</returns>
    Task<List<ChatGroup>?> GetChatGroupSessionsAsync(string presetId);

    /// <summary>
    /// 添加或更新群组会话.
    /// </summary>
    /// <param name="session">会话.</param>
    /// <returns><see cref="Task"/>.</returns>
    Task AddOrUpdateChatGroupSessionAsync(ChatGroup session);

    /// <summary>
    /// 移除聊天群组会话.
    /// </summary>
    /// <param name="sessionId">会话标识符.</param>
    /// <returns><see cref="Task"/>.</returns>
    Task RemoveChatGroupSessionAsync(string sessionId);

    /// <summary>
    /// 获取聊天会话预设.
    /// </summary>
    /// <returns>预设列表.</returns>
    Task<List<ChatSessionPresetOld>> GetChatSessionPresetsAsync();

    /// <summary>
    /// 获取指定 ID 的聊天会话预设.
    /// </summary>
    /// <param name="presetId">预设 ID.</param>
    /// <returns><see cref="ChatSessionPresetOld"/>.</returns>
    Task<ChatSessionPresetOld> GetChatSessionPresetByIdAsync(string presetId);

    /// <summary>
    /// 添加或更新聊天会话预设.
    /// </summary>
    /// <param name="preset">预设.</param>
    /// <returns><see cref="Task"/>.</returns>
    Task AddOrUpdateChatSessionPresetAsync(ChatSessionPresetOld preset);

    /// <summary>
    /// 移除聊天会话预设.
    /// </summary>
    /// <param name="presetId">预设标识符.</param>
    /// <returns><see cref="Task"/>.</returns>
    Task RemoveChatSessionPresetAsync(string presetId);

    /// <summary>
    /// 获取本地助理列表.
    /// </summary>
    /// <returns>助理列表.</returns>
    Task<List<ChatSessionPresetOld>> GetChatAgentsAsync();

    /// <summary>
    /// 添加或更新助理.
    /// </summary>
    /// <param name="agent">助理信息.</param>
    /// <returns><see cref="Task"/>.</returns>
    Task AddOrUpdateChatAgentAsync(ChatSessionPresetOld agent);

    /// <summary>
    /// 移除助理.
    /// </summary>
    /// <param name="agentId">助理名称.</param>
    /// <returns><see cref="Task"/>.</returns>
    Task RemoveChatAgentAsync(string agentId);

    /// <summary>
    /// 移除群组.
    /// </summary>
    /// <param name="presetId">群组标识符.</param>
    /// <returns><see cref="Task"/>.</returns>
    Task RemoveChatGroupPresetAsync(string presetId);

    /// <summary>
    /// 获取会话群组预设列表.
    /// </summary>
    /// <returns>助理列表.</returns>
    Task<List<ChatGroupPreset>> GetChatGroupPresetsAsync();

    /// <summary>
    /// 获取指定 ID 的群组会话预设.
    /// </summary>
    /// <param name="presetId">预设 ID.</param>
    /// <returns><see cref="ChatGroupPreset"/>.</returns>
    Task<ChatGroupPreset> GetChatGroupPresetByIdAsync(string presetId);

    /// <summary>
    /// 添加或更新群组预设.
    /// </summary>
    /// <param name="preset">群组信息.</param>
    /// <returns><see cref="Task"/>.</returns>
    Task AddOrUpdateChatGroupPresetAsync(ChatGroupPreset preset);

    /// <summary>
    /// 获取指定供应商的翻译会话.
    /// </summary>
    /// <param name="type">供应商类型.</param>
    /// <returns>会话列表.</returns>
    Task<List<TranslateSession>?> GetTranslateSessionsAsync(TranslateProviderType type);

    /// <summary>
    /// 添加或更新翻译会话.
    /// </summary>
    /// <param name="session">会话.</param>
    /// <returns><see cref="Task"/>.</returns>
    Task AddOrUpdateTranslateSessionAsync(TranslateSession session);

    /// <summary>
    /// 移除翻译会话.
    /// </summary>
    /// <param name="sessionId">会话标识符.</param>
    /// <returns><see cref="Task"/>.</returns>
    Task RemoveTranslateSessionAsync(string sessionId);

    /// <summary>
    /// 获取所有绘图会话.
    /// </summary>
    /// <returns>会话列表.</returns>
    Task<List<DrawSession>?> GetDrawSessionsAsync();

    /// <summary>
    /// 添加或更新绘图会话.
    /// </summary>
    /// <param name="session">会话.</param>
    /// <param name="imageData">图像数据.</param>
    /// <returns><see cref="Task"/>.</returns>
    Task AddOrUpdateDrawSessionAsync(DrawSession session, byte[]? imageData);

    /// <summary>
    /// 移除绘图会话.
    /// </summary>
    /// <param name="sessionId">会话标识符.</param>
    /// <returns><see cref="Task"/>.</returns>
    Task RemoveDrawSessionAsync(string sessionId);

    /// <summary>
    /// 获取所有音频会话.
    /// </summary>
    /// <returns>会话列表.</returns>
    Task<List<AudioSession>?> GetAudioSessionsAsync();

    /// <summary>
    /// 添加或更新音频会话.
    /// </summary>
    /// <param name="session">会话.</param>
    /// <param name="audioData">音频数据.</param>
    /// <returns><see cref="Task"/>.</returns>
    Task AddOrUpdateAudioSessionAsync(AudioSession session, byte[]? audioData);

    /// <summary>
    /// 移除音频会话.
    /// </summary>
    /// <param name="sessionId">会话标识符.</param>
    /// <returns><see cref="Task"/>.</returns>
    Task RemoveAudioSessionAsync(string sessionId);

    /// <summary>
    /// 获取 Azure 语音服务的语音列表（JSON）.
    /// </summary>
    /// <returns>Azure 语音服务 JSON 数据.</returns>
    Task<string> RetrieveAzureSpeechVoicesAsync();

    /// <summary>
    /// 保存 Azure 语音服务的语音列表（JSON）.
    /// </summary>
    /// <param name="json">JSON 数据.</param>
    /// <returns><see cref="Task"/>.</returns>
    Task SaveAzureSpeechVoicesAsync(string json);
}
