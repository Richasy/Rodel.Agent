// Copyright (c) Richasy. All rights reserved.

using RodelAgent.Models.Common;

namespace RodelAgent.Context;

/// <summary>
/// Provides methods to access secret data.
/// </summary>
public sealed partial class SecretDataService(string workingDir, string packageDir)
{
    private const string DbName = "secret.db";
    private IFreeSql? _freeSql;

    public async Task InitializeAsync()
    {
        if (_freeSql != null)
        {
            return;
        }

        var path = Path.Combine(workingDir, DbName);
        await DbTool.InitializeAsync(DbName, packageDir, workingDir, InitializeDbContextAsync).ConfigureAwait(false);
    }

    public async Task<Metadata?> GetSecretAsync(string id)
        => (await _freeSql!.Select<Metadata>().Where(p => p.Id == id).ToListAsync().ConfigureAwait(false)).FirstOrDefault();

    public async Task AddOrUpdateSecretAsync(Metadata metadata)
    {
        var source = (await _freeSql!.Select<Metadata>().Where(p => p.Id == metadata.Id).ToListAsync().ConfigureAwait(false)).FirstOrDefault();
        if (source != null)
        {
            source.Value = metadata.Value;
        }

        await _freeSql!.InsertOrUpdate<Metadata>().SetSource(metadata).ExecuteAffrowsAsync().ConfigureAwait(false);
    }

    public async Task RemoveSecretAsync(string id)
        => await _freeSql!.Delete<Metadata>().Where(p => p.Id == id).ExecuteAffrowsAsync().ConfigureAwait(false);
}
