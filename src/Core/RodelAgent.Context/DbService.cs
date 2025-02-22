// Copyright (c) Richasy. All rights reserved.

using Microsoft.EntityFrameworkCore;
using RodelAgent.Models.Common;

namespace RodelAgent.Context;

/// <summary>
/// 数据库服务.
/// </summary>
public sealed class DbService : IDisposable
{
    private SecretDataService? _secretService;
    private DrawDataService? _drawService;
    private AudioDataService? _audioService;
    private ChatDbContext? _chatDb;
    private string _workingDirectory;
    private string _packageDirectory;

    /// <inheritdoc/>
    public void Dispose()
    {
        _secretService?.Dispose();
        _drawService?.Dispose();
        _audioService?.Dispose();
    }

    /// <summary>
    /// 设置工作目录.
    /// </summary>
    /// <param name="workingDirectory">工作目录.</param>
    public void SetWorkingDirectory(string workingDirectory)
        => _workingDirectory = workingDirectory;

    /// <summary>
    /// 设置包目录.
    /// </summary>
    /// <param name="packageDirectory">包目录.</param>
    public void SetPackageDirectory(string packageDirectory)
        => _packageDirectory = packageDirectory;

    /// <summary>
    /// 获取机密配置.
    /// </summary>
    /// <param name="key">名称.</param>
    /// <returns>字符串.</returns>
    public async Task<string?> GetSecretAsync(string key)
    {
        await InitializeSecretServiceAsync().ConfigureAwait(false);
        var data = await _secretService!.GetMetadataAsync(key).ConfigureAwait(false);
        return data?.Value;
    }

    /// <summary>
    /// 设置机密配置.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    public async Task SetSecretAsync(string key, string value)
    {
        await InitializeSecretServiceAsync().ConfigureAwait(false);
        await _secretService!.AddOrUpdateMetadataAsync(new SecretMeta { Id = key, Value = value }).ConfigureAwait(false);
    }

    /// <summary>
    /// 获取所有聊天数据.
    /// </summary>
    /// <returns>JSON 列表.</returns>
    public async Task<List<string>> GetAllChatSessionAsync()
    {
        _chatDb ??= await MigrationUtils.GetChatDbAsync(_workingDirectory).ConfigureAwait(false);
        return await _chatDb.Sessions.Select(p => p.Value).ToListAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// 获取所有群组会话数据.
    /// </summary>
    /// <returns>JSON 列表.</returns>
    public async Task<List<string>> GetAllChatGroupAsync()
    {
        _chatDb ??= await MigrationUtils.GetChatDbAsync(_workingDirectory).ConfigureAwait(false);
        return await _chatDb.Groups.Select(p => p.Value).ToListAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// 添加或更新聊天数据.
    /// </summary>
    /// <param name="dataId">会话标识符.</param>
    /// <param name="value">会话 JSON 数据.</param>
    /// <param name="isGroup">是否为群组数据.</param>
    /// <returns><see cref="Task"/>.</returns>
    public async Task AddOrUpdateChatDataAsync(string dataId, string value, bool isGroup = false)
    {
        _chatDb ??= await MigrationUtils.GetChatDbAsync(_workingDirectory).ConfigureAwait(false);
        var dataset = isGroup ? _chatDb.Groups : _chatDb.Sessions;
        var data = await dataset.FirstOrDefaultAsync(x => x.Id == dataId).ConfigureAwait(false);
        if (data is null)
        {
            await dataset.AddAsync(new Metadata { Id = dataId, Value = value }).ConfigureAwait(false);
        }
        else
        {
            data.Value = value;
            dataset.Update(data);
        }

        await _chatDb.SaveChangesAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// 移除聊天会话.
    /// </summary>
    /// <param name="dataId">数据标识符.</param>
    /// <param name="isGroup">是否为群组消息.</param>
    /// <returns><see cref="Task"/>.</returns>
    public async Task RemoveChatDataAsync(string dataId, bool isGroup = false)
    {
        _chatDb ??= await MigrationUtils.GetChatDbAsync(_workingDirectory).ConfigureAwait(false);
        var dataset = isGroup ? _chatDb.Groups : _chatDb.Sessions;
        var data = await dataset.FirstOrDefaultAsync(x => x.Id == dataId).ConfigureAwait(false);
        if (data is not null)
        {
            dataset.Remove(data);
            await _chatDb.SaveChangesAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// 获取所有绘图记录.
    /// </summary>
    /// <returns>JSON 列表.</returns>
    public async Task<List<string>> GetAllDrawSessionsAsync()
    {
        await InitializeDrawServiceAsync().ConfigureAwait(false);
        return await _drawService!.GetAllSessionsAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// 添加或更新绘图记录.
    /// </summary>
    /// <param name="dataId">绘图记录标识符.</param>
    /// <param name="value">绘图记录 JSON 数据.</param>
    /// <returns><see cref="Task"/>.</returns>
    public async Task AddOrUpdateDrawDataAsync(string dataId, string value)
    {
        await InitializeDrawServiceAsync().ConfigureAwait(false);
        await _drawService!.AddOrUpdateMetadataAsync(new DrawMeta { Id = dataId, Value = value }).ConfigureAwait(false);
    }

    /// <summary>
    /// 移除绘图记录.
    /// </summary>
    /// <param name="dataId">数据标识符.</param>
    /// <returns><see cref="Task"/>.</returns>
    public async Task RemoveDrawDataAsync(string dataId)
    {
        await InitializeDrawServiceAsync().ConfigureAwait(false);
        await _drawService!.RemoveMetadataAsync(dataId).ConfigureAwait(false);
    }

    /// <summary>
    /// 获取所有音频记录.
    /// </summary>
    /// <returns>JSON 列表.</returns>
    public async Task<List<string>> GetAllAudioSessionsAsync()
    {
        await InitializeAudioServiceAsync().ConfigureAwait(false);
        return await _audioService!.GetAllSessionsAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// 添加或更新音频记录.
    /// </summary>
    /// <param name="dataId">音频记录标识符.</param>
    /// <param name="value">音频记录 JSON 数据.</param>
    /// <returns><see cref="Task"/>.</returns>
    public async Task AddOrUpdateAudioDataAsync(string dataId, string value)
    {
        await InitializeAudioServiceAsync().ConfigureAwait(false);
        await _audioService!.AddOrUpdateMetadataAsync(new AudioMeta { Id = dataId, Value = value }).ConfigureAwait(false);
    }

    /// <summary>
    /// 移除音频记录.
    /// </summary>
    /// <param name="dataId">数据标识符.</param>
    /// <returns><see cref="Task"/>.</returns>
    public async Task RemoveAudioDataAsync(string dataId)
    {
        await InitializeAudioServiceAsync().ConfigureAwait(false);
        await _audioService!.RemoveMetadataAsync(dataId).ConfigureAwait(false);
    }

    private async Task InitializeSecretServiceAsync()
    {
        if (_secretService is not null)
        {
            return;
        }

        if (string.IsNullOrEmpty(_workingDirectory) || string.IsNullOrEmpty(_packageDirectory))
        {
            throw new InvalidOperationException("Working directory or package directory is not set.");
        }

        _secretService = new SecretDataService(_workingDirectory, _packageDirectory);
        await _secretService.InitializeAsync().ConfigureAwait(false);
    }

    private async Task InitializeDrawServiceAsync()
    {
        if (_drawService is not null)
        {
            return;
        }

        if (string.IsNullOrEmpty(_workingDirectory) || string.IsNullOrEmpty(_packageDirectory))
        {
            throw new InvalidOperationException("Working directory or package directory is not set.");
        }

        _drawService = new DrawDataService(_workingDirectory, _packageDirectory);
        await _drawService.InitializeAsync().ConfigureAwait(false);
    }

    private async Task InitializeAudioServiceAsync()
    {
        if (_audioService is not null)
        {
            return;
        }

        if (string.IsNullOrEmpty(_workingDirectory) || string.IsNullOrEmpty(_packageDirectory))
        {
            throw new InvalidOperationException("Working directory or package directory is not set.");
        }

        _audioService = new AudioDataService(_workingDirectory, _packageDirectory);
        await _audioService.InitializeAsync().ConfigureAwait(false);
    }
}
