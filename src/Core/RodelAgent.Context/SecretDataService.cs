// Copyright (c) Richasy. All rights reserved.

using RodelAgent.Models.Common;

namespace RodelAgent.Context;

/// <summary>
/// Provides methods to access secret data.
/// </summary>
public sealed partial class SecretDataService(string workingDir, string packageDir)
    : MetadataServiceBase(workingDir, packageDir, "secret.ddb")
{
    protected override async Task InitializeDbContextAsync(string path)
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
                    p.Name("Secrets");
                    p.Property(x => x.Id).IsIdentity(true);
                });
        }).ConfigureAwait(false);
    }
}
