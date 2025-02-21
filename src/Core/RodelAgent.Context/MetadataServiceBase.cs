// Copyright (c) Richasy. All rights reserved.

using RodelAgent.Models.Common;

namespace RodelAgent.Context;

public abstract class MetadataServiceBase(string workingDir, string packageDir, string dbName) : IDisposable
{
    private bool disposedValue;

    protected IFreeSql? Sql { get; set; }

    public async Task InitializeAsync()
    {
        if (Sql != null)
        {
            return;
        }

        var path = Path.Combine(workingDir, dbName);
        await DbTool.InitializeAsync(dbName, packageDir, workingDir, InitializeDbContextAsync).ConfigureAwait(false);
    }

    public async Task BatchAddMetadataAsync(List<Metadata> metadataList)
        => await Sql!.Insert<Metadata>().AppendData(metadataList).ExecuteAffrowsAsync().ConfigureAwait(false);

    public async Task<Metadata?> GetMetadataAsync(string id)
        => (await Sql!.Select<Metadata>().Where(p => p.Id == id).ToListAsync().ConfigureAwait(false)).FirstOrDefault();

    public async Task AddOrUpdateMetadataAsync(Metadata metadata)
    {
        var source = (await Sql!.Select<Metadata>().Where(p => p.Id == metadata.Id).ToListAsync().ConfigureAwait(false)).FirstOrDefault();
        if (source != null)
        {
            source.Value = metadata.Value;
        }

        await Sql!.InsertOrUpdate<Metadata>().SetSource(metadata).ExecuteAffrowsAsync().ConfigureAwait(false);
    }

    public async Task RemoveMetadataAsync(string id)
        => await Sql!.Delete<Metadata>().Where(p => p.Id == id).ExecuteAffrowsAsync().ConfigureAwait(false);

    public void Dispose()
    {
        // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
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

    protected virtual async Task InitializeDbContextAsync(string path)
    {
        await Task.Run(() =>
        {
            Sql = new FreeSql.FreeSqlBuilder()
                .UseConnectionString(FreeSql.DataType.DuckDB, $"DataSource={path}")
                .UseAutoSyncStructure(true)
                .UseLazyLoading(true)
                .Build();

            Sql.CodeFirst
                .ConfigEntity<Metadata>(p =>
                {
                    p.Name("Metadata");
                    p.Property(x => x.Id).IsIdentity(true);
                });
        }).ConfigureAwait(false);
    }
}
