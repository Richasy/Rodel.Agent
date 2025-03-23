// Copyright (c) Richasy. All rights reserved.

using RodelAgent.Models.Common;

namespace RodelAgent.Context;

/// <summary>
/// Provides methods to access secret data.
/// </summary>
public sealed class SecretDataService(string workingDir, string packageDir)
    : MetadataServiceBase<SecretMeta>(workingDir, packageDir, "secret.sqlite")
{
    public async Task<List<string>> GetAllSecretsAsync()
    {
        return await Task.Run(async () =>
        {
            using var sql = GetSql();
            var list = await sql.Queryable<SecretMeta>().ToListAsync().ConfigureAwait(false);
            return list.ConvertAll(p => p.Value);
        }).ConfigureAwait(false);
    }

    public async Task BatchAddSecretsAsync(List<SecretMeta> metadataList)
    {
        await Task.Run(async () =>
        {
            using var sql = GetSql();
            await sql.Insertable(metadataList).ExecuteCommandAsync().ConfigureAwait(false);
        }).ConfigureAwait(false);
    }

    public async Task<SecretMeta?> GetSecretAsync(string id)
    {
        return await Task.Run(async () =>
        {
            using var sql = GetSql();
            return await sql.Queryable<SecretMeta>().Where(p => p.Id == id).FirstAsync().ConfigureAwait(false);
        }).ConfigureAwait(false);
    }

    public async Task AddOrUpdateSecretAsync(SecretMeta metadata)
    {
        await Task.Run(async () =>
        {
            using var sql = GetSql();
            await sql.Storageable(metadata).ExecuteCommandAsync().ConfigureAwait(false);
        }).ConfigureAwait(false);
    }

    protected override async Task InitializeDbContextAsync(string path)
    {
        await Task.Run(() =>
        {
            using var sql = GetSql();
            sql.CodeFirst.InitTables<SecretMeta>();
        }).ConfigureAwait(false);
    }

    public async Task RemoveSecretAsync(string id)
    {
        await Task.Run(async () =>
        {
            using var sql = GetSql();
            await sql.Deleteable<SecretMeta>().Where(p => p.Id == id).ExecuteCommandAsync().ConfigureAwait(false);
        }).ConfigureAwait(false);
    }
}
