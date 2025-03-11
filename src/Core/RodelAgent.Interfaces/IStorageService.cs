﻿// Copyright (c) Richasy. All rights reserved.

using Richasy.AgentKernel;
using RodelAgent.Models.Common;
using RodelAgent.Models.Feature;
using System.Text.Json.Serialization.Metadata;

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
    Task<T?> GetChatConfigAsync<T>(ChatProviderType type, JsonTypeInfo<T> typeInfo)
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
    Task<T?> GetTranslateConfigAsync<T>(TranslateProviderType type, JsonTypeInfo<T> typeInfo)
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
    Task<T?> GetDrawConfigAsync<T>(DrawProviderType type, JsonTypeInfo<T> typeInfo)
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
    Task<T?> GetAudioConfigAsync<T>(AudioProviderType type, JsonTypeInfo<T> typeInfo)
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
    Task<List<ChatConversation>?> GetChatConversationsAsync(ChatProviderType type);

    /// <summary>
    /// 获取指定助理的聊天会话.
    /// </summary>
    /// <param name="agentId">助理标识符.</param>
    /// <returns>会话列表.</returns>
    Task<List<ChatConversation>?> GetChatConversationsByAgentAsync(string agentId);

    /// <summary>
    /// 获取指定群组的聊天会话.
    /// </summary>
    /// <param name="groupId">群组标识符.</param>
    /// <returns>会话列表.</returns>
    Task<List<ChatConversation>?> GetChatConversationsByGroupAsync(string groupId);

    /// <summary>
    /// 添加或更新聊天会话.
    /// </summary>
    /// <param name="conversation">会话.</param>
    /// <returns><see cref="Task"/>.</returns>
    Task AddOrUpdateChatConversationAsync(ChatConversation conversation);

    /// <summary>
    /// 移除聊天会话.
    /// </summary>
    /// <param name="conversationId">会话标识符.</param>
    /// <returns><see cref="Task"/>.</returns>
    Task RemoveChatConversationAsync(string conversationId);

    /// <summary>
    /// 获取本地助理列表.
    /// </summary>
    /// <returns>助理列表.</returns>
    Task<List<ChatAgent>> GetChatAgentsAsync();

    /// <summary>
    /// 添加或更新助理.
    /// </summary>
    /// <param name="agent">助理信息.</param>
    /// <returns><see cref="Task"/>.</returns>
    Task AddOrUpdateChatAgentAsync(ChatAgent agent);

    /// <summary>
    /// 移除助理.
    /// </summary>
    /// <param name="agentId">助理名称.</param>
    /// <returns><see cref="Task"/>.</returns>
    Task RemoveChatAgentAsync(string agentId);

    /// <summary>
    /// 移除群组.
    /// </summary>
    /// <param name="groupId">群组标识符.</param>
    /// <returns><see cref="Task"/>.</returns>
    Task RemoveChatGroupAsync(string groupId);

    /// <summary>
    /// 获取会话群组预设列表.
    /// </summary>
    /// <returns>助理列表.</returns>
    Task<List<ChatGroup>> GetChatGroupsAsync();

    /// <summary>
    /// 获取指定 ID 的群组会话预设.
    /// </summary>
    /// <param name="groupId">预设 ID.</param>
    /// <returns><see cref="ChatGroup"/>.</returns>
    Task<ChatGroup?> GetChatGroupByIdAsync(string groupId);

    /// <summary>
    /// 添加或更新群组预设.
    /// </summary>
    /// <param name="group">群组信息.</param>
    /// <returns><see cref="Task"/>.</returns>
    Task AddOrUpdateChatGroupAsync(ChatGroup group);

    /// <summary>
    /// 获取所有绘图会话.
    /// </summary>
    /// <returns>会话列表.</returns>
    Task<List<DrawRecord>?> GetDrawSessionsAsync();

    /// <summary>
    /// 添加或更新绘图会话.
    /// </summary>
    /// <param name="session">会话.</param>
    /// <param name="imageData">图像数据.</param>
    /// <returns><see cref="Task"/>.</returns>
    Task AddOrUpdateDrawSessionAsync(DrawRecord session, byte[]? imageData);

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
    Task<List<AudioRecord>?> GetAudioSessionsAsync();

    /// <summary>
    /// 添加或更新音频会话.
    /// </summary>
    /// <param name="session">会话.</param>
    /// <param name="audioData">音频数据.</param>
    /// <returns><see cref="Task"/>.</returns>
    Task AddOrUpdateAudioSessionAsync(AudioRecord session, byte[]? audioData);

    /// <summary>
    /// 移除音频会话.
    /// </summary>
    /// <param name="sessionId">会话标识符.</param>
    /// <returns><see cref="Task"/>.</returns>
    Task RemoveAudioSessionAsync(string sessionId);
}
