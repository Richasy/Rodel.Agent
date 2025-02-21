// Copyright (c) Richasy. All rights reserved.

using RodelAgent.Models.Common;

namespace RodelAgent.Context;

/// <summary>
/// Provides methods to access secret data.
/// </summary>
public sealed partial class SecretDataService
{
    private async Task InitializeDbContextAsync(string path)
    {
        await Task.Run(() =>
        {
            _freeSql = new FreeSql.FreeSqlBuilder()
                .UseConnectionString(FreeSql.DataType.DuckDB, $"DataSource={path}")
                .UseAutoSyncStructure(true)
                .UseLazyLoading(true)
                .Build();

            _freeSql.CodeFirst
                .ConfigEntity<Metadata>(p =>
                {
                    p.Name("Metadata");
                    p.Property(x => x.Id).IsIdentity(true);
                });
        }).ConfigureAwait(false);
    }
}
