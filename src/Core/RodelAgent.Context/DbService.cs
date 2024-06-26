// Copyright (c) Rodel. All rights reserved.

using Microsoft.EntityFrameworkCore;
using RodelAgent.Models.Common;

namespace RodelAgent.Context;

/// <summary>
/// 数据库服务.
/// </summary>
public sealed class DbService
{
    private SecretDbContext? _secretDb;
    private ChatDbContext? _chatDb;
    private TranslateDbContext? _translateDb;
    private DrawDbContext? _drawDb;
    private AudioDbContext? _audioDb;
    private string _workingDirectory;

    /// <summary>
    /// 设置工作目录.
    /// </summary>
    /// <param name="workingDirectory">工作目录.</param>
    public void SetWorkingDirectory(string workingDirectory)
        => _workingDirectory = workingDirectory;

    /// <summary>
    /// 获取机密配置.
    /// </summary>
    /// <param name="key">名称.</param>
    /// <returns>字符串.</returns>
    public async Task<string?> GetSecretAsync(string key)
    {
        _secretDb ??= await MigrationUtils.GetSecretDbAsync(_workingDirectory);
        var data = await _secretDb.Metadata.FirstOrDefaultAsync(x => x.Id == key);
        return data?.Value;
    }

    /// <summary>
    /// 设置机密配置.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    public async Task SetSecretAsync(string key, string value)
    {
        _secretDb ??= await MigrationUtils.GetSecretDbAsync(_workingDirectory);
        var data = await _secretDb.Metadata.FirstOrDefaultAsync(x => x.Id == key);
        if (data is null)
        {
            await _secretDb.Metadata.AddAsync(new Metadata { Id = key, Value = value });
        }
        else
        {
            data.Value = value;
            _secretDb.Metadata.Update(data);
        }

        await _secretDb.SaveChangesAsync();
    }

    /// <summary>
    /// 获取所有聊天数据.
    /// </summary>
    /// <returns>JSON 列表.</returns>
    public async Task<List<string>> GetAllChatSessionAsync()
    {
        _chatDb ??= await MigrationUtils.GetChatDbAsync(_workingDirectory);
        return await _chatDb.Sessions.Select(p => p.Value).ToListAsync();
    }

    /// <summary>
    /// 获取所有群组会话数据.
    /// </summary>
    /// <returns>JSON 列表.</returns>
    public async Task<List<string>> GetAllChatGroupAsync()
    {
        _chatDb ??= await MigrationUtils.GetChatDbAsync(_workingDirectory);
        return await _chatDb.Groups.Select(p => p.Value).ToListAsync();
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
        _chatDb ??= await MigrationUtils.GetChatDbAsync(_workingDirectory);
        var dataset = isGroup ? _chatDb.Groups : _chatDb.Sessions;
        var data = await dataset.FirstOrDefaultAsync(x => x.Id == dataId);
        if (data is null)
        {
            await dataset.AddAsync(new Metadata { Id = dataId, Value = value });
        }
        else
        {
            data.Value = value;
            dataset.Update(data);
        }

        await _chatDb.SaveChangesAsync();
    }

    /// <summary>
    /// 移除聊天会话.
    /// </summary>
    /// <param name="dataId">数据标识符.</param>
    /// <param name="isGroup">是否为群组消息.</param>
    /// <returns><see cref="Task"/>.</returns>
    public async Task RemoveChatDataAsync(string dataId, bool isGroup = false)
    {
        _chatDb ??= await MigrationUtils.GetChatDbAsync(_workingDirectory);
        var dataset = isGroup ? _chatDb.Groups : _chatDb.Sessions;
        var data = await dataset.FirstOrDefaultAsync(x => x.Id == dataId);
        if (data is not null)
        {
            dataset.Remove(data);
            await _chatDb.SaveChangesAsync();
        }
    }

    /// <summary>
    /// 获取所有翻译记录.
    /// </summary>
    /// <returns>JSON 列表.</returns>
    public async Task<List<string>> GetAllTranslateSessionAsync()
    {
        _translateDb ??= await MigrationUtils.GetTranslateDbAsync(_workingDirectory);
        return await _translateDb.Sessions.Select(p => p.Value).ToListAsync();
    }

    /// <summary>
    /// 添加或更新翻译记录.
    /// </summary>
    /// <param name="dataId">翻译记录标识符.</param>
    /// <param name="value">翻译记录 JSON 数据.</param>
    /// <returns><see cref="Task"/>.</returns>
    public async Task AddOrUpdateTranslateDataAsync(string dataId, string value)
    {
        _translateDb ??= await MigrationUtils.GetTranslateDbAsync(_workingDirectory);
        var dataset = _translateDb.Sessions;
        var data = await dataset.FirstOrDefaultAsync(x => x.Id == dataId);
        if (data is null)
        {
            await dataset.AddAsync(new Metadata { Id = dataId, Value = value });
        }
        else
        {
            data.Value = value;
            dataset.Update(data);
        }

        await _translateDb.SaveChangesAsync();
    }

    /// <summary>
    /// 移除翻译记录.
    /// </summary>
    /// <param name="dataId">数据标识符.</param>
    /// <returns><see cref="Task"/>.</returns>
    public async Task RemoveTranslateDataAsync(string dataId)
    {
        _translateDb ??= await MigrationUtils.GetTranslateDbAsync(_workingDirectory);
        var dataset = _translateDb.Sessions;
        var data = await dataset.FirstOrDefaultAsync(x => x.Id == dataId);
        if (data is not null)
        {
            dataset.Remove(data);
            await _translateDb.SaveChangesAsync();
        }
    }

    /// <summary>
    /// 获取所有绘图记录.
    /// </summary>
    /// <returns>JSON 列表.</returns>
    public async Task<List<string>> GetAllDrawSessionAsync()
    {
        _drawDb ??= await MigrationUtils.GetDrawDbAsync(_workingDirectory);
        return await _drawDb.Sessions.Select(p => p.Value).ToListAsync();
    }

    /// <summary>
    /// 添加或更新绘图记录.
    /// </summary>
    /// <param name="dataId">绘图记录标识符.</param>
    /// <param name="value">绘图记录 JSON 数据.</param>
    /// <returns><see cref="Task"/>.</returns>
    public async Task AddOrUpdateDrawDataAsync(string dataId, string value)
    {
        _drawDb ??= await MigrationUtils.GetDrawDbAsync(_workingDirectory);
        var dataset = _drawDb.Sessions;
        var data = await dataset.FirstOrDefaultAsync(x => x.Id == dataId);
        if (data is null)
        {
            await dataset.AddAsync(new Metadata { Id = dataId, Value = value });
        }
        else
        {
            data.Value = value;
            dataset.Update(data);
        }

        await _drawDb.SaveChangesAsync();
    }

    /// <summary>
    /// 移除绘图记录.
    /// </summary>
    /// <param name="dataId">数据标识符.</param>
    /// <returns><see cref="Task"/>.</returns>
    public async Task RemoveDrawDataAsync(string dataId)
    {
        _drawDb ??= await MigrationUtils.GetDrawDbAsync(_workingDirectory);
        var dataset = _drawDb.Sessions;
        var data = await dataset.FirstOrDefaultAsync(x => x.Id == dataId);
        if (data is not null)
        {
            dataset.Remove(data);
            await _drawDb.SaveChangesAsync();
        }
    }

    /// <summary>
    /// 获取所有音频记录.
    /// </summary>
    /// <returns>JSON 列表.</returns>
    public async Task<List<string>> GetAllAudioSessionAsync()
    {
        _audioDb ??= await MigrationUtils.GetAudioDbAsync(_workingDirectory);
        return await _audioDb.Sessions.Select(p => p.Value).ToListAsync();
    }

    /// <summary>
    /// 添加或更新音频记录.
    /// </summary>
    /// <param name="dataId">音频记录标识符.</param>
    /// <param name="value">音频记录 JSON 数据.</param>
    /// <returns><see cref="Task"/>.</returns>
    public async Task AddOrUpdateAudioDataAsync(string dataId, string value)
    {
        _audioDb ??= await MigrationUtils.GetAudioDbAsync(_workingDirectory);
        var dataset = _audioDb.Sessions;
        var data = await dataset.FirstOrDefaultAsync(x => x.Id == dataId);
        if (data is null)
        {
            await dataset.AddAsync(new Metadata { Id = dataId, Value = value });
        }
        else
        {
            data.Value = value;
            dataset.Update(data);
        }

        await _audioDb.SaveChangesAsync();
    }

    /// <summary>
    /// 移除音频记录.
    /// </summary>
    /// <param name="dataId">数据标识符.</param>
    /// <returns><see cref="Task"/>.</returns>
    public async Task RemoveAudioDataAsync(string dataId)
    {
        _audioDb ??= await MigrationUtils.GetAudioDbAsync(_workingDirectory);
        var dataset = _audioDb.Sessions;
        var data = await dataset.FirstOrDefaultAsync(x => x.Id == dataId);
        if (data is not null)
        {
            dataset.Remove(data);
            await _audioDb.SaveChangesAsync();
        }
    }

    /// <summary>
    /// 重置所有数据库连接.
    /// </summary>
    public void ResetAllDbConnections()
    {
        _secretDb?.Dispose();
        _chatDb?.Dispose();
        _translateDb?.Dispose();
        _drawDb?.Dispose();
        _audioDb?.Dispose();

        _secretDb = null;
        _chatDb = null;
        _translateDb = null;
        _drawDb = null;
        _audioDb = null;
    }
}
