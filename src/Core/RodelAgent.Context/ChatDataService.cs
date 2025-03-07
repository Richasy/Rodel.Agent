// Copyright (c) Richasy. All rights reserved.

using RodelAgent.Models.Common;
using SqlSugar;
using System.Reflection;

namespace RodelAgent.Context;

/// <summary>
/// 聊天数据服务.
/// </summary>
public sealed class ChatDataService(string workingDir, string packageDir)
{
    private const string DbName = "chat.sqlite";
    private bool _isInitialized;

    public async Task<List<string>> GetAllConversationsAsync()
    {
        return await Task.Run(async () =>
        {
            using var sql = GetSql();
            var list = await sql.Queryable<ChatConversationMeta>().ToListAsync().ConfigureAwait(false);
            return list.ConvertAll(p => p.Value);
        }).ConfigureAwait(false);
    }

    public async Task<List<string>> GetAllGroupsAsync()
    {
        return await Task.Run(async () =>
        {
            using var sql = GetSql();
            var list = await sql.Queryable<ChatGroupMeta>().ToListAsync().ConfigureAwait(false);
            return list.ConvertAll(p => p.Value);
        }).ConfigureAwait(false);
    }

    public async Task InitializeAsync()
    {
        if (_isInitialized)
        {
            return;
        }

        var path = Path.Combine(workingDir, DbName);
        await DbTool.InitializeAsync(DbName, packageDir, workingDir, InitializeDbContextAsync).ConfigureAwait(false);
        _isInitialized = true;
    }

    public async Task BatchAddConversationsAsync(List<ChatConversationMeta> metadataList)
    {
        await Task.Run(async () =>
        {
            using var sql = GetSql();
            await sql.Insertable(metadataList).ExecuteCommandAsync().ConfigureAwait(false);
        }).ConfigureAwait(false);
    }

    public async Task BatchAddGroupsAsync(List<ChatGroupMeta> metadataList)
    {
        await Task.Run(async () =>
        {
            using var sql = GetSql();
            await sql.Insertable(metadataList).ExecuteCommandAsync().ConfigureAwait(false);
        }).ConfigureAwait(false);
    }

    public async Task<ChatConversationMeta?> GetConversationAsync(string id)
    {
        return await Task.Run(async () =>
        {
            using var sql = GetSql();
            return await sql.Queryable<ChatConversationMeta>().Where(p => p.Id == id).FirstAsync().ConfigureAwait(false);
        }).ConfigureAwait(false);
    }

    public async Task<ChatGroupMeta?> GetGroupAsync(string id)
    {
        return await Task.Run(async () =>
        {
            using var sql = GetSql();
            return await sql.Queryable<ChatGroupMeta>().Where(p => p.Id == id).FirstAsync().ConfigureAwait(false);
        }).ConfigureAwait(false);
    }

    public async Task AddOrUpdateConversationAsync(ChatConversationMeta metadata)
    {
        await Task.Run(async () =>
        {
            using var sql = GetSql();
            await sql.Storageable(metadata).ExecuteCommandAsync().ConfigureAwait(false);
        }).ConfigureAwait(false);
    }

    public async Task AddOrUpdateGroupAsync(ChatGroupMeta metadata)
    {
        await Task.Run(async () =>
        {
            using var sql = GetSql();
            await sql.Storageable(metadata).ExecuteCommandAsync().ConfigureAwait(false);
        }).ConfigureAwait(false);
    }

    public async Task RemoveConversationAsync(string id)
    {
        await Task.Run(async () =>
        {
            using var sql = GetSql();
            await sql.Deleteable<ChatConversationMeta>().Where(p => p.Id == id).ExecuteCommandAsync().ConfigureAwait(false);
        }).ConfigureAwait(false);
    }

    public async Task RemoveGroupAsync(string id)
    {
        await Task.Run(async () =>
        {
            using var sql = GetSql();
            await sql.Deleteable<ChatGroupMeta>().Where(p => p.Id == id).ExecuteCommandAsync().ConfigureAwait(false);
        }).ConfigureAwait(false);
    }

    private async Task InitializeDbContextAsync(string path)
    {
        await Task.Run(() =>
        {
            using var sql = GetSql();
            sql.CodeFirst.InitTables<ChatConversationMeta>();
            sql.CodeFirst.InitTables<ChatGroupMeta>();
        }).ConfigureAwait(false);
    }

    private SqlSugarClient GetSql()
    {
        return new SqlSugarClient(new ConnectionConfig()
        {
            ConnectionString = $"Data Source={Path.Combine(workingDir, DbName)}",
            DbType = DbType.Sqlite,
            IsAutoCloseConnection = true,
            ConfigureExternalServices = new ConfigureExternalServices
            {
                EntityService = (c, p) =>
                {
                    if (!p.IsPrimarykey && new NullabilityInfoContext().Create(c).WriteState is NullabilityState.Nullable)
                    {
                        p.IsNullable = true;
                    }
                }
            }
        });
    }
}
