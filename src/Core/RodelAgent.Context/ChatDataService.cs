// Copyright (c) Richasy. All rights reserved.

using RodelAgent.Models.Common;

namespace RodelAgent.Context;

/// <summary>
/// 聊天数据服务.
/// </summary>
public sealed class ChatDataService(string workingDir, string packageDir) : IDisposable
{
    private bool disposedValue;

    private IFreeSql? Sql { get; set; }

    public async Task<List<string>> GetAllConversationsAsync()
    {
        var list = await Sql!.Select<ChatConversationMeta>().ToListAsync().ConfigureAwait(false);
        return list.ConvertAll(p => p.Value);
    }

    public async Task<List<string>> GetAllGroupsAsync()
    {
        var list = await Sql!.Select<ChatGroupMeta>().ToListAsync().ConfigureAwait(false);
        return list.ConvertAll(p => p.Value);
    }

    public async Task InitializeAsync()
    {
        if (Sql != null)
        {
            return;
        }

        var path = Path.Combine(workingDir, "chat.ddb");
        await DbTool.InitializeAsync("chat.ddb", packageDir, workingDir, InitializeDbContextAsync).ConfigureAwait(false);
    }

    public async Task BatchAddConversationsAsync(List<ChatConversationMeta> metadataList)
        => await Sql!.Insert<ChatConversationMeta>().AppendData(metadataList).ExecuteAffrowsAsync().ConfigureAwait(false);

    public async Task BatchAddGroupsAsync(List<ChatGroupMeta> metadataList)
        => await Sql!.Insert<ChatGroupMeta>().AppendData(metadataList).ExecuteAffrowsAsync().ConfigureAwait(false);

    public async Task<ChatConversationMeta?> GetConversationAsync(string id)
        => (await Sql!.Select<ChatConversationMeta>().Where(p => p.Id == id).ToListAsync().ConfigureAwait(false)).FirstOrDefault();

    public async Task<ChatGroupMeta?> GetGroupAsync(string id)
        => (await Sql!.Select<ChatGroupMeta>().Where(p => p.Id == id).ToListAsync().ConfigureAwait(false)).FirstOrDefault();

    public async Task AddOrUpdateConversationAsync(ChatConversationMeta metadata)
    {
        var source = (await Sql!.Select<ChatConversationMeta>().Where(p => p.Id == metadata.Id).ToListAsync().ConfigureAwait(false)).FirstOrDefault();
        if (source != null)
        {
            source.Value = metadata.Value;
        }

        await Sql!.InsertOrUpdate<ChatConversationMeta>().SetSource(metadata).ExecuteAffrowsAsync().ConfigureAwait(false);
    }

    public async Task AddOrUpdateGroupAsync(ChatGroupMeta metadata)
    {
        var source = (await Sql!.Select<ChatGroupMeta>().Where(p => p.Id == metadata.Id).ToListAsync().ConfigureAwait(false)).FirstOrDefault();
        if (source != null)
        {
            source.Value = metadata.Value;
        }

        await Sql!.InsertOrUpdate<ChatGroupMeta>().SetSource(metadata).ExecuteAffrowsAsync().ConfigureAwait(false);
    }

    public async Task RemoveConversationAsync(string id)
        => await Sql!.Delete<ChatConversationMeta>().Where(p => p.Id == id).ExecuteAffrowsAsync().ConfigureAwait(false);

    public async Task RemoveGroupAsync(string id)
        => await Sql!.Delete<ChatGroupMeta>().Where(p => p.Id == id).ExecuteAffrowsAsync().ConfigureAwait(false);

    public void Dispose()
    {
        // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private async Task InitializeDbContextAsync(string path)
    {
        await Task.Run(() =>
        {
            Sql = new FreeSql.FreeSqlBuilder()
                .UseConnectionString(FreeSql.DataType.DuckDB, $"DataSource={path}")
                .UseAutoSyncStructure(true)
                .UseLazyLoading(true)
                .Build();

            Sql.CodeFirst
                .ConfigEntity<ChatConversationMeta>(p =>
                {
                    p.Name("Conversations");
                    p.Property(x => x.Id).IsIdentity(true);
                });

            Sql.CodeFirst
                .ConfigEntity<ChatGroupMeta>(p =>
                {
                    p.Name("Groups");
                    p.Property(x => x.Id).IsIdentity(true);
                });
        }).ConfigureAwait(false);
    }

    private void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                Sql?.Dispose();
            }

            Sql = null;
            disposedValue = true;
        }
    }
}
