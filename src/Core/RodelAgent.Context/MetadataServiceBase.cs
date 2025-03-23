// Copyright (c) Richasy. All rights reserved.

using RodelAgent.Models.Common;
using SqlSugar;
using System.Reflection;

namespace RodelAgent.Context;

public abstract class MetadataServiceBase<T>(string workingDir, string packageDir, string dbName)
    where T : Metadata
{
    private bool _isInitialized;

    public async Task InitializeAsync()
    {
        if (_isInitialized)
        {
            return;
        }

        await DbTool.InitializeAsync(dbName, packageDir, workingDir, InitializeDbContextAsync).ConfigureAwait(false);
        _isInitialized = true;
    }

    protected SqlSugarClient GetSql()
    {
        return new SqlSugarClient(new ConnectionConfig()
        {
            ConnectionString = $"Data Source={Path.Combine(workingDir, dbName)}",
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

    protected abstract Task InitializeDbContextAsync(string path);
}
