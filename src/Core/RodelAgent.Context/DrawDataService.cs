// Copyright (c) Richasy. All rights reserved.

using RodelAgent.Models.Common;

namespace RodelAgent.Context;

/// <summary>
/// 绘图数据服务.
/// </summary>
public sealed class DrawDataService(string workingDir, string packageDir)
    : MetadataServiceBase(workingDir, packageDir, "draw.ddb")
{
    public async Task<List<string>> GetAllSessionsAsync()
    {
        var list = await Sql!.Select<Metadata>().ToListAsync().ConfigureAwait(false);
        return list.ConvertAll(p => p.Value);
    }

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
                    p.Name("Sessions");
                    p.Property(x => x.Id).IsIdentity(true);
                });
        }).ConfigureAwait(false);
    }
}
