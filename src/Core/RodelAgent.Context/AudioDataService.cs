// Copyright (c) Richasy. All rights reserved.

using RodelAgent.Models.Common;

namespace RodelAgent.Context;

/// <summary>
/// 音频数据服务.
/// </summary>
public sealed class AudioDataService(string workingDir, string packageDir)
    : MetadataServiceBase<AudioMeta>(workingDir, packageDir, "audio.ddb")
{
    public async Task<List<string>> GetAllSessionsAsync()
    {
        var list = await Sql!.Select<AudioMeta>().ToListAsync().ConfigureAwait(false);
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
                .ConfigEntity<AudioMeta>(p =>
                {
                    p.Name("Sessions");
                    p.Property(x => x.Id).IsIdentity(true);
                });
        }).ConfigureAwait(false);
    }
}
