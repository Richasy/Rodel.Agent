// Copyright (c) Richasy. All rights reserved.

using RodelAgent.Context;
using RodelAgent.Models.Common;
using RodelAgent.UI.Models.Constants;
using SqlSugar;
using System.Reflection;
using Windows.ApplicationModel;

namespace RodelAgent.UI.Toolkits;

/// <summary>
/// 迁移工具箱.
/// </summary>
public static class MigrationToolkit
{
    internal static bool ShouldMigrate()
    {
        var libPath = SettingsToolkit.ReadLocalSetting(SettingNames.WorkingDirectory, string.Empty);
        var dbFiles = Directory.EnumerateFiles(libPath, "*.db", SearchOption.TopDirectoryOnly).Where(p => !p.Contains(".old", StringComparison.Ordinal));
        return dbFiles.Any();
    }

    internal static async Task TryMigrateAsync()
    {
        var libPath = SettingsToolkit.ReadLocalSetting(SettingNames.WorkingDirectory, string.Empty);
        var dbFiles = Directory.EnumerateFiles(libPath, "*.db", SearchOption.TopDirectoryOnly).Where(p => !p.Contains(".old", StringComparison.Ordinal));

        SettingsToolkit.WriteLocalSetting(SettingNames.MigrationFailed, false);

        try
        {
            var shouldMove = File.Exists(Path.Combine(libPath, "move"));
            foreach (var dbFile in dbFiles)
            {
                if(!shouldMove)
                {
                    var dbFileName = Path.GetFileName(dbFile);
                    if (dbFileName == "secret.db")
                    {
                        await MigrateSecretDbAsync(dbFile);
                    }
                    else if (dbFileName == "draw.db")
                    {
                        await MigrateDrawDbAsync(dbFile);
                    }
                    else if (dbFileName == "audio.db")
                    {
                        await MigrateAudioDbAsync(dbFile);
                    }
                }
                else
                {
                    await MoveOldDbFileAsync(dbFile);
                }
            }

            if (!shouldMove)
            {
                await File.Create(Path.Combine(libPath, "move")).DisposeAsync();
            }
            else
            {
                File.Delete(Path.Combine(libPath, "move"));
            }
        }
        catch (Exception ex)
        {
            GlobalDependencies.Kernel.Get<ILogger<App>>().LogError(ex, $"Failed to migrate database.");
            SettingsToolkit.WriteLocalSetting(SettingNames.MigrationFailed, true);
            throw;
        }
    }

    private static async Task MigrateSecretDbAsync(string dbPath)
    {
        var secrets = await GetOldDataFromDatabaseAsync<SecretMeta>(dbPath, "Metadata");
        var libPath = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.WorkingDirectory, string.Empty);
        var service = new SecretDataService(libPath, Package.Current.InstalledPath);
        await service.InitializeAsync();
        await service.BatchAddSecretsAsync(secrets);
    }

    private static async Task MigrateDrawDbAsync(string dbPath)
    {
        var secrets = await GetOldDataFromDatabaseAsync<DrawMeta>(dbPath, "Sessions");
        var libPath = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.WorkingDirectory, string.Empty);
        var service = new DrawDataService(libPath, Package.Current.InstalledPath);
        await service.InitializeAsync();
        await service.BatchAddSessionsAsync(secrets);
    }

    private static async Task MigrateAudioDbAsync(string dbPath)
    {
        var secrets = await GetOldDataFromDatabaseAsync<AudioMeta>(dbPath, "Sessions");
        var libPath = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.WorkingDirectory, string.Empty);
        var service = new AudioDataService(libPath, Package.Current.InstalledPath);
        await service.InitializeAsync();
        await service.BatchAddSessionsAsync(secrets);
    }

    private static async Task<List<T>> GetOldDataFromDatabaseAsync<T>(string dbPath, string tableName)
    {
        return await Task.Run(async () =>
        {
            using var sql = new SqlSugarClient(new ConnectionConfig
            {
                ConnectionString = $"Data Source={dbPath}",
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

            sql.CodeFirst.InitTables<T>();
            return await sql.Queryable<T>().AS(tableName).ToListAsync();
        });
    }

    private static async Task MoveOldDbFileAsync(string dbPath)
    {
        try
        {
            await Task.Delay(1000);
            var libFolder = SettingsToolkit.ReadLocalSetting(SettingNames.WorkingDirectory, string.Empty);
            var relativePath = Path.GetRelativePath(libFolder, Path.GetDirectoryName(dbPath)!);
            relativePath = relativePath.Trim('\\');
            var sqliteFolder = Path.Combine(libFolder, ".old");
            var oldFolder = Path.Combine(sqliteFolder, relativePath);
            if (!Directory.Exists(oldFolder))
            {
                Directory.CreateDirectory(oldFolder);
            }

            var oldFileName = Path.Combine(oldFolder!, Path.GetFileName(dbPath));
            File.Move(dbPath, oldFileName, true);
        }
        catch (Exception ex)
        {
            GlobalDependencies.Kernel.Get<ILogger<App>>().LogError(ex, $"Failed to move old database file. {Path.GetFileName(dbPath)}");
            throw;
        }
    }
}
