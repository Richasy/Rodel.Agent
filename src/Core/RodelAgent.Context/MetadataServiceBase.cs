// Copyright (c) Richasy. All rights reserved.

using RodelAgent.Models.Common;

namespace RodelAgent.Context;

public abstract class MetadataServiceBase<T>(string workingDir, string packageDir, string dbName) : IDisposable
    where T : Metadata
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

    public async Task BatchAddMetadataAsync(List<T> metadataList)
        => await Sql!.Insert<T>().AppendData(metadataList).ExecuteAffrowsAsync().ConfigureAwait(false);

    public async Task<T?> GetMetadataAsync(string id)
        => (await Sql!.Select<T>().Where(p => p.Id == id).ToListAsync().ConfigureAwait(false)).FirstOrDefault();

    public async Task AddOrUpdateMetadataAsync(T metadata)
    {
        var source = (await Sql!.Select<T>().Where(p => p.Id == metadata.Id).ToListAsync().ConfigureAwait(false)).FirstOrDefault();
        if (source != null)
        {
            source.Value = metadata.Value;
        }

        await Sql!.InsertOrUpdate<T>().SetSource(metadata).ExecuteAffrowsAsync().ConfigureAwait(false);
    }

    public async Task RemoveMetadataAsync(string id)
        => await Sql!.Delete<T>().Where(p => p.Id == id).ExecuteAffrowsAsync().ConfigureAwait(false);

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

    protected abstract Task InitializeDbContextAsync(string path);
}
