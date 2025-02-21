// Copyright (c) Richasy. All rights reserved.

using RichasyKernel;
using RodelAgent.Context;
using RodelAgent.UI.Models.Constants;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using Windows.ApplicationModel;

namespace RodelAgent.UI.Toolkits;

/// <summary>
/// 迁移工具箱.
/// </summary>
public static class MigrationToolkit
{
    internal static async Task TryMigrateAsync()
    {
        var libPath = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.WorkingDirectory, string.Empty);
        var dbFiles = Directory.EnumerateFiles(libPath, "*.db", SearchOption.TopDirectoryOnly);
        if (!dbFiles.Any())
        {
            return;
        }

        SettingsToolkit.WriteLocalSetting(SettingNames.MigrationFailed, false);

        foreach (var dbFile in dbFiles)
        {
            try
            {
                var dbFileName = Path.GetFileName(dbFile);
                var delFile = Path.Combine(libPath, $"{dbFileName}.del");
                if (File.Exists(delFile))
                {
                    MoveOldDbFile(dbFile);
                    continue;
                }

                if (dbFileName == "secret.db")
                {
                    await MigrateSecretDbAsync(dbFile);
                }

                await File.Create(delFile).DisposeAsync();
            }
            catch (Exception ex)
            {
                GlobalDependencies.Kernel.Get<ILogger<App>>().LogError(ex, $"Failed to migrate database. {Path.GetFileName(dbFile)}");
                SettingsToolkit.WriteLocalSetting(SettingNames.MigrationFailed, true);
                throw;
            }
        }
    }

    private static async Task MigrateSecretDbAsync(string dbPath)
    {
        var json = await GetJsonFromDatabaseAsync(dbPath, "Metadata");
        var metadatas = JsonSerializer.Deserialize(json, JsonGenContext.Default.ListMetadata);
        var libPath = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.WorkingDirectory, string.Empty);
        using var service = new SecretDataService(libPath, Package.Current.InstalledPath);
        await service.InitializeAsync();
        await service.BatchAddSecretsAsync(metadatas ?? []);
    }

    private static async Task<string> GetJsonFromDatabaseAsync(string dbPath, string tableName)
    {
        var architecture = RuntimeInformation.ProcessArchitecture;
        var identifier = architecture == Architecture.Arm64 ? "arm64" : "x64";
        var exeFile = Path.Combine(Package.Current.InstalledPath, "Assets", "SqliteJson", identifier, "SqliteJson.exe");
        var taskCompletion = new TaskCompletionSource<string>();
        var sb = new StringBuilder();
        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = exeFile,
                Arguments = $"\"{dbPath}\" \"{tableName}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            },
            EnableRaisingEvents = true,
        };
        process.OutputDataReceived += (sender, e) =>
        {
            if (e.Data is not null)
            {
                if (e.Data == "__DONE__")
                {
                    taskCompletion.TrySetResult(sb.ToString());
                    return;
                }

                sb.AppendLine(e.Data);
            }
        };
        process.ErrorDataReceived += (sender, e) =>
        {
            if (e.Data is not null)
            {
                taskCompletion.TrySetException(new KernelException(e.Data));
            }
        };
        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        var json = await taskCompletion.Task;
        await process.WaitForExitAsync();
        return json;
    }

    private static void MoveOldDbFile(string dbPath)
    {
        try
        {
            var folder = Path.GetDirectoryName(dbPath);
            var oldFolder = Path.Combine(folder!, ".sqlite");
            if (!Directory.Exists(oldFolder))
            {
                Directory.CreateDirectory(oldFolder);
            }

            var oldFileName = Path.Combine(oldFolder!, Path.GetFileName(dbPath));
            File.Move(dbPath, oldFileName);
            var delFile = Path.Combine(folder!, $"{Path.GetFileName(dbPath)}.del");
            if (File.Exists(delFile))
            {
                File.Delete(delFile);
            }
        }
        catch (Exception ex)
        {
            GlobalDependencies.Kernel.Get<ILogger<App>>().LogError(ex, $"Failed to move old database file. {Path.GetFileName(dbPath)}");
        }
    }
}
